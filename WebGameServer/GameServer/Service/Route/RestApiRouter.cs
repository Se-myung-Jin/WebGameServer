using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq.Expressions;
using System.Reflection;

namespace GameServer;

public partial class RestApiRouter : Singleton<RestApiRouter>
{
    private Dictionary<string, Func<string, IWebPacket, Task<IWebPacket>>> apiCallBack;
    private WebRequestParser m_parser = new WebRequestMemoryPackParser();

    public void Initialize()
    {
        var classMethods = CommonCustomAttribute.FindMethodAndAttributes(typeof(ApiHandleAttribute), BindingFlags.Instance | BindingFlags.NonPublic);

        apiCallBack = new(classMethods.Count);

        foreach (var element in classMethods)
        {
            var method = element.Item1;
            var name = method.Name;
            try
            {
                var parameters = method.GetParameters().Select(p => Expression.Parameter(p.ParameterType.BaseType == typeof(IWebPacket) ? p.ParameterType.BaseType : p.ParameterType, p.Name)).ToArray();
                var call = Expression.Call(Expression.Constant(this), method, parameters);
                
                apiCallBack.Add(name, Expression.Lambda<Func<string, IWebPacket, Task<IWebPacket>>>(call, parameters).Compile());
            }
            catch (Exception ex)
            {
                LogSystem.Log.Error(ex);
            }
        }
    }

    public async Task Process(HttpContext context)
    {
        var requestUri = context.Request.GetEncodedPathAndQuery();
        var remoteIp = NetworkIp.GetRemoteIp(context);

        var api = GetApi(requestUri);

        IWebPacket responseObject = null;

        try
        {
            if (apiCallBack.TryGetValue(api, out var call))
            {
                var requestObject = await m_parser.ReadAsync(context);
                if (requestObject != null)
                {
                    responseObject = await call.Invoke(remoteIp, requestObject);
                }
            }
        }
        catch (Exception ex)
        {
            responseObject = new PKT_WEB_SC_INTERNALERROR() { Message = ex.Message };
        }
        finally
        {
            responseObject = responseObject ?? new PKT_WEB_SC_INTERNALERROR() { Message = "CODE-19" };

            await m_parser.WriteAsync(context, responseObject);
        }
    }

    private string GetApi(string url)
    {
        var spitUrl = url.Split('/');

        return spitUrl.LastOrDefault();
    }
}
