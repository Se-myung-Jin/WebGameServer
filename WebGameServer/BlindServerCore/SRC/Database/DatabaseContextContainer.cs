using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;

namespace BlindServerCore.Database;

public struct DBConfig
{
    public readonly DataBaseCategory Category;
    public readonly ReplicaType ReplicaType;
    public readonly string ConnectionString;
    public readonly int Id;
    public readonly string Name;

    public DBConfig(string name, DataBaseCategory category, ReplicaType replicaType, string config, int id) => (Name, Category, ReplicaType, ConnectionString, Id) = (name, category, replicaType, config, id);
};

public sealed class DatabaseContextContainer
{
    private static Lazy<DatabaseContextContainer> instance = new Lazy<DatabaseContextContainer>(() => new DatabaseContextContainer());
    public static DatabaseContextContainer Instance = instance.Value;
    public MongoDbContextContainer MongoDb { get; init; } = new MongoDbContextContainer();
    public RedisContextContainer Redis { get; init; } = new RedisContextContainer();
    public MySqlContextContainer MySql { get; init; } = new MySqlContextContainer();

    private DatabaseContextContainer()
    {
        ConventionRegistry.Register("IgnoreExtraElements", new ConventionPack { new IgnoreExtraElementsConvention(true) }, type => true);
    }

    public void Create(DBConfig config)
    {
        switch (config.Category)
        {
            case DataBaseCategory.MONGO:
                MongoDb.Add(config);
                break;
            case DataBaseCategory.REDIS:
                Redis.Add(config);
                break;
            case DataBaseCategory.MYSQL:
                MySql.Add(config);
                break;
        }
    }

    public void Create(List<DBConfig> configList)
    {
        for (int i = 0; i < configList.Count; ++i)
        {
            Create(configList[i]);
        }
    }

    public void Initialize()
    {
        MongoDb.Initialize();
        Redis.Initialize();
        MySql.Initialize();
    }

    public void Destory()
    {

    }
}