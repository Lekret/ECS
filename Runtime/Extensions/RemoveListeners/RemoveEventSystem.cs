using System.Collections.Generic;

namespace Lekret.Ecs.Extensions
{
    public class RemoveEventSystem<T> : ReactiveSystem
    {
        private readonly List<IRemoveListener<T>> _listenerBuffer = new List<IRemoveListener<T>>();

        public RemoveEventSystem(EcsManager manager) : base(manager)
        {
        }

        protected override Collector GetCollector(EcsManager manager)
        {
            return manager.Exc<T>().ToCollector();
        }

        protected override bool Filter(Entity entity)
        {
            return !entity.Has<T>() && entity.Has<RemoveListeners<T>>();
        }

        protected override void Execute(List<Entity> entities)
        {
            foreach (var entity in entities)
            {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(entity.Get<RemoveListeners<T>>().Value);
                var value = entity.Get<T>();
                
                foreach (var listener in _listenerBuffer)
                {
                    listener.OnRemove(entity, value);
                }
            }
        }
    }
}