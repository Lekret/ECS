using System.Collections.Generic;

namespace Lekret.Ecs.Extensions
{
    public class SelfRemovedEventSystem<T> : ReactiveSystem
    {
        private readonly List<IRemovedListener<T>> _listenerBuffer = new List<IRemovedListener<T>>();

        public SelfRemovedEventSystem(EcsManager manager) : base(manager)
        {
        }

        protected override Collector GetCollector(EcsManager manager)
        {
            return manager.Collector(Mask.With<T>().Removed());
        }

        protected override bool Filter(Entity entity)
        {
            return !entity.Has<T>() && entity.Has<RemovedListeners<T>>();
        }

        protected override void Execute(List<Entity> entities)
        {
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(entity.Get<RemovedListeners<T>>().Value);
                var value = entity.Get<T>();

                for (var k = 0; k < _listenerBuffer.Count; k++)
                {
                    _listenerBuffer[k].OnRemoved(entity, value);
                }
            }
        }
    }
}