using System.Collections.Generic;
using ECS.Runtime.Access;
using ECS.Runtime.Access.Collector;
using ECS.Runtime.Core;

namespace ECS.Runtime.Extensions.SetListeners
{
    public class AnySetEventSystem<T> : ReactiveSystem
    {
        private readonly List<ComponentSet<T>> _listenerBuffer = new List<ComponentSet<T>>();
        private readonly Filter _listeners;

        public AnySetEventSystem(World world) : base(world)
        {
            _listeners = world.Filter(Mask.With<SetListeners<T>>());
        }

        protected override Collector GetCollector(World world)
        {
            return world.Collector(Mask.With<T>().Set());
        }

        protected override bool Filter(Entity entity)
        {
            return entity.IsAlive() && !entity.Has<T>();
        }

        protected override void Execute(List<Entity> entities)
        {
            var listeners = GetListeners();
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var value = entity.Get<T>();
                for (var k = 0; k < listeners.Count; k++)
                {
                    listeners[k](entity, value);
                }
            }
        }

        private List<ComponentSet<T>> GetListeners()
        {
            _listenerBuffer.Clear();
            foreach (var entity in _listeners)
            {
                _listenerBuffer.AddRange(entity.Get<SetListeners<T>>().Value);
            }

            return _listenerBuffer;
        }
    }
}