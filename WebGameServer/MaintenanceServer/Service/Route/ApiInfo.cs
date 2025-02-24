using System.Reflection;

namespace MaintenanceServer;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiHandleAttribute : Attribute
{

}

public class ApiInfo
{
    public MethodInfo ApiMethod;
    public Type ApiParameter;

    public ApiInfo(MethodInfo apiMethod, Type apiParameter)
    {
        ApiMethod = apiMethod;
        ApiParameter = apiParameter;
    }
}
