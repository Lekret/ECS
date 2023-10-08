using System.Collections.Generic;
using ECS.Runtime.Access;
using ECS.Runtime.Access.Collector;
using ECS.Runtime.Core;
using ECS.Runtime.Utils;

namespace ECS.Runtime.Extensions.Listeners.Self
{
    public class ComponentChangedReactiveSystem<T> : ReactiveSystem
    {
        private readonly List<ComponentChanged<T>> _listenerBuffer = new();

        public ComponentChangedReactiveSystem(World world) : base(world)
        {
        }

        protected override Collector GetCollector(World world)
        {
            return world.Collector(Mask.With<T>().SetOrRemoved());
        }

        protected override bool Filter(Entity entity)
        {
            return entity.IsAlive() && entity.Has<ComponentChangedListeners<T>>();
        }

        protected override void Execute(List<Entity> entities)
        {
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(entity.Get<ComponentChangedListeners<T>>().Value);
                for (var k = 0; k < _listenerBuffer.Count; k++)
                {
                    _listenerBuffer[k](entity);
                }
            }
        }
    }
}