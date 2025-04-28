namespace Common.Database.Dao;

public enum PubSubCommandProtocol
{
    None = 0,
    DuplicateLogin,
    TableRefresh,
};

[MemoryPack.MemoryPackable]
[MemoryPack.MemoryPackUnion(1, typeof(COMMAND_DUPLICATE_LOGIN))]
[MemoryPack.MemoryPackUnion(2, typeof(COMMAND_TABLE_REFRESH))]
public abstract partial class SUBSCRIPTION_COMMAND
{
    public PubSubCommandProtocol Protocol { get; set; }
    public long CreateTimeTick { get; set; }

    [MemoryPack.MemoryPackConstructor]
    public SUBSCRIPTION_COMMAND(PubSubCommandProtocol protocol)
    {
        Protocol = protocol;
    }
};

[MemoryPack.MemoryPackable]
public partial class COMMAND_DUPLICATE_LOGIN : SUBSCRIPTION_COMMAND
{
    public long PlayerId { get; set; }

    public COMMAND_DUPLICATE_LOGIN()
        : base (PubSubCommandProtocol.DuplicateLogin)
    {
    }
        
}

[MemoryPack.MemoryPackable]
public partial class COMMAND_TABLE_REFRESH : SUBSCRIPTION_COMMAND
{
    public List<string> RefreshList { get; set; }

    [MemoryPack.MemoryPackConstructor]
    public COMMAND_TABLE_REFRESH()
        : base(PubSubCommandProtocol.TableRefresh)
    { }
}