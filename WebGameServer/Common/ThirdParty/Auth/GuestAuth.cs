namespace Common;

public class GuestAuth : VerityAuth
{
    public override bool Verify() => false;

    public override async Task<bool> VerityAsync()
    {
        if (IsJoin())
        {
            if (await CreateAsync())
            {
                return await base.VerityAsync();
            }
        }

        var sp = new spSelectAccount(AuthType.Guest, Id, Token);
        if (await sp.StartPoolAync() && sp.OutResult != null) 
        {
            Account = sp.OutResult;
            PlayerId = sp.OutResult.UserId;

            return await base.VerityAsync();
        }

        return false;
    }

    public bool IsJoin() => string.IsNullOrWhiteSpace(Id) && string.IsNullOrWhiteSpace(Token);

    private async Task<bool> CreateAsync()
    {
        Account = CreateAccount(AuthType.Guest, ObjectId.GenerateNewId().ToString(), Guid.NewGuid().ToString());
        Id = Account.AuthId;
        Token = Account.Token;

        var sp = new spInsertAccount(Account);
        var response = await sp.StartPoolAync();
        if (response && Account._id != ObjectId.Empty) 
        {
            PlayerId = Account.UserId;
        }

        return response;
    }
};
