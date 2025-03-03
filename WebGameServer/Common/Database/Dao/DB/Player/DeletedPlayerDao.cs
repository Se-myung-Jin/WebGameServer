namespace Common;

[Table("Worlds", "worldDeletedPlayer")]
public partial class DeletedPlayerDao : BaseDao
{
    public long PlayerId { get; set; }
}
