using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BlindServerCore.Utils;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlindServerCore.Database;

public class TableAttribute : Attribute
{
    public readonly string DbName;
    public readonly string TableName;
    public readonly MongoDbKind Kind;
    public readonly bool EnableShading;

    public TableAttribute(string dbName, string tableName, MongoDbKind dbKind = MongoDbKind.Main, bool enableShading = false)
    {
        DbName = dbName;
        TableName = tableName;
        Kind = dbKind;
        EnableShading = enableShading;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class TableIndexAttribute : Attribute
{
    public readonly string Key1;
    public readonly string Key2;
    public readonly string Key3;
    public readonly bool Unique = false;

    public TableIndexAttribute(string key1, bool unique = false)
    {
        Key1 = key1;
        Unique = unique;
    }

    public TableIndexAttribute(string key1, string key2, bool unique = false)
    {
        Key1 = key1;
        Key2 = key2;
        Unique = unique;
    }

    public TableIndexAttribute(string key1, string key2, string key3, bool unique = false)
    {
        Key1 = key1;
        Key2 = key2;
        Key3 = key3;
        Unique = unique;
    }
}


public class MongoDbContextContainer
{
    private Dictionary<MongoDbKind, MongoClient> _connectionMap = new();
    private Dictionary<Type, TableAttribute> _typeToNameMap = new();

    public void Initialize()
    {
        CreateIndex();
    }

    public MongoClient Add(DBConfig config)
    {
        Enum.TryParse(config.Name, out MongoDbKind typeName);
        var client = new MongoClient(config.ConnectionString);
        
        _connectionMap.Add(typeName, client);

        return client;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MongoClient Client(MongoDbKind name) => _connectionMap[name];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMongoDatabase Database(MongoDbKind name, string dbName) => _connectionMap[name].GetDatabase(dbName);

    public IMongoCollection<T> GetTable<T>(MongoDbKind name, string dbName, string table) => _connectionMap[name].GetDatabase(dbName).GetCollection<T>(table);

    public IMongoCollection<T> GetCollection<T>()
    {
        var find = GetTypeToName<T>();

        return Client(find.dbKind).GetDatabase(find.dbName).GetCollection<T>(find.tableName);
    }

    private (string dbName, string tableName, MongoDbKind dbKind, bool enableShading) GetTypeToName<T>()
    {
        return GetTypeToName(typeof(T));
    }

    private (string dbName, string tableName, MongoDbKind dbKind, bool enableShading) GetTypeToName(Type type)
    {
        if (_typeToNameMap.TryGetValue(type, out var tblAttribute) == false)
        {
            var attributes = type.GetCustomAttributes(true);
            for (int i = 0; i < attributes.Length; ++i)
            {
                if (attributes[i].GetType() == typeof(TableAttribute))
                {
                    var attr = attributes[i] as TableAttribute;
                    _typeToNameMap.TryAdd(type, attr);

                    return (attr.DbName, attr.TableName, attr.Kind, attr.EnableShading);
                }
            }
        }

        return (tblAttribute.DbName, tblAttribute.TableName, tblAttribute.Kind, tblAttribute.EnableShading);
    }

    public void Destory()
    {
        foreach (var connection in _connectionMap.Values)
        {
            connection.Dispose();
        }

        _connectionMap.Clear();
    }

    private void CreateIndex()
    {
        var indexList = CommonCustomAttribute.FindClassAttribute(typeof(TableIndexAttribute), false);
        if (indexList?.Count > 0)
        {
            for (int i = 0; i < indexList.Count; ++i)
            {
                var index = indexList[i];
                var findResult = GetTypeToName(index.Item1);
                var client = Client(findResult.dbKind);
                var collection = Client(findResult.dbKind).GetDatabase(findResult.dbName).GetCollection<BsonDocument>(findResult.tableName);

                if (findResult.enableShading == true)
                {
                    var adminDb = client.GetDatabase("admin");
                    var configDb = client.GetDatabase("config");
                    var database = collection.Database;

                    var databaseName = database.DatabaseNamespace.DatabaseName;
                    var collectionName = collection.CollectionNamespace.CollectionName;
                    /*
                    try
                    {
                        //BsonDocument parameter = adminDb.RunCommand<BsonDocument>(BsonDocument.Parse("{ getParameter: 1, featureCompatibilityVersion: 1 }"));
                        var shardDbResult = adminDb.RunCommand<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument( "enableSharding", $"{databaseName}"));
                        var shardScript = $"{{shardCollection: \"{databaseName}.{collectionName}\"}}";
                        var commandDict = new Dictionary<string, object>() { { "shardCollection", $"{databaseName}.{collectionName}" }, { "key", new Dictionary<string, object>() { { "_id", "hashed" } } } };
                        var commandDoc = new BsonDocumentCommand<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument(commandDict));
                        var response = adminDb.RunCommand(commandDoc);
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Log.Error(ex);
                    }
                    */
                }

                List<CreateIndexModel<BsonDocument>> createList = new();
                for (int n = 0; n < index.Item2.Length; ++n)
                {
                    var indexAttr = index.Item2[n] as TableIndexAttribute;

                    var builder = new IndexKeysDefinitionBuilder<BsonDocument>();
                    IndexKeysDefinition<BsonDocument> key = builder.Ascending(indexAttr.Key1);

                    if (string.IsNullOrWhiteSpace(indexAttr.Key2) == false)
                    {
                        key = key.Ascending(indexAttr.Key2);
                    }

                    if (string.IsNullOrWhiteSpace(indexAttr.Key3) == false)
                    {
                        key = key.Ascending(indexAttr.Key3);
                    }

                    createList.Add(new CreateIndexModel<BsonDocument>(key, new CreateIndexOptions { Unique = indexAttr.Unique }));
                }

                try
                {
                    var result = collection.Indexes.CreateMany(createList);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}