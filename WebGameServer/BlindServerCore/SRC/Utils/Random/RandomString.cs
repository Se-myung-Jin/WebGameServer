using System.Security.Cryptography;
using System.Text;
using System;

namespace BlindServerCore.Utils;

public class RandomString
{
    readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    public string GetUniqueKey(int size)
    {
        byte[] data = new byte[4 * size];
        using (var crypto = new RNGCryptoServiceProvider())
        {
            crypto.GetBytes(data);
        }
        StringBuilder result = new StringBuilder(size);
        for (int i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % _chars.Length;

            result.Append(_chars[idx]);
        }

        return result.ToString();
    }

    public string GetUniqueKeyOriginal_BIASED(int size)
    {
        char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        byte[] data = new byte[size];

        using (var crypto = new RNGCryptoServiceProvider())
        {
            crypto.GetBytes(data);
        }

        StringBuilder result = new StringBuilder(size);
        foreach (byte b in data)
        {
            result.Append(chars[b % (chars.Length)]);
        }
        return result.ToString();
    }
}
