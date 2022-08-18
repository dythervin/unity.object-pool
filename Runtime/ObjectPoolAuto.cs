using System;

namespace Dythervin.ObjectPool
{
    public class ObjectPoolAuto<T> :
        ObjectPoolBase<T>
        where T : class, new()
    {
        public ObjectPoolAuto(Action<T> onGet = null, Action<T> onRelease = null, Action<T> actionOnDestroy = null,
            bool collectionCheckDefault = DefaultCollectionCheck,
            int defaultCapacity = DefaultCapacity, int maxSize = DefaultMaxSize) : base(collectionCheckDefault, onGet: onGet, onRelease: onRelease,
            actionOnDestroy: actionOnDestroy, defaultCapacity: defaultCapacity, maxSize: maxSize) { }

        protected override T CreateNew()
        {
            return new T();
        }
    }
}