global using MongoDB.Bson;
global using MongoDB.Driver;
global using DBContext = Common.DatabaseContextContainer;

namespace Common;

public class TableDao
{
    public string Name { get; set; }
    public byte[] Data { get; set; }
}
