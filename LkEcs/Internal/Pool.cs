using System.Collections.Generic;

namespace LkEcs.Internal
{
    internal class Pool<T> where T : new()
    {
        internal static readonly Pool<T> Instance = new Pool<T>();
        private readonly Queue<T> _pool = new Queue<T>();
        
        private Pool() { }

        internal T Spawn()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();
            return new T();
        }

        internal void Release(T item)
        {
            _pool.Enqueue(item);
        }
    }
}