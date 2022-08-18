using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;

namespace Dythervin.ObjectPool
{
    [Serializable]
    public abstract class ObjectPoolBase<T> : IDisposable, IObjectPool<T>
        where T : class
    {
        public const int DefaultMaxSize = 1000;
        public const int DefaultCapacity = 10;
        public const bool DefaultCollectionCheck = true;

        [SerializeField] protected int maxSize = DefaultMaxSize;
        [SerializeField] protected bool collectionCheckDefault = DefaultCollectionCheck;
        protected Stack<T> Stack { get; private set; }

        public event Action<T> OnCreated;
        public event Action<T> OnDestroy;
        public event Action<T> OnGet;
        public event Action<T> OnRelease;
        public int CountActive => CountAll - CountInactive;
        public int CountAll { get; private set; }
        public int CountInactive => Stack.Count;

        public int MaxSize
        {
            get => maxSize;
            set
            {
                if (maxSize <= 0)
                    throw new ArgumentException("Max Size must be greater than 0", nameof(maxSize));
                maxSize = value;
            }
        }

        public void Dispose()
        {
            Clear();
        }

        public virtual void Clear()
        {
            if (OnDestroy != null)
                foreach (T obj in Stack)
                    OnDestroy(obj);

            Stack.Clear();
            CountAll = 0;
        }

        public void EnsureObjCount(int count)
        {
            if (count <= 0 || count > maxSize)
                throw new ArgumentOutOfRangeException();

            while (Stack.Count < count)
                Release(GetNew());
        }

        public virtual T Get()
        {
            T obj = Stack.Count == 0 ? GetNew() : Stack.Pop();
            OnGot(obj);
            return obj;
        }

        protected void OnGot(T obj)
        {
            var onGet = OnGet;
            onGet?.Invoke(obj);
        }


        protected void SetStack(int capacity)
        {
            Assert.IsNull(Stack);
            Stack = new Stack<T>(capacity);
        }

        protected ObjectPoolBase([DefaultValue(DefaultCollectionCheck)] bool collectionCheckDefault,
            Action<T> onGet = null,
            Action<T> onRelease = null,
            Action<T> actionOnDestroy = null,
            int defaultCapacity = DefaultCapacity,
            int maxSize = DefaultMaxSize)
        {
            SetStack(defaultCapacity);
            MaxSize = maxSize;
            OnGet = onGet;
            OnRelease = onRelease;
            OnDestroy = actionOnDestroy;
            this.collectionCheckDefault = collectionCheckDefault;
        }

        protected ObjectPoolBase() { }

        protected void OnDestroyInvoke(T element)
        {
            var actionOnDestroy = OnDestroy;
            actionOnDestroy?.Invoke(element);
        }

        protected void OnReleaseInvoke(T element)
        {
            var actionOnRelease = OnRelease;
            actionOnRelease?.Invoke(element);
        }

        protected T GetNew()
        {
            T obj = CreateNew();
            ++CountAll;
            var onCreated = OnCreated;
            onCreated?.Invoke(obj);
            return obj;
        }

        protected abstract T CreateNew();

        // ReSharper disable once UnusedParameter.Global
        public void Release(T element, bool collectionCheck)
        {
#if UNITY_EDITOR
            //Always check in editor
            if (Stack.Count > 0 && Stack.Contains(element))
#else
            if (collectionCheck && Stack.Count > 0 && Stack.Contains(element))
#endif
                throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");

            OnReleaseInvoke(element);

            if (CountInactive < maxSize)
                Stack.Push(element);
            else
                OnDestroyInvoke(element);

            OnReleased(element);
        }

        public void Release(T element)
        {
            Release(element, collectionCheckDefault);
        }

        protected virtual void OnReleased(T element) { }
    }
}