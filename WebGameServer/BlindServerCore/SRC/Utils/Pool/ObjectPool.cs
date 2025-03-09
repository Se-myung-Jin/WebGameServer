using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BlindServerCore.Utils;

public class ObjectPool<T> where T : new()
{
    private Queue<T> _queue = new Queue<T>(100);
    private bool _autoIncrease = true;

    public ObjectPool(int defaultSize = 100, bool autoIncrease = true)
    {
        if (defaultSize > 0)
        {
            Create(defaultSize);
        }

        _autoIncrease = autoIncrease;
    }


    protected void Create(int size)
    {
        T[] array = new T[size];
        for (int i = 0; i < size; ++i)
        {
            array[i] = new T();
            _queue.Enqueue(array[i]);
        }
    }

    public T Acuire()
    {
        if (_queue.TryDequeue(out var data) == false)
        {
            if (_autoIncrease)
            {
                Create(100);
            }
            _queue.TryDequeue(out data);
        }
        return data;
    }

    public void Release(T source)
    {
        _queue.Enqueue(source);
    }
}

public class ConcurrencyObjectPool<T> where T : class, new()
{
    public int PoolSize => _queue.Count;

    private int _limitSize = 1000;
    private ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
    private Type _createType;

    public ConcurrencyObjectPool()
    {
    }

    public void Initialize(int size, int limitSize = 1000)
    {
        _limitSize = limitSize;
        Create(size);
    }

    public void Initialize(int size, Type type, int limitSize = 1000)
    {
        _limitSize = limitSize;
        _createType = type;
        Create(size);
    }

    protected void Create(int size)
    {
        var dataArray = _createType == null ? new T[size] : Array.CreateInstance(_createType, size) as T[];
        for (int i = 0; i < size; ++i)
        {
            dataArray[i] = _createType == null ? new T() : Activator.CreateInstance(_createType) as T;
            _queue.Enqueue(dataArray[i]);
        }
    }

    public T Acuire(bool create = true)
    {
        if (_queue.TryDequeue(out var data) == false)
        {
            if (create)
            {
                data = _createType == null ? new T() : Activator.CreateInstance(_createType) as T;
            }
        }
        return data;
    }

    public void Release(T source)
    {
        if (_queue.Count >= _limitSize)
        {
            return;
        }

        _queue.Enqueue(source);
    }
}
