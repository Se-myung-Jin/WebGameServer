namespace Common;

public class ProxyCommand : Singleton<ProxyCommand>
{
    public readonly ProxyCommandRedis Redis = new ProxyCommandRedis();
}