using System.Collections.Generic;
using ECS.Runtime.Core;
using ECS.Runtime.Extensions.Listeners;
using ECS.Runtime.Extensions.Listeners.Global;
using ECS.Runtime.Extensions.Listeners.Self;
using ECS.Runtime.Utils;

namespace ECS.Runtime.Extensions
{
    public static class EntityEventExtensions
    {
        public static Entity SubscribeChanged<T>(this Entity entity, ComponentChanged<T> callback)
        {
            if (entity.Has<ComponentChangedListeners<T>>())
            {
                entity.Get<ComponentChangedListeners<T>>().Value.Add(callback);
            }
            else
            {
                var listeners = Pool<List<ComponentChanged<T>>>.Spawn();
                listeners.Add(callback);
                entity.Set(new ComponentChangedListeners<T> {Value = listeners});
            }

            return entity;
        }

        public static Entity UnsubscribeChanged<T>(this Entity entity, ComponentChanged<T> callback)
        {
            if (entity.Has<ComponentChangedListeners<T>>())
            {
                var listeners = entity.Get<ComponentChangedListeners<T>>().Value;
                listeners.Remove(callback);
                if (listeners.Count == 0)
                {
                    entity.Remove<ComponentChangedListeners<T>>();
                    Pool<List<ComponentChanged<T>>>.Release(listeners);
                }
            }
            
            return entity;
        }
        
        public static Entity SubscribeChangedGlobal<T>(this Entity entity, ComponentChanged<T> callback)
        {
            if (entity.Has<ComponentChangedGlobalListeners<T>>())
            {
                entity.Get<ComponentChangedGlobalListeners<T>>().Value.Add(callback);
            }
            else
            {
                var listeners = Pool<List<ComponentChanged<T>>>.Spawn();
                listeners.Add(callback);
                entity.Set(new ComponentChangedGlobalListeners<T> {Value = listeners});
            }

            return entity;
        }
        
        public static Entity UnsubscribeChangedGlobal<T>(this Entity entity, ComponentChanged<T> callback)
        {
            if (entity.Has<ComponentChangedGlobalListeners<T>>())
            {
                entity.Get<ComponentChangedGlobalListeners<T>>().Value.Add(callback);
            }
            else
            {
                var listeners = Pool<List<ComponentChanged<T>>>.Spawn();
                listeners.Add(callback);
                entity.Set(new ComponentChangedGlobalListeners<T> {Value = listeners});
            }

            return entity;
        }
    }
}