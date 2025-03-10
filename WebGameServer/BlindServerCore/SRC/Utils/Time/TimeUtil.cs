using System;

namespace BlindServerCore.Utils
{
    public static class TimeUtils
    {
        public static readonly int SECOND_DAY = 86400;
        public static readonly int SECOND_WEEK = 604800;

        public static DateTime ToDateTime(this long source) => new DateTime(source, DateTimeKind.Utc);
        public static DateTime CreateTime(int year, int month, int day) => new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime CreateTime(int year, int month, int day, int hour, int min, int second) => new DateTime(year, month, day, hour, min, second, DateTimeKind.Utc);

        /// <summary>
        /// 하루의 시작 시간을 구한다 (Global Time 기준 n시)
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartTimeOnDay(int hour)
        {
            DateTime current = GetTime();
            DateTime resDT = CreateTime(current.Year, current.Month, current.Day, hour, 0, 1);
            if (current < resDT)
            {
                resDT = resDT.AddDays(-1);
            }

            return resDT;
        }

        public static long GetStartTimeOnDayToTick(int hour) => GetStartTimeOnDay(hour).Ticks;
        public static long GetStartTimeOnDayToSecond(int hour) => TimeToSecond(GetStartTimeOnDay(hour));
        public static DateTime GetStartTimeOnNextDay(int hour) => GetStartTimeOnDay(hour).AddDays(1);
        public static long GetStartTimeOnNextDayToTick(int hour) => GetStartTimeOnNextDay(hour).Ticks;
        public static long GetStartTimeOnNextDayToSecond(int hour) => TimeToSecond(GetStartTimeOnNextDay(hour));

        /// <summary>
        /// 주의 시작 시간을 구한다 (Global Time 기준 월요일 n시)
        /// </summary>
        /// <returns></returns>

        public static DateTime GetStartTimeOnWeek(int hour, DayOfWeek week = DayOfWeek.Monday)
        {
            DateTime current = GetTime();
            DateTime monday = current.AddDays(-((int)current.DayOfWeek - (int)week));

            DateTime resDT = CreateTime(monday.Year, monday.Month, monday.Day, hour, 0, 1);

            if (current < resDT)
            {
                resDT = resDT.AddDays(-7);
            }

            return resDT;
        }

        public static long GetStartTimeOnWeekToTick(int hour) => GetStartTimeOnWeek(hour).Ticks;
        public static long GetStartTimeOnWeekToSecond(int hour) => TimeToSecond(GetStartTimeOnWeek(hour));
        public static DateTime GetStartTimeOnNextWeek(int hour) => GetStartTimeOnWeek(hour).AddDays(7);
        public static long GetStartTimeOnNextWeekToTick(int hour) => GetStartTimeOnNextWeek(hour).Ticks;
        public static long GetStartTimeOnNextWeekToSecond(int hour) => TimeToSecond(GetStartTimeOnNextWeek(hour));

        /// <summary>
        /// 달의 시작 시간을 구한다 (Global Time 기준 1일 n시)
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartTimeOnMonth(int hour)
        {
            DateTime current = GetTime();
            DateTime resDT = CreateTime(current.Year, current.Month, 1, hour, 0, 1);

            if (current < resDT)
            {
                resDT = resDT.AddMonths(-1);
            }
            return resDT;
        }

        public static long GetStartTimeOnMonthToTick(int hour) => GetStartTimeOnMonth(hour).Ticks;
        public static long GetStartTimeOnMonthToSecond(int hour) => TimeToSecond(GetStartTimeOnMonth(hour));
        public static DateTime GetStartTimeOnNextMonth(int hour) => GetStartTimeOnMonth(hour).AddMonths(1);

        public static long GetStartTimeOnNextMonthToTick(int hour) => GetStartTimeOnNextMonth(hour).Ticks;
        public static long GetStartTimeOnNextMonthToSecond(int hour) => TimeToSecond(GetStartTimeOnNextMonth(hour));

        public static DateTime GetTime() => DateTime.UtcNow;
        public static long GetTimeTick() => DateTime.UtcNow.Ticks;
        public static long GetTimeTickAddMilliSecond(int milliSecond) => GetTime().AddMilliseconds(milliSecond).Ticks;
        public static long GetTimeTickAddMilliSecond(uint milliSecond) => GetTime().AddMilliseconds(milliSecond).Ticks;
        public static long GetTimeTickAddMilliSecond(long milliSecond) => GetTime().AddMilliseconds(milliSecond).Ticks;
        public static long GetTimeTickAddSecond(int second) => GetTime().AddSeconds(second).Ticks;

        public static string TimeToString()
        {
            return GetTime().ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static long TimeToDay()
        {
            return GetTime().Ticks / TimeSpan.TicksPerDay;
        }

        public static long TimeToMilliSecond()
        {
            return GetTime().Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static long TimeToSecond()
        {
            return GetTime().Ticks / TimeSpan.TicksPerSecond;
        }

        public static long TimeToSecond(DateTime time)
        {
            return time.Ticks / TimeSpan.TicksPerSecond;
        }

        public static uint ToUnixTimeSecond()
        {
            return ToUnixTimeSecond(GetTime());
        }

        public static uint ToUnixTimeSecond(this DateTime time)
        {
            return (uint)(time - GUnixTimeStamp).TotalSeconds;
        }

        public static uint ToSpecialUnixTimeSecond(this DateTime epochTime)
        {
            var diffTIme = GetTime() - epochTime;
            return (uint)diffTIme.TotalSeconds;
        }

        public static DateTime FromUnixSecondTime(this int unixTime)
        {
            return GUnixTimeStamp + TimeSpan.FromSeconds(unixTime);
        }

        /// <summary>
        /// 연-월-일 시:분:초 형식으로 string type 반환. Null 넣으면 "" 반환.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ConvertTimeString(this DateTime? date)
        {
            if (date == null)
            {
                return "";
            }

            return date.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 연-월-일 시:분:초 형식으로 string type 반환.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ConvertTimeString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static bool IsBetween(this DateTime date, DateTime start, DateTime end)
        {
            if (start.Ticks > date.Ticks)
            {
                return false;
            }

            if (end.Ticks <= date.Ticks)
            {
                return false;
            }

            return true;
        }

        public static bool IsTimeOver(this DateTime date)
        {
            if (date.Ticks > GetTime().Ticks)
            {
                return false;
            }

            return true;
        }

        public static bool IsTimeOver(this long timeTick) => timeTick <= GetTimeTick();

        public static long GetAddMilliSecond(this long timeTick, int milliSecond)
        {
            return timeTick + milliSecond * TimeSpan.TicksPerMillisecond;
        }

        public static long GetAddMilliSecond(this long timeTick, long milliSecond)
        {
            return timeTick + milliSecond * TimeSpan.TicksPerMillisecond;
        }

        public static long GetAddSecond(this long timeTick, int second)
        {
            return timeTick + second * TimeSpan.TicksPerSecond;
        }

        public static long GetElapseMilliSecond(this long timeTick)
        {
            var currentTick = GetTimeTick();
            double diffTick = currentTick - timeTick;
            //return diffTick / TimeSpan.TicksPerMillisecond;
            return (long)(diffTick * 0.0001d);
        }

        public static long GetElapseSecond(this long timeTick)
        {
            var currentTick = GetTimeTick();
            double diffTick = currentTick - timeTick;
            return (long)(diffTick * 0.0000001d);
        }

        public static double GetElapseMilliSecondToDoble(this long timeTick)
        {
            var currentTick = GetTimeTick();
            double diffTick = currentTick - timeTick;
            return diffTick * 0.0001d;
        }

        public static double GetElapseSecondToDoble(this long timeTick)
        {
            var currentTick = GetTimeTick();
            double diffTick = currentTick - timeTick;
            return diffTick * 0.0000001d;
        }

        public static long GetRemainSecond(this long expireTimeTick)
        {
            long currentTick = GetTimeTick();
            var diffTimeTick = expireTimeTick - currentTick;
            if (diffTimeTick <= 0)
            {
                return 0;
            }

            //TimeSpan.TicksPerSecond
            return (long)(diffTimeTick * 0.0000001d);
        }

        public static long GetRemainMilliSecond(this long expireTimeTick)
        {
            long currentTick = GetTimeTick();
            var diffTimeTick = expireTimeTick - currentTick;
            if (diffTimeTick <= 0)
            {
                return 0;
            }

            //TimeSpan.TicksPerMillisecond
            return (long)(diffTimeTick * 0.0001d);
        }

        public static long GetRemainMilliSecondDivision(this long expireTimeTick, long divisionValue, long minCount = 0)
        {
            var remainMilliSecond = GetRemainMilliSecond(expireTimeTick);
            if (remainMilliSecond == 0)
            {
                return minCount;
            }

            var divisionCount = remainMilliSecond / divisionValue;
            var remainCount = remainMilliSecond % divisionValue > 0 ? 1 : 0;
            return divisionCount + remainCount;
        }

        public static int GetDailyKey(this DateTime source)
        {
            int key = source.Year * 100 * 100 + source.Month * 100 + source.Day;
            return key;
        }

        public static int GetWeeklyKey(this DateTime source, DayOfWeek week = DayOfWeek.Monday)
        {
            DateTime monday = source.AddDays(-((int)source.DayOfWeek - (int)week));
            DateTime resDT = CreateTime(monday.Year, monday.Month, monday.Day, 0, 0, 1);

            if (source < resDT)
            {
                resDT = resDT.AddDays(-7);
            }

            int key = resDT.Year * 100 * 10 + (int)week * 100 + resDT.DayOfYear / 7;
            return key;
        }

        public static int GetMonthlyKey(this DateTime source)
        {
            int key = source.Year * 100 + source.Month;
            return key;
        }


        public static int GetDailyKey(this long source) => GetDailyKey(source.ToDateTime());
        public static int GetWeeklyKey(this long source) => GetWeeklyKey(source.ToDateTime());
        public static int GetMonthlyKey(this long source) => GetMonthlyKey(source.ToDateTime());

        private static readonly DateTime GUnixTimeStamp = CreateTime(1970, 1, 1);
    }
}