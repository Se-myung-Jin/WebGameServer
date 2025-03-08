using Microsoft.AspNetCore.Http;

namespace GameServer;

public class WebRequestParser
{
    public virtual Task<IWebPacket> ReadAsync(HttpContext context)
    {
        throw new NotImplementedException("ReadAsync NotImplementedException");
    }

    public virtual async Task<object> ReadAsync(HttpContext context, Type converType)
    {
        var reader = context.Request.BodyReader;
        var readResult = await reader.ReadAsync();

        if (readResult.IsCanceled)
        {
            throw new OperationCanceledException("ReadAsync was canceled");
        }

        return Task.FromResult("OK");
    }

    public virtual async Task WriteAsync(HttpContext context, object data)
    {
        await Task.CompletedTask;
    }
}

public class WebRequestJsonParser : WebRequestParser
{
    public override async Task<object> ReadAsync(HttpContext context, Type converType)
    {
        return await context.Request.ReadFromJsonAsync(converType);
    }

    public override async Task WriteAsync(HttpContext context, object data)
    {
        await context.Response.WriteAsJsonAsync(data);
    }
}

public class WebRequestMemoryPackParser : WebRequestParser
{
    public override async Task<IWebPacket> ReadAsync(HttpContext context)
    {
        try
        {
            return await MemoryPack.MemoryPackSerializer.DeserializeAsync<IWebPacket>(context.Request.Body);
        }
        catch (Exception ex)
        {

        }

        return default;
    }

    public override async Task<object> ReadAsync(HttpContext context, Type converType)
    {
        try
        {
            return await MemoryPack.MemoryPackSerializer.DeserializeAsync<IWebPacket>(context.Request.Body);
        }
        catch (Exception ex)
        {

        }

        return default;
    }

    public override async Task WriteAsync(HttpContext context, object data)
    {
        try
        {
            await MemoryPack.MemoryPackSerializer.SerializeAsync(context.Response.Body, data as IWebPacket);
        }
        catch (Exception ex)
        {

        }
    }
}
