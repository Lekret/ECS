using System.Collections.Generic;

namespace Lekret.Ecs.Internal
{
    internal class Pool<T> where T : new()
    {
        private static readonly Queue<T> _pool = new Queue<T>();

        private Pool() { }

        internal static T Spawn()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();
            return new T();
        }

        internal static void Release(T item)
        {
            _pool.Enqueue(item);
        }
    }
}