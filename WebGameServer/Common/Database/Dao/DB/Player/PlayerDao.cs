using Common;

[Table("Worlds", "player")]
[TableIndex("UserId")]
[TableIndex("PlayerId")]
[MemoryPack.MemoryPackable]
public partial class PlayerDao : BaseDao
{

}