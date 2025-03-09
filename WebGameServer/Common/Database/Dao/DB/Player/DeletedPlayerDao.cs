namespace Common.Database.Dao;

[Table("Worlds", "worldDeletedPlayer")]
public partial class DeletedPlayerDao : BaseDao
{
    public long PlayerId { get; set; }
}
