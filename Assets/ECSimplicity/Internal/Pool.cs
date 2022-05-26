using System;
using System.Collections.Generic;

namespace ECSimplicity.Internal
{
    internal class Pool<T> where T : new()
    {
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly Action<T> _beforeRelease;

        internal Pool(Action<T> beforeRelease = null)
        {
            _beforeRelease = beforeRelease;
        }

        internal T Get()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();
            return new T();
        }

        internal void Release(T item)
        {
            _beforeRelease?.Invoke(item);
            _pool.Enqueue(item);
        }
    }
}