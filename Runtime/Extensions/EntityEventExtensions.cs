using System.Collections.Generic;
using ECS.Runtime.Core;
using ECS.Runtime.Extensions.RemoveListeners;
using ECS.Runtime.Extensions.SetListeners;
using ECS.Runtime.Utils;

namespace ECS.Runtime.Extensions
{
    public static class EntityEventExtensions
    {
        public static Entity SubscribeSet<T>(this Entity entity, ComponentSet<T> callback)
        {
            if (entity.Has<SetListeners<T>>())
            {
                entity.Get<SetListeners<T>>().Value.Add(callback);
            }
            else
            {
                var listeners = Pool<List<ComponentSet<T>>>.Spawn();
                listeners.Add(callback);
                entity.Set(new SetListeners<T> {Value = listeners});
            }

            return entity;
        }

        public static Entity UnsubscribeSet<T>(this Entity entity, ComponentSet<T> callback)
        {
            if (entity.Has<SetListeners<T>>())
            {
                var listeners = entity.Get<SetListeners<T>>().Value;
                listeners.Remove(callback);
                if (listeners.Count == 0)
                {
                    entity.Remove<SetListeners<T>>();
                    Pool<List<ComponentSet<T>>>.Release(listeners);
                }
            }
            
            return entity;
        }

        public static Entity SubscribeRemoved<T>(this Entity entity, ComponentRemoved<T> callback)
        {
            if (entity.Has<RemovedListeners<T>>())
            {
                entity.Get<RemovedListeners<T>>().Value.Add(callback);
            }
            else
            {
                var listeners = Pool<List<ComponentRemoved<T>>>.Spawn();
                listeners.Add(callback);
                entity.Set(new RemovedListeners<T> {Value = listeners});
            }
            
            return entity;
        }

        public static Entity UnsubscribeRemoved<T>(this Entity entity, ComponentRemoved<T> callback)
        {
            if (entity.Has<RemovedListeners<T>>())
            {
                var listeners = entity.Get<RemovedListeners<T>>().Value;
                listeners.Remove(callback);
                if (listeners.Count == 0)
                {
                    entity.Remove<RemovedListeners<T>>();
                    Pool<List<ComponentRemoved<T>>>.Release(listeners);
                }
            }
            
            return entity;
        }
    }
}