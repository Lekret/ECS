using System.Collections.Generic;
using SimpleEcs.Runtime;

namespace Lekret.Ecs.Extensions
{
    public class RemoveSelfEventSystem<T> : ReactiveSystem
    {
        private readonly List<IRemoveListener<T>> _listenerBuffer = new List<IRemoveListener<T>>();

        public RemoveSelfEventSystem(EcsManager manager) : base(manager)
        {
        }

        protected override Collector GetCollector(EcsManager manager)
        {
            return manager.Collector(Mask.With<T>().Removed());
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
                    listener.OnRemoved(entity, value);
                }
            }
        }
    }
}