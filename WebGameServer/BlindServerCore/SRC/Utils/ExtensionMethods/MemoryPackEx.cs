using System;
using System.IO;
using System.Threading.Tasks;
namespace BlindServerCore;

public static partial class GlobalExtentionMethods
{
    public static byte[] WriteMemoryPack<T>(this T data)
    {
        try
        {
            return MemoryPack.MemoryPackSerializer.Serialize(data);
        }
        catch (Exception ex)
        {

        }
        return default;
    }

    public static async Task WriteMemoryPackAsync<T>(this T data, Stream stream)
    {
        try
        {
            await MemoryPack.MemoryPackSerializer.SerializeAsync(stream, data);
        }
        catch (Exception ex)
        {

        }
    }

    public static T ReadMemoryPack<T>(this byte[] data)
    {
        if (data == null || data.Length <= 0)
        {
            return default;
        }

        try
        {
            return MemoryPack.MemoryPackSerializer.Deserialize<T>(data);
        }
        catch (Exception ex)
        {

        }
        return default;
    }

    public static Nullable<T> ReadMemoryPackNullAble<T>(this byte[] data) where T : struct
    {
        if (data == null || data.Length <= 0)
        {
            return null;
        }

        try
        {
            return MemoryPack.MemoryPackSerializer.Deserialize<T>(data);
        }
        catch (Exception ex)
        {

        }
        return null;
    }

    public static async Task<T> ReadMemoryPackAsync<T>(this byte[] data)
    {
        if (data == null || data.Length <= 0)
        {
            return default(T);
        }

        try
        {
            //await using var stream = SystemGlobal.Instance.RecycleMemory.GetStream(data);

            //return await MemoryPack.MemoryPackSerializer.DeserializeAsync<T>(stream);
        }
        catch (Exception ex)
        {

        }
        return default(T);
    }

    public static async Task<T> ReadMemoryPackAsync<T>(this Stream stream)
    {
        if (stream == null || stream.Length <= 0)
        {
            return default(T);
        }

        try
        {
            return await MemoryPack.MemoryPackSerializer.DeserializeAsync<T>(stream);
        }
        catch (Exception ex)
        {

        }
        return default(T);
    }
}
