using System.IdentityModel.Tokens.Jwt;

namespace Common;

public abstract class VerityAuth
{
    private static IdGenerator _idGenerator = new(1);

    public long PlayerId { get; protected set; }
    /// <summary>
    /// 내부적으로 로그인시마다 발급되는 일회용Key
    /// </summary>
    public string AccessKey { get; protected set; }
    public bool Created { get; protected set; } = false;
    public string Id { get; protected set; }
    /// <summary>
    /// 패스워드 또는 소셜에서 발급되는 토큰
    /// </summary>
    public string Token { get; protected set; }
    public AccountDao Account { get; protected set; }

    public void initialize(string id, string token)
    {
        Id = id;
        Token = token;
    }

    public virtual bool Verify()
    {
        CreateAccessKey();

        return true;
    }

    public virtual async Task<bool> VerityAsync()
    {
        CreateAccessKey();

        return await Task.FromResult(true);
    }

    public AccountDao CreateAccount(EAuthType type, string authId, string token)
    {
        Created = true;
        var nowTime = DateTime.UtcNow;
        return new AccountDao
        {
            AccountType = (byte)type,
            AuthId = authId,
            Token = token,
            CreateTime = nowTime,
            LastLoginTime = nowTime,
            UserId = _idGenerator.Create(),
        };
    }

    protected virtual string CreateAccessKey()
    {
        AccessKey = DateTime.UtcNow.Ticks.ToString();
        
        return AccessKey;
    }
}

public class GoogleAuth : VerityAuth
{
    public override bool Verify()
    {
        //var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJiaWxsaW5nc2VydmljZWFjY291bnRAYXBpLTQ2NDE1NTM5ODcyMzczOTQyMDUtNTUxMzU5LmlhbS5nc2VydmljZWFjY291bnQuY29tIiwiaWF0IjoxNjQyNDc0Nzg3LjAyOCwiZXhwIjoxNjQyNDc4Mzg3LjAyOCwic2NvcGUiOiJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9hdXRoL2FuZHJvaWRwdWJsaXNoZXIiLCJhdWQiOiJodHRwczovL29hdXRoMi5nb29nbGVhcGlzLmNvbS90b2tlbiJ9.vagBHLgdfzyaDO0mutpVZWqzXcrK4K0M7o6s6jQNFB5m50mcvJyLTI9E6RcoHv14ujGodP-NvKSdsg4-mASBV6Ay65bjsQta4GNrpVRAp8Xse8Rsl3Z8j7EtDOtFvdEs9TjHvj_LPThtr0FlFE3yr-513cJmyeAd53kxDOdKeHguuTOrvtp58IrYoAKyuJURY20ijg7ibZD07r-lcDK2cx9yejHs-TYGcvEjAxL7PECEP4JbXaA2NmTKoiNClXfm533Qq-8JqOHUPINx6iKtIkkLmZmzHDvArWZ5WyuvnJuQz83Eyk-iwgY7H_4_6zuv1IU-VG69vaitipCPfMh53g";
        var tokenHandle = new JwtSecurityTokenHandler();
        var readToken = tokenHandle.ReadJwtToken(Token);

        return readToken.ValidTo.Ticks >= DateTime.UtcNow.Ticks;
    }

    public override async Task<bool> VerityAsync() => await Task.FromResult(false);
};

public class AppleAuth : VerityAuth
{
    public override bool Verify() => false;

    public override async Task<bool> VerityAsync() => await Task.FromResult(false);
};

public class FacebookAuth : VerityAuth
{
    public override bool Verify() => false;

    public override async Task<bool> VerityAsync() => await Task.FromResult(false);
};