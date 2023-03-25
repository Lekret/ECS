using System;
using System.Collections.Generic;

namespace Lekret.Ecs
{
    internal static class Pool<T> where T : new()
    {
        [ThreadStatic] private static Queue<T> Queue;

        internal static T Spawn()
        {
            var pool = GetPool();
            if (pool.Count > 0)
                return pool.Dequeue();
            return new T();
        }

        internal static void Release(T item)
        {
            var pool = GetPool();
            pool.Enqueue(item);
        }

        private static Queue<T> GetPool()
        {
            if (Queue == null)
            {
                Queue = new Queue<T>();
            }

            return Queue;
        }
    }
}