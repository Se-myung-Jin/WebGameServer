namespace Common;

public static class SerialCouponCodeUtils
{
    public static readonly Dictionary<char, byte> CouponToBitMap = new()
        {
            { '0', 0 }, { '1', 1 }, { '2', 2 }, { '3', 3 }, { '4', 4 }, { '5', 5 }, { '6', 6 }, { '7', 7 }, { '8', 8 }, { '9', 9 },
            { 'A', 10 }, { 'B', 11 }, { 'C', 12 }, { 'D', 13 }, { 'E', 14 }, { 'F', 15 }, { 'G', 16 }, { 'H', 17 }, { 'I', 18 }, { 'J', 19 },
            { 'K', 20 }, { 'L', 21 }, { 'M', 22 }, { 'N', 23 }, { 'O', 24 }, { 'P', 25 }, { 'Q', 26 }, { 'R', 27 }, { 'S', 28 }, { 'T', 29 },
            { 'U', 30 }, { 'V', 31 }, { 'W', 32 }, { 'X', 33 }, { 'Y', 34 }, { 'Z', 35 }
        };

    public static readonly Dictionary<byte, char> BitToCouponMap = new()
        {
            { 0, '0' }, { 1, '1' }, { 2, '2' }, { 3, '3' }, { 4, '4' }, { 5, '5' }, { 6, '6' }, { 7, '7' }, { 8, '8' }, { 9, '9' },
            { 10, 'A' }, { 11, 'B' }, { 12, 'C' }, { 13, 'D' }, { 14, 'E' }, { 15, 'F' }, { 16, 'G' }, { 17, 'H' }, { 18, 'I' }, { 19, 'J' },
            { 20, 'K' }, { 21, 'L' }, { 22, 'M' }, { 23, 'N' }, { 24, 'O' }, { 25, 'P' }, { 26, 'Q' }, { 27, 'R' }, { 28, 'S' }, { 29, 'T' },
            { 30, 'U' }, { 31, 'V' }, { 32, 'W' }, { 33, 'X' }, { 34, 'Y' }, { 35, 'Z' }
        };

    public static readonly int KeywordLength = 12;

    //TODO: Live 전에 검토 필요 0~11, PapperKey가 4가 아닌 경우 Pattern 4 -> 8개 가능
    public static readonly int VerifyKeyIndex = 0;
    public static readonly List<byte[]> ReadPatternList = new()
        {
            { [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11] },
            { [11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1] },
            { [1, 11, 2, 10, 3, 9, 4, 8, 5, 7, 6] },
            { [6, 7, 5, 8, 4, 9, 3, 10, 2, 11, 1] },
        };

    public static readonly long FLAG_1BIT = 0b0001_0000;
    public static readonly long FLAG_4BIT = 0b0000_1111;

    public static readonly byte FLAG_PAPPER_KEY = 0b0011_1000;
    public static readonly byte FLAG_PATTERN_KEY = 0b0000_0111;

    public static byte GetRandomPapperKey() => (byte)Random.Shared.Next(0, 5);
    public static byte GetRandomPatternKey() => (byte)Random.Shared.Next(0, 4);
    public static long GetCouponTick() => DateTime.UtcNow.AddYears(-2000).Ticks >> 8;

    public static byte ConvertCouponBit(this char ch)
    {
        if (CouponToBitMap.TryGetValue(ch, out var coupon) == false)
        {
            coupon = byte.MaxValue;
        }

        return coupon;
    }

    public static char ConvertCouponChar(this byte bit)
    {
        if (BitToCouponMap.TryGetValue(bit, out var coupon) == false)
        {
            coupon = '0';
        }

        return coupon;
    }

    public static byte Get1Bit(this byte bit) => (byte)((bit & FLAG_1BIT) >> 4);
    public static byte Get4Bit(this byte bit) => (byte)(bit & FLAG_4BIT);
    public static byte GetPapperKey(this byte bit) => (byte)((bit & FLAG_PAPPER_KEY) >> 3);
    public static byte GetPatternKey(this byte bit) => (byte)(bit & FLAG_PATTERN_KEY);

    public static long GetTimeTickFullBit(this byte bit, int index) => (long)(((bit & FLAG_1BIT) >> 4 << index) + ((bit & FLAG_4BIT) << 8 + index * 4));
    public static long GetTimeTick4Bit(this byte bit, int index) => (long)((bit & FLAG_4BIT) << index);

    public static long GetTimeTick(this byte[] binary)
    {
        long timeTick = 0;
        byte papperKey = binary[0].GetPapperKey();
        for (int i = 0; i < 8; ++i)
        {
            timeTick += ((byte)(binary[11 - i] - papperKey)).GetTimeTickFullBit(i);
        }
        for (int i = 0; i < 3; ++i)
        {
            timeTick += ((byte)(binary[3 - i] - papperKey)).GetTimeTick4Bit(40 + 4 * i);
        }
        return timeTick << 8;
    }

    //public static byte SetVerifyKey(byte papperKey, byte patternKey) => (byte)(papperKey << 3 + patternKey);
    public static void SetSerialCode(this byte[] binary, byte papperKey, long timeTick)
    {
        binary[0] = (byte)(papperKey << 3);

        for (int i = 1; i <= 3; ++i)
        {
            int shift = 52 - i * 4;
            binary[i] = (byte)((timeTick & (FLAG_4BIT << shift)) >> shift);
            if ((papperKey & (0b0000_0001 << i - 1)) > 0)
            {
                binary[i] += (byte)FLAG_1BIT;
            }
        }

        for (int i = 4; i <= 11; ++i)
        {
            int shift = 52 - i * 4;
            binary[i] = (byte)((timeTick & (FLAG_4BIT << shift)) >> shift);
            if ((timeTick & (FLAG_1BIT >> 4 << 11 - i)) > 0)
            {
                binary[i] += (byte)FLAG_1BIT;
            }
        }

        for (int i = 1; i < 12; ++i)
        {
            binary[i] += papperKey;
        }
    }

    public static bool SerialCouponCodeValidCheck(this string serialString)
    {
        if (string.IsNullOrEmpty(serialString))
        {
            return false;
        }

        if (serialString.Length != KeywordLength)
        {
            return false;
        }

        SerialCouponCode serialCouponCode = new SerialCouponCode(serialString);
        return serialCouponCode.IsValid;
    }
}