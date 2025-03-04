using MongoDB.Bson;
using System.Text.RegularExpressions;

namespace MaintenanceServer
{
    public class CouponManager : Singleton<CouponManager>
    {
        private Dictionary<ObjectId, CouponDao> m_couponMap = new();
        private HashSet<string> m_keywordHashSet = new();
        private string m_couponStringPattern = @"^[0-9A-Z]+$";


        public void Initialize()
        {
            Refresh();
        }

        public void Refresh()
        {
            var proc = new spSelectAllCouponData();
            if (proc.Run())
            {
                Dictionary<ObjectId, CouponDao> couponMap = new();
                HashSet<string> keywordHashSet = new();

                for (int i = 0, count = proc.CouponList.Count; i < count; ++i)
                {
                    var couponInfo = proc.CouponList[i];
                    couponMap.Add(couponInfo._id, couponInfo);
                }
                for (int i = 0, count = proc.KeywordCouponCodeList.Count; i < count; ++i)
                {
                    var KeywordCouponCode = proc.KeywordCouponCodeList[i];
                    keywordHashSet.Add(KeywordCouponCode.Keyword);
                }

                m_couponMap = couponMap;
                m_keywordHashSet = keywordHashSet;
            }
        }

        public async Task RefreshAsync()
        {
            var proc = new spSelectAllCouponData();
            if (await proc.StartPoolAync())
            {
                Dictionary<ObjectId, CouponDao> couponMap = new();
                HashSet<string> keywordHashSet = new();

                for (int i = 0, count = proc.CouponList.Count; i < count; ++i)
                {
                    var couponInfo = proc.CouponList[i];
                    couponMap.Add(couponInfo._id, couponInfo);
                }
                for (int i = 0, count = proc.KeywordCouponCodeList.Count; i < count; ++i)
                {
                    var KeywordCouponCode = proc.KeywordCouponCodeList[i];
                    keywordHashSet.Add(KeywordCouponCode.Keyword);
                }

                m_couponMap = couponMap;
                m_keywordHashSet = keywordHashSet;
            }
        }

        public bool CheckKeywordCouponCode(string keyword)
        {
            return m_keywordHashSet.Contains(keyword);
        }

        public CouponDao GetCoupon(ObjectId id)
        {
            m_couponMap.TryGetValue(id, out var couponInfo);
            return couponInfo;
        }



        //TODO: 메일 보상을 받을 Character OwnerKey 필요
        public async Task<EResult> UseCoupon(string couponCode, long playerId)
        {
            if (ValidCouponString(couponCode) == false)
            {
                return EResult.Error_InvalidParameter;
            }

            CouponDao coupon = null;
            KeywordCouponCodeDao keywordCouponCode = null;
            SerialCouponCodeDao serialCouponCode = null;

            if (CheckKeywordCouponCode(couponCode))
            {
                var keywordCouponCodeSelectProc = new spSelectKeywordCouponCode(couponCode);
                if (await keywordCouponCodeSelectProc.StartPoolAync() == false)
                {
                    return EResult.Error_Internal;
                }

                keywordCouponCode = keywordCouponCodeSelectProc.KeywordCouponCode;
                if (keywordCouponCode != null)
                {
                    if (keywordCouponCode.RemainCount <= 0)
                    {
                        return EResult.Error_TableNotFound;
                    }

                    coupon = GetCoupon(keywordCouponCode.CouponId);
                }
            }
            else if (couponCode.SerialCouponCodeValidCheck())
            {
                var serialCouponCodeSelectProc = new spSelectSerialCouponCode(couponCode);
                if (await serialCouponCodeSelectProc.StartPoolAync() == false)
                {
                    return EResult.Error_Internal;
                }

                serialCouponCode = serialCouponCodeSelectProc.SerialCouponCode;
                if (serialCouponCode != null)
                {
                    if (serialCouponCode.IsUsed)
                    {
                        //TODO: ErrorCode 추가 이미 사용된 쿠폰
                        return EResult.Error_TableNotFound;
                    }

                    if (serialCouponCode.PlayerId == 0 || serialCouponCode.PlayerId == playerId)
                    {
                        coupon = GetCoupon(serialCouponCodeSelectProc.SerialCouponCode.CouponId);
                    }
                    else
                    {
                        //TODO: ErrorCode 가 달라야할 시 추가
                    }
                }
            }

            if (coupon == null)
            {
                //TODO: ErrorCode 추가
                return EResult.Error_TableNotFound;
            }

            if (coupon.ExpireTime < DateTime.UtcNow)
            {
                //TODO: ErrorCode 추가 만료 쿠폰
                return EResult.Error_ExpireSlot;
            }

            var logSelectProc = new spCountUseCouponLog(playerId, coupon._id);
            if (await logSelectProc.StartPoolAync() == false)
            {
                return EResult.Error_Internal;
            }
            else if (logSelectProc.UseCount >= coupon.UseCountPerUser)
            {
                //TODO: ErrorCode 추가 사용 횟수 초과
                return EResult.Error_TableNotFound;
            }

            UseCouponLogDao logDao = new();
            logDao.PlayerId = playerId;
            logDao.CouponId = coupon._id;
            logDao.CouponCode = couponCode;
            logDao.LogTime = DateTime.UtcNow;

            //TODO: Mail Add And Upsert Log
            if (keywordCouponCode != null)
            {
                logDao.CouponCodeId = keywordCouponCode._id;
                var updateProc = new spUpdateUseKeywordCoupon(keywordCouponCode.Keyword, coupon.IssuedCount, logDao);
                if (await updateProc.StartPoolAync() == false)
                {
                    //TODO: 해당 프로시져는 다르게 처리해야하는가?
                    //bool값이 아닌 result를 반환?
                    return EResult.Error_Internal;
                }
            }
            else
            {
                logDao.CouponCodeId = serialCouponCode._id;
                var updateProc = new spUpdateUseSerialCoupon(serialCouponCode.SerialCode, logDao);
                if (await updateProc.StartPoolAync() == false)
                {
                    //TODO: 해당 프로시져는 다르게 처리해야하는가?
                    return EResult.Error_Internal;
                }
            }

            return EResult.Success;
        }

        public async Task IssueSerialCouponAsync(CouponDao coupon, List<long> playerIdList = null)
        {
            //TODO: 삭제 가능 코드
            await RefreshAsync();

            var couponInsertProc = new spInsertCouponDao(coupon);
            await couponInsertProc.StartPoolAync();

            //TODO: 디비 혹은 테이블에 저장 필요
            //NOTE: 1millisecond 당 39개 생성 가능
            int issueCountPerLoop = 2000;
            int loopDelayMilliseconds = 100;

            int totalIssuedCount = 0;
            int remainIssuedCount = (int)coupon.IssuedCount;
            while (remainIssuedCount > 0)
            {
                int issueCount = Math.Min(issueCountPerLoop, remainIssuedCount);

                List<string> serialStringList = new(issueCount);
                long couponTick = SerialCouponCodeUtils.GetCouponTick();
                SerialCouponCode serialCode = new();

                for (int i = 0; i < issueCount; ++i)
                {
                    serialCode.SetSerialCode(SerialCouponCodeUtils.GetRandomPapperKey(), ++couponTick);
                    var serialString = serialCode.GetSerialString(SerialCouponCodeUtils.GetRandomPatternKey());
                    if (CheckKeywordCouponCode(serialString))
                    {
                        continue;
                    }

                    serialStringList.Add(serialString);
                }

                List<SerialCouponCodeDao> serialCouponCodeList = new(serialStringList.Count);
                for (int i = 0, count = serialStringList.Count; i < count; ++i)
                {
                    var serialString = serialStringList[i];
                    SerialCouponCodeDao serialCouponCode = new();
                    serialCouponCode.CouponId = coupon._id;
                    serialCouponCode.SerialCode = serialString;
                    serialCouponCode.PlayerId = playerIdList?[i] ?? 0;
                    serialCouponCodeList.Add(serialCouponCode);
                }

                var serialCouponListInsertProc = new spInsertSerialCouponList(serialCouponCodeList);
                await serialCouponListInsertProc.StartPoolAync();

                totalIssuedCount += serialCouponListInsertProc.InsertCount;
                remainIssuedCount -= serialCouponListInsertProc.InsertCount;
                await Task.Delay(loopDelayMilliseconds);
            }

            //TODO: 도중에 취소하고 싶을 경우에는 어떻게 하는지....
            //너무 오래 걸리는 경우, 잘못 발행을 입력한 경우
        }

        public async Task IssueKeywordCouponAsync(string keyword, CouponDao coupon)
        {
            if (ValidCouponString(keyword) == false)
            {
                return;
            }

            if (CheckKeywordCouponCode(keyword))
            {
                return;
            }

            KeywordCouponCodeDao keywordCouponCode = new();
            keywordCouponCode.Keyword = keyword;
            keywordCouponCode.RemainCount = coupon.IssuedCount;

            var proc = new spInsertKeywordCoupon(coupon, keywordCouponCode);
            if (await proc.StartPoolAync() == false || proc.Error != EResult.Success)
            {

            }

            m_keywordHashSet.Add(keyword);
        }

        public bool ValidCouponString(string couponCode)
        {
            return Regex.IsMatch(couponCode, m_couponStringPattern);
        }
    }
}
