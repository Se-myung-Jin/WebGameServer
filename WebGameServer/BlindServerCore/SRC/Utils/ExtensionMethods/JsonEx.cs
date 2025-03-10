using System.IO;
using System;
using BlindServerCore.Log;

namespace BlindServerCore;

public static partial class GlobalExtentionMethods
{
    public static bool FileWriteJson<T>(string path, T data, out string error)
    {
        error = null;
        try
        {
            var resultJson = WriteJson(data, true);
            using (var writer = new StreamWriter(path))
            {
                writer.Write(resultJson);
            }
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
        return false;
    }

    public static T FileReadJson<T>(string path, out string error)
    {
        error = null;
        try
        {
            using (var reader = new StreamReader(path))
            {
                var readJson = reader.ReadToEnd();
                return readJson.ReadJson<T>();
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
        return default;
    }

    public static string FileReadJson(string path, out string error)
    {
        error = null;
        try
        {
            using var reader = new StreamReader(path) ;
            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
        return default;
    }

    public static string WriteJson<T>(this T data, bool indented = false)
    {
        try
        {
            var format = indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None;
            return Newtonsoft.Json.JsonConvert.SerializeObject(data, format);
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }
        return default;
    }

    public static T ReadJson<T>(this string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return default;
        }

        try
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }
        return default;
    }
}