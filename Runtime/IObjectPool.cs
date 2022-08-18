using System;

namespace Dythervin.ObjectPool
{
    public interface IObjectPool
    {
        int CountInactive { get; }
        void Clear();
    }

    public interface IObjectPoolOut<out T> : IObjectPool
        where T : class
    {
        event Action<T> OnCreated;
        event Action<T> OnGet;
        event Action<T> OnRelease;
        event Action<T> OnDestroy;

        T Get();
    }

    public interface IObjectPool<T> : IObjectPoolOut<T>
        where T : class
    {
        void Release(T obj);
    }
}