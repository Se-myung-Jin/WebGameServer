namespace MaintenanceServer;

public partial class RestApiRouter
{

    [ApiHandleAttribute]
    private async Task<IWebPacket> Auth(string clientAddress, IWebPacket protocol)
    {
        var pkt = protocol as PKT_WEB_CS_AUTH;
        // 예약 번호 미리 발급 및 인증정보 Redis  기록
        var response = new PKT_WEB_SC_AUTH
        {
            Block = null,
            Result = Result.Error_Unknown,
        };

        if (pkt.Validate() == false)
        {
            response.Result = Result.Error_InvalidParameter;
            return response;
        }

        var auth = pkt.AuthType switch
        {
            AuthType.Guest => new GuestAuth(),
            //eAuthType.Google => new GoogleAuth(),
            //eAuthType.Apple => new AppleAuth(),
            //eAuthType.FaceBook => new FacebookAuth(),
            _ => null,
        };

        var timeTick = DateTime.UtcNow.Ticks;

        auth.initialize(pkt.AuthId, pkt.AuthToken);
        if (await auth.VerityAsync() == false)
        {
            response.Result = Result.Error_InvalidIdOrPass;
            return response;
        }

        //todo: 로그 기록 및 처리
        response.Result = Result.Success;
        response.IsCreate = auth.Created;

        if (response.IsCreate)
        {
            response.AuthId = auth.Id;
            response.AuthToken = auth.Token;
        }

        /*var checkTerms = auth.Account.CheckTerms(GameData.Instance.MAINTENANCE_Table.GetPublisherTerms(pkt.Publisher), pkt.AgreeTermsList, TimeUtils.GetTime());
        if (checkTerms.confirm == false)
        {
            response.Result = eResult.Error_FirstAgreePolicy;
            return response;
        }

        if (checkTerms.isUpdate)
        {
            spUpdateTermsList proc = new(auth.Account._id, auth.Account.AgreeTermsList);
            await proc.StartPoolAync();
        }*/

        response.Token = new AccessToken { UserId = auth.PlayerId, Key = auth.AccessKey, TimeTick = timeTick, ValidateHash = auth.AccessKey.GetHashCode() };

        //todo: 만약 기존에 캐싱 데이터가 있는 상태이고 월드 입장까지 한 상태(SelectWorld != 0)라면 기존 월드에서 팅겨 내라고 메시지를 보낸다.
        /*
        var oldCacheData = await ProxyCommand.Instance.Redis.GetAccountAsync(auth.PlayerId);
        if (oldCacheData?.SelectWorld > 0)
        {
            LogSystem.Log.Debug("Find CachedData. {$oldCacheData}", oldCacheData);
            // 원래 있던 서버 로그아웃 처리
            await Global.GetPubSub<WebPubSub>().WorldSendAsync(oldCacheData.SelectWorld, new COMMAND_DUPLICATE_LOGIN() { PlayerId = auth.PlayerId });
        }
        */
        await ProxyCommand.Instance.Redis.SaveAccountAsync(auth.Account, response.Token);

        var log = new LogAuthDao() { UserId = auth.Account.UserId, AccountType = auth.Account.AccountType, Publisher = (byte)pkt.Publisher };
        LogWriter.LogToRedisStream(log);

        return response;
    }
}
