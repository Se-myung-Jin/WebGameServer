namespace Common;

public enum EServerType
{
    Undefine = 0,
    Maintenance,
};

public enum EDataBaseCategory
{
    MSSQL,
    MYSQL,
    MONGO,
    REDIS,
    MAX,
};

public enum EReplicaType
{
    MASTER,
    SLAVE,
    MAX,
};