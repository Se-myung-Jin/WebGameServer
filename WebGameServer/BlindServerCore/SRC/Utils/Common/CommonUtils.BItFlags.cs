namespace BlindServerCore.Utils
{
    public static partial class CommonUtils
    {
        public static bool IsBitSet(this long value, int pos)
        {
            return (value & (1 << pos)) != 0;
        }

        public static long BitSet(ref this long value, int pos)
        {
            long bitValue = 1 << pos;
            return value |= bitValue;
        }

        public static long BitUnSet(ref this long value, int pos)
        {
            return value &= ~(1 << pos);
        }

        public static bool IsBitSet(this uint value, int pos)
        {
            return (value & (1 << pos)) != 0;
        }

        public static uint BitSet(ref this uint value, int pos)
        {
            uint bitValue = 1u << pos;
            return value |= bitValue;
        }

        public static uint BitUnSet(ref this uint value, int pos)
        {
            return value &= ~(1u << pos);
        }

        public static bool IsBitSet(this int value, int pos)
        {
            return (value & (1 << pos)) != 0;
        }

        public static int BitSet(ref this int value, int pos)
        {
            int bitValue = 1 << pos;
            return value |= bitValue;
        }

        public static int BitUnSet(ref this int value, int pos)
        {
            return value &= ~(1 << pos);
        }

        public static long Combine(int value1, int value2)
        {
            long shiftValue = value1;
            long shiftValue2 = value2;
            return shiftValue << 32 | shiftValue2;
        }

        public static long Combine(uint value1, uint value2)
        {
            long shiftValue = value1;
            long shiftValue2 = value2;
            return shiftValue << 32 | shiftValue2;
        }

        public static long Combine(uint value1, ushort value2)
        {
            long shiftValue = value1;
            long shiftValue2 = value2;
            return shiftValue << 32 | shiftValue2;
        }

        public static int Combine(ushort value1, ushort value2)
        {
            int shiftValue = value1;
            int shiftValue2 = value2;
            return shiftValue << 16 | shiftValue2;
        }

        public static int Combine(short value1, short value2)
        {
            int shiftValue = value1;
            int shiftValue2 = value2;
            return shiftValue << 16 | shiftValue2;
        }
    }
}
