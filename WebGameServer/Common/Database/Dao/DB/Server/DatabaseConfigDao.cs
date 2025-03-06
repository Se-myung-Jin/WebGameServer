namespace Common;

[Table("Servers", "databaseConfig")]
public class DatabaseConfigDao : BaseDao
{
    public List<ConfigDatabase> DatabaseLIst { get; set; }
    public List<ConfigRedis> RedisList { get; set; }
    public ConfigLogDatabase LogDatabase { get; set; }
}
