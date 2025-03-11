namespace BlindServerCore.Utils
{
    public static partial class CommonUtils
    {
        public static int Percent(this int value, int max) => (int)(value * 100d / max);
        public static int Percent(this double value, double max) => (int)(value * 100d / max);
        public static bool IsScope(this int value, int min, int max) => value >= min && value <= max;
        public static bool IsScope(this long value, long min, long max) => value >= min && value <= max;
    }
}