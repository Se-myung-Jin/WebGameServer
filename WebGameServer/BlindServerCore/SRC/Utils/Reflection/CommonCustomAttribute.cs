using System;
using System.Collections.Generic;
using System.Reflection;

namespace BlindServerCore.Utils;

public static partial class CommonCustomAttribute
{
    public static List<ValueTuple<MethodInfo, Attribute>> FindMethodAndAttributes(Type findType, BindingFlags flag)
    {
        List<ValueTuple<MethodInfo, Attribute>> resultList = new();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            foreach (var typeClass in assembly.GetTypes())
            {
                if (typeClass.IsClass == false)
                {
                    continue;
                }

                foreach (var method in typeClass.GetMethods(flag))
                {
                    var findAttribute = method.GetCustomAttribute(findType, true);
                    if (findAttribute != null && findAttribute.GetType() == findType)
                    {
                        resultList.Add(new ValueTuple<MethodInfo, Attribute>(method, findAttribute));
                    }
                }
            }
        }

        return resultList;
    }

    public static List<string> FindClassAttributeWithNameList(Type findAttribute)
    {
        List<string> resultList = new();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            foreach (var typeClass in assembly.GetTypes())
            {
                if (typeClass.IsClass == false)
                {
                    continue;
                }

                var findResults = typeClass.GetCustomAttributes(findAttribute, true);
                if (findResults?.Length > 0)
                {
                    resultList.Add(typeClass.Name);
                }
            }
        }

        return resultList;
    }

    public static List<ValueTuple<Type, object[]>> FindClassAttribute(Type findAttr, bool inherit = true)
    {
        List<ValueTuple<Type, object[]>> resultList = new();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            foreach (var typeClass in assembly.GetTypes())
            {
                if (typeClass.IsClass == false)
                {
                    continue;
                }

                var findResults = typeClass.GetCustomAttributes(findAttr, inherit);
                if (findResults?.Length > 0)
                {
                    resultList.Add(new ValueTuple<Type, object[]>(typeClass, findResults));
                }
            }
        }

        return resultList;
    }
}
