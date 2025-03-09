namespace Common.Database.Dao;

[MemoryPack.MemoryPackable]
public partial class AccountRedisDao
{
    public AuthType AuthType { get; set; }
    public long LoginTime { get; set; }
    public long LogOutTIme { get; set; }
    public ushort SelectWorld { get; set; }
    public AccessToken Token { get; set; }
    public byte Grade { get; set; }
}