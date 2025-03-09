namespace BlindServerCore;

public enum ServerType
{
    Undefine = 0,
    Maintenance,
    Game,
    LogAggregation,
};

public enum DataBaseCategory
{
    MSSQL,
    MYSQL,
    MONGO,
    REDIS,
    MAX,
};

public enum ReplicaType
{
    MASTER,
    SLAVE,
    MAX,
};

public enum MongoDbKind
{
    Main,
};

public enum RedisKeyType
{
    Auth,
};

public enum MySqlKind
{
    Write = 1,
    ReadOnly = 2,
};

public enum Publisher
{
    UnDefine = 0,
    Katkit,
};

public enum AuthType
{
    UnDefine = 0,
    Guest,
    Google,
    Apple,
    FaceBook,
    Max,
};


public enum OsType
{
    UnDefine = 0,
    Windows,
    Google,
    Apple,
    Max,
};

public enum MaintenanceUpdateType
{
    UnDefine = 0,
    Force,
    Recommand,
    InvalidVersion,
    Max,
};

public enum TermsType
{
    Undefine = 0,
    TermOfUse,
    PrivacyPolicy,
};