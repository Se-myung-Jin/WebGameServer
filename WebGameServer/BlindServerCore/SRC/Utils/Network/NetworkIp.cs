using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlindServerCore.Utils;

public class NetworkIp
{
    public static bool IsNetworkAvailable => System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

    public static string GetPrivateAddress()
    {
        var networkInterfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
        foreach (var networkInterface in networkInterfaces)
        {
            if (networkInterface.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211 &&
                networkInterface.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Ethernet)
            {
                continue;
            }

            if (networkInterface.Name.StartsWith("bridge", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            foreach (var ip in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    continue;
                }

                return ip.Address.MapToIPv4().ToString();
            }
        }

        return null;
    }

    public static async Task<string> GetPublicIpAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            var externalIp = await httpClient.GetStringAsync("http://ipinfo.io/ip");
            if (string.IsNullOrWhiteSpace(externalIp) == false)
            {
                externalIp = externalIp.Trim();
            }

            return externalIp;
        }
        catch
        {

        }

        return default;
    }

    public static async Task<string> GetPublicIPv4AddressAsync()
    {
        var urlContent = await GetUrlContentAsStringAsync("http://ipv4.icanhazip.com/").ConfigureAwait(false);
        
        return ParseSingleIPv4Address(urlContent).ToString();
    }

    public static async Task<string> GetUrlContentAsStringAsync(string url)
    {
        var urlContent = string.Empty;
        
        try
        {
            using var httpClient = new HttpClient();
            using var httpResonse = await httpClient.GetAsync(url).ConfigureAwait(false);

            urlContent = await httpResonse.Content.ReadAsStringAsync().ConfigureAwait(false);         
        }
        catch
        {
            
        }

        return urlContent;
    }

    public static IPAddress ParseSingleIPv4Address(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Input string must not be null", input);
        }

        var addressBytesSplit = input.Trim().Split('.').ToList();
        if (addressBytesSplit.Count != 4)
        {
            throw new ArgumentException("Input string was not in valid IPV4 format \"xxx.xxx.xxx.xxx\"", input);
        }

        var addressBytes = new byte[4];
        foreach (var i in Enumerable.Range(0, addressBytesSplit.Count))
        {
            if (int.TryParse(addressBytesSplit[i], out var parsedInt) == false)
            {
                throw new FormatException($"Unable to parse integer from {addressBytesSplit[i]}");
            }

            if (0 > parsedInt || parsedInt > 255)
            {
                throw new ArgumentOutOfRangeException($"{parsedInt} not within required IP address range [0,255]");
            }

            addressBytes[i] = (byte)parsedInt;
        }

        return new IPAddress(addressBytes);
    }

    public static async Task<string> GetListenIp(bool usePublicIp = true)
    {
        string listenIp = GetPrivateAddress();
        if (usePublicIp)
        {
            listenIp = await GetPublicIpAsync();
            listenIp = listenIp ?? await GetPublicIPv4AddressAsync();
        }

        return listenIp;
    }

    public static string GetRemoteIp(IPEndPoint endPoint)
    {
        if (endPoint.Address.IsIPv4MappedToIPv6)
        {
            return endPoint.Address.MapToIPv4().ToString();
        }

        return endPoint.Address.ToString();
    }

    public static string GetRemoteIp(IPAddress address)
    {
        if (address.IsIPv4MappedToIPv6)
        {
            return address.MapToIPv4().ToString();
        }

        return address.ToString();
    }

    public static string GetRemoteIp(Microsoft.AspNetCore.Http.HttpContext context)
    {
        string ip;

        var forward = context.Request.Headers["X-Forwarded-For"];
        if (forward.Count == 0)
        {
            ip = context.Connection.RemoteIpAddress.ToString();
        }
        else
        {
            ip = forward[0];
        }

        return GetRemoteIp(System.Net.IPAddress.Parse(ip));
    }

    public static byte[] GetRemoteIpByteArray(Microsoft.AspNetCore.Http.HttpContext context)
    {
        string ip;

        var forward = context.Request.Headers["X-Forwarded-For"];
        if (forward.Count == 0)
        {
            ip = context.Connection.RemoteIpAddress.ToString();
        }
        else
        {
            ip = forward[0];
        }

        var address = IPAddress.Parse(ip);

        if (address.IsIPv4MappedToIPv6)
        {
            return address.MapToIPv4().GetAddressBytes();
        }

        return address.GetAddressBytes();
    }
}