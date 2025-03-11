using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace BlindServerCore.Utils;

public static partial class CommonUtils
{
    private static ThreadLocal<Well512Random> m_tlsRandom = new ThreadLocal<Well512Random>(() => new Well512Random((uint)DateTime.UtcNow.Ticks));
    private static ThreadLocal<RandomString> m_tlsRandomString = new ThreadLocal<RandomString>(() => new RandomString());
    private static ThreadLocal<StringBuilder> m_tlsStringBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder());
    
    /// <summary>
    /// 사용불가능한 언어가 포함되어 있는지 체크 
    /// availableSpecialCharacter 포함되어 있는 특수문자는 포함될수 있음
    /// </summary>
    public static bool IsUsableNameString(string text, int min = 2, int max = 12, string availableSpecialCharacter = @"_\[\]()&@!#%")
    {
        if (string.IsNullOrWhiteSpace(text) || min <= 0 || max <= 0 || min > max || min == max)
        {
            return false;
        }

        if (CheckSpecialCharacter(text) == true)
        {
            return false;
        }

        var pattern = GetStringBuilder();
        if (string.IsNullOrWhiteSpace(availableSpecialCharacter) == false)
        {
            pattern.AppendFormat(@"^[\p{{L}}\p{{Nd}}{0}]{{{1},{2}}}$", availableSpecialCharacter, min, max);
        }
        else
        {
            pattern.AppendFormat(@"^[\p{{L}}\p{{Nd}}]{{{0},{1}}}$", min, max);
        }

        return System.Text.RegularExpressions.Regex.IsMatch(text, pattern.ToString());
    }


    public static bool CheckTextKorean(string text, byte min, byte max)
    {
        var pattern = GetStringBuilder();
        pattern.AppendFormat(@"^[가-힣]{{{0},{1}}}$", min, max);

        return System.Text.RegularExpressions.Regex.IsMatch(text, pattern.ToString());
    }

    public static bool CheckTextEnglishAndNumbers(string text, byte min, byte max)
    {
        var pattern = GetStringBuilder();
        pattern.AppendFormat(@"^[0-9a-zA-Z]{{{0},{1}}}$", min, max);

        return System.Text.RegularExpressions.Regex.IsMatch(text, pattern.ToString());
    }

    public static Well512Random GetThreadSafeRandom()
    {
        return m_tlsRandom.Value;
    }

    public static RandomString GetThreadSafeRandomString()
    {
        return m_tlsRandomString.Value;
    }

    public static uint GetRandom(int min, int max)
    {
        return GetThreadSafeRandom().Next(min, max);
    }

    public static uint GetRandom(uint maxValue)
    {
        return GetThreadSafeRandom().Next(maxValue);
    }

    public static float GetRandomFloat(this Random random, float min, float max)
    {
        return random.NextSingle() * (max - min) + min;
    }

    public static string RandomString(int size)
    {
        return GetThreadSafeRandomString().GetUniqueKey(size);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        Random rng = Random.Shared;

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static long IPAddressToInt64(System.Net.EndPoint remote)
    {
        System.Net.IPEndPoint remoteIpEndPoint = remote as System.Net.IPEndPoint;

        return System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt32(remoteIpEndPoint.Address.GetAddressBytes(), 0));
    }

    public static long IPAddressToInt64(string ip)
    {	
        return System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt32(System.Net.IPAddress.Parse(ip).GetAddressBytes(), 0));
    }

    public static string IPAddressFromInt64(long ip)
    {
        return System.Net.IPAddress.Parse(ip.ToString()).ToString();
    }

    // throws NoSuchAlgorithmException, InvalidKeyException
    public static byte[] encodeHMAC(String key, string message, string alg = "SHA1")
    {
        return encodeHMAC(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(message), alg);
    }

    // throws NoSuchAlgorithmException, InvalidKeyException
    public static byte[] encodeHMAC(String key, byte[] message, string alg = "SHA1")
    {
        return encodeHMAC(Encoding.UTF8.GetBytes(key), message, alg);
    }

    // throws NoSuchAlgorithmException, InvalidKeyException
    public static byte[] encodeHMAC(byte[] keyBytes, byte[] textBytes, string alg = "SHA1")
    {
        KeyedHashAlgorithm hash;
        switch (alg)
        {
            case "MD5":
                hash = new HMACMD5(keyBytes);
                break;
            case "SHA256":
                hash = new HMACSHA256(keyBytes);
                break;
            case "SHA384":
                hash = new HMACSHA384(keyBytes);
                break;
            case "SHA512":
                hash = new HMACSHA512(keyBytes);
                break;
            case "SHA1":
            default:
                hash = new HMACSHA1(keyBytes);
                break;
        }

        Byte[] hashBytes = hash.ComputeHash(textBytes);

        return hashBytes;
    }

    public static String bytesToHex(byte[] inData)
    {
        StringBuilder builder = new StringBuilder();
        foreach (byte b in inData)
        {
            builder.Append(b.ToString("X2"));
        }

        return builder.ToString().ToLower();
    }

    public static string MD5Hash(string text)
    {
        var hashArray = MD5.HashData(Encoding.UTF8.GetBytes(text).AsSpan());

        return bytesToHex(hashArray);
    }

    private static StringBuilder GetStringBuilder()
    {
        var builder = m_tlsStringBuilder.Value;
        builder.Clear();

        return builder;
    }

    private static bool CheckSpecialCharacter(string text)
    {
        if (text.Contains(" ") || text.Contains("/") || text.Contains(@"\") || text.Contains("\r\n") || text.Contains("\n") || text.Contains("\r") || text.Contains("\t") || text.Contains(Environment.NewLine))
        {
            return true;
        }

        return false;
    }
}
