using System;

namespace Dythervin.ObjectPool
{
    public class ObjectPool<T> : ObjectPoolBase<T>
        where T : class
    {
        private readonly Func<T> _createFunc;

        public ObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null, Action<T> actionOnDestroy = null,
            bool collectionCheckDefault = DefaultCollectionCheck,
            int defaultCapacity = DefaultCapacity, int maxSize = DefaultMaxSize) : base(collectionCheckDefault, onGet: onGet, onRelease: onRelease,
            actionOnDestroy: actionOnDestroy,
            defaultCapacity: defaultCapacity, maxSize: maxSize)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        }

        protected override T CreateNew()
        {
            return _createFunc();
        }
    }
}