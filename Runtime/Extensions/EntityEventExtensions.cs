using System.Collections.Generic;

namespace Lekret.Ecs.Extensions
{
    public static class EntityEventExtensions
    {        
        public static void ListenSet<T>(this Entity entity, ISetListener<T> listener)
        {
            if (entity.Has<SetListeners<T>>())
            {
                entity.Get<SetListeners<T>>().Value.Add(listener);
            }
            else
            {
                var listeners = Pool<List<ISetListener<T>>>.Spawn();
                listeners.Add(listener);
                entity.Set(new SetListeners<T> {Value = listeners});
            }
        }
        
        public static void UnlistenSet<T>(this Entity entity, ISetListener<T> listener)
        {
            if (entity.Has<SetListeners<T>>())
            {
                var listeners = entity.Get<SetListeners<T>>().Value;
                listeners.Remove(listener);
                if (listeners.Count == 0)
                {
                    entity.Remove<SetListeners<T>>();
                    Pool<List<ISetListener<T>>>.Release(listeners);
                }
            }
        }
        
        public static void ListenRemoved<T>(this Entity entity, IRemovedListener<T> listener)
        {
            if (entity.Has<RemovedListeners<T>>())
            {
                entity.Get<RemovedListeners<T>>().Value.Add(listener);
            }
            else
            {
                var listeners = Pool<List<IRemovedListener<T>>>.Spawn();
                listeners.Add(listener);
                entity.Set(new RemovedListeners<T> {Value = listeners});
            }
        }
        
        public static void UnlistenRemoved<T>(this Entity entity, IRemovedListener<T> listener)
        {
            if (entity.Has<RemovedListeners<T>>())
            {
                var listeners = entity.Get<RemovedListeners<T>>().Value;
                listeners.Remove(listener);
                if (listeners.Count == 0)
                {
                    entity.Remove<RemovedListeners<T>>();
                    Pool<List<IRemovedListener<T>>>.Release(listeners);
                }
            }
        }
    }
}