namespace Common;

public enum EServerType
{
    Undefine = 0,
    Maintenance,
    Game,
    LogAggregation,
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

public enum EMongoDbKind
{
    Main,
};

public enum ERedisKeyType
{
    Auth,
};

public enum EPublisher
{
    UnDefine = 0,
    Katkit,
};

public enum EAuthType
{
    UnDefine = 0,
    Guest,
    Google,
    Apple,
    FaceBook,
    Max,
};


public enum EOsType
{
    UnDefine = 0,
    Windows,
    Google,
    Apple,
    Max,
};

public enum EMaintenanceUpdateType
{
    UnDefine = 0,
    Force,
    Recommand,
    InvalidVersion,
    Max,
};

public enum ETermsType
{
    Undefine = 0,
    TermOfUse,
    PrivacyPolicy,
};