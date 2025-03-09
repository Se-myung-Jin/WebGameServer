namespace Common.Content;

//TODO: SerialCouponCodeUtils에 합치는것 고려
public struct SerialCouponCode
{
    public bool IsValid { get; set; } = true;
    public byte[] BinaryData { get; private set; } = new byte[SerialCouponCodeUtils.KeywordLength];

    public SerialCouponCode(string serialString)
    {
        Convert(serialString);
    }

    public SerialCouponCode()
    {
    }

    public void Convert(string serialString)
    {
        if (string.IsNullOrEmpty(serialString))
        {
            IsValid = false;
            return;
        }

        if (serialString.Length != BinaryData.Length)
        {
            IsValid = false;
            return;
        }

        BinaryData[0] = serialString[SerialCouponCodeUtils.VerifyKeyIndex].ConvertCouponBit();

        var PapperKey = BinaryData[0].GetPapperKey();
        var PatternKey = BinaryData[0].GetPatternKey();

        if (PapperKey > 4)
        {
            IsValid = false;
            return;
        }

        if (PatternKey > 3)
        {
            IsValid = false;
            return;
        }

        var readPattern = SerialCouponCodeUtils.ReadPatternList[PatternKey];
        for (int i = 0, count = readPattern.Length; i < count; ++i)
        {
            var index = readPattern[i];
            BinaryData[index] = (byte)(serialString[i + 1].ConvertCouponBit());
            if (BinaryData[index] > 31 + PapperKey || BinaryData[index] < PapperKey)
            {
                IsValid = false;
                return;
            }
        }

        IsValid = CheckValid();
    }

    public bool CheckValid()
    {
        return ((BinaryData[1].Get1Bit() << 0) + (BinaryData[2].Get1Bit() << 1) + (BinaryData[3].Get1Bit() << 2)) == BinaryData[0].GetPapperKey() && BinaryData.GetTimeTick() < DateTime.UtcNow.AddYears(-2000).Ticks;
    }

    public void SetSerialCode(byte papperKey, long timeTick)
    {
        BinaryData.SetSerialCode(papperKey, timeTick);
    }

    public string GetSerialString(byte patternKey)
    {
        char[] chArr = new char[BinaryData.Length];
        BinaryData[0] += patternKey;
        chArr[0] = BinaryData[0].ConvertCouponChar();

        var readPattern = SerialCouponCodeUtils.ReadPatternList[patternKey];
        for (int i = 0, count = readPattern.Length; i < count; ++i)
        {
            chArr[i + 1] = BinaryData[readPattern[i]].ConvertCouponChar();
        }

        return new string(chArr);
    }
}