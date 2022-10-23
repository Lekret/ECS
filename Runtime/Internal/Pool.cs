using System;
using System.Collections.Generic;

namespace Lekret.Ecs.Internal
{
    internal class Pool<T> where T : new()
    {
        [ThreadStatic]
        private static Queue<T> PoolThreadStatic;

        private Pool() { }
        
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
            if (PoolThreadStatic == null)
            {
                PoolThreadStatic = new Queue<T>();
            }
            return PoolThreadStatic;
        }
    }
}