using System.Collections.Generic;
using ECS.Runtime.Access;
using ECS.Runtime.Access.Collector;
using ECS.Runtime.Core;

namespace ECS.Runtime.Extensions.RemoveListeners
{
    public class AnyRemovedEventSystem<T> : ReactiveSystem
    {
        private readonly List<ComponentRemoved<T>> _listenerBuffer = new List<ComponentRemoved<T>>();
        private readonly Filter _listeners;

        public AnyRemovedEventSystem(World world) : base(world)
        {
            _listeners = world.Filter(Mask.With<RemovedListeners<T>>());
        }

        protected override Collector GetCollector(World world)
        {
            return world.Collector(Mask.With<T>().Removed());
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
                    listeners[k](entity);
                }
            }
        }

        private List<ComponentRemoved<T>> GetListeners()
        {
            _listenerBuffer.Clear();
            foreach (var entity in _listeners)
            {
                _listenerBuffer.AddRange(entity.Get<RemovedListeners<T>>().Value);
            }

            return _listenerBuffer;
        }
    }
}