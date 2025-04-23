namespace Common.Database.Dao;

[Table("Servers", "databaseConfig")]
public class DatabaseConfigDao : BaseDao
{
    public List<ConfigDatabase> DatabaseLIst { get; set; }
    public List<ConfigRedis> RedisList { get; set; }
    public List<ConfigLogDatabase> LogDatabaseList { get; set; }
}
