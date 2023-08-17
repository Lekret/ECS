using System.Collections.Generic;
using ECS.Runtime.Access;
using ECS.Runtime.Access.Collector;
using ECS.Runtime.Core;

namespace ECS.Runtime.Extensions.SetListeners
{
    public class SelfSetEventSystem<T> : ReactiveSystem
    {
        private readonly List<ComponentSet<T>> _listenerBuffer = new List<ComponentSet<T>>();

        public SelfSetEventSystem(EcsManager manager) : base(manager)
        {
        }

        protected override Collector GetCollector(EcsManager manager)
        {
            return manager.Collector(Mask.With<T>().Set());
        }

        protected override bool Filter(Entity entity)
        {
            return entity.Has<T>() && entity.Has<SetListeners<T>>();
        }

        protected override void Execute(List<Entity> entities)
        {
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(entity.Get<SetListeners<T>>().Value);
                var value = entity.Get<T>();

                for (var k = 0; k < _listenerBuffer.Count; k++)
                {
                    _listenerBuffer[k](entity, value);
                }
            }
        }
    }
}