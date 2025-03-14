﻿using System;

namespace BlindServerCore.Utils;

public abstract class Singleton<T> where T : new()
{
    private static readonly Lazy<T> instance = new Lazy<T>(() => new T());

    public static T Instance
    {
        get { return instance.Value; }
    }
}
