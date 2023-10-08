using System.Collections.Generic;
using ECS.Runtime.Access;
using ECS.Runtime.Access.Collector;
using ECS.Runtime.Core;

namespace ECS.Runtime.Extensions.Listeners.Global
{
    public class ComponentChangedGlobalReactiveSystem<T> : ReactiveSystem
    {
        private readonly List<ComponentChanged<T>> _listenerBuffer = new();
        private readonly Filter _listeners;

        public ComponentChangedGlobalReactiveSystem(World world) : base(world)
        {
            _listeners = world.Filter(Mask.With<ComponentChangedGlobalListeners<T>>());
        }

        protected override Collector GetCollector(World world)
        {
            return world.Collector(Mask.With<T>().SetOrRemoved());
        }

        protected override bool Filter(Entity entity)
        {
            return entity.IsAlive();
        }

        protected override void Execute(List<Entity> entities)
        {
            var listeners = GetListeners();
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                for (var k = 0; k < listeners.Count; k++)
                {
                    listeners[k](entity);
                }
            }
        }

        private List<ComponentChanged<T>> GetListeners()
        {
            _listenerBuffer.Clear();
            foreach (var entity in _listeners)
            {
                _listenerBuffer.AddRange(entity.Get<ComponentChangedGlobalListeners<T>>().Value);
            }

            return _listenerBuffer;
        }
    }
}