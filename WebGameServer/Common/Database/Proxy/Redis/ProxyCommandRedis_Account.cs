using StackExchange.Redis;

namespace Common;

public partial class ProxyCommandRedis
{
    public async Task<AccountRedisDao> GetAccountAsync(long playerId) => await RedisCommand.StringGetAsync<AccountRedisDao>(CreateParamAccount(playerId));

    public async Task<AccountRedisDao> SaveAccountAsync(AccountDao account, AccessToken token)
    {
        var redisData = new AccountRedisDao
        {
            AuthType = (AuthType)account.AccountType,
            Token = token,
            SelectWorld = account.LastSelectWorld,
            LoginTime = DateTime.UtcNow.Ticks,
            LogOutTIme = 0,
            Grade = account.Grade,
        };

        var param = CreateParamAccount(token.UserId, redisData.WriteMemoryPack(), 60000);

        // 로그인 캐시 저장.
        await RedisCommand.StringSetAsync(param);

        return redisData;
    }

    public async Task<(Result result, AccountRedisDao account)> ValidateKeyAccountAsync(AccessToken token)
    {
        var account = await GetAccountAsync(token.UserId);
        if (account == null)
        {
            return (Result.Error_CacheAccountNotFound, null);
        }
        
        if (account.Token.Key != token.Key)
        {
            return (Result.Error_NotFoundAuthKey, null);
        }

        return (Result.Success, account);
    }

    public async Task<AccountRedisDao> UpdateAccountSelectWorldAsync(long playerId, ushort selectWorld)
    {

        var accountRedis = await GetAccountAsync(playerId);
        if (accountRedis == null)
        {
            return null;
        }

        accountRedis.SelectWorld = selectWorld;
        await RedisCommand.StringSetAsync(CreateParamAccount(playerId, accountRedis.WriteMemoryPack(), 60000));

        // 이곳에서 DB 저장은 하지 않음. DB 저장은 게임 종료할때 저장

        return accountRedis;
    }

    private RedisStringParameter CreateParamAccount(long playerId) => CreateParamAccount(playerId, RedisValue.Null, 0);
    private RedisStringParameter CreateParamAccount(long playerId, RedisValue value, uint expireTimeSecond)
    {
        return new RedisStringParameter
        {
            Kind = GlobalValue.GLOBAL_REDIS,
            Key = RedisKeyBuilder.CreateKey(RedisKeyType.Auth, playerId),
            Value = value,
            ExpireSecond = expireTimeSecond,
        };
    }
}