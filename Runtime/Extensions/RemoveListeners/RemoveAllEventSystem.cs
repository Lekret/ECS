using System.Collections.Generic;
using SimpleEcs.Runtime;

namespace Lekret.Ecs.Extensions
{
    public class RemoveAllEventSystem<T> : ReactiveSystem
    {
        private readonly List<IRemoveListener<T>> _listenerBuffer = new List<IRemoveListener<T>>();
        private readonly Filter _listeners;

        public RemoveAllEventSystem(EcsManager manager) : base(manager)
        {
            _listeners = manager.Filter(Mask.With<RemoveListeners<T>>());
        }

        protected override Collector GetCollector(EcsManager manager)
        {
            return manager.Collector(Mask.With<T>().Removed());
        }

        protected override bool Filter(Entity entity)
        {
            return entity.IsAlive() && !entity.Has<T>();
        }

        protected override void Execute(List<Entity> entities)
        {
            var listeners = GetListeners();
            foreach (var entity in entities)
            {
                var value = entity.Get<T>();
                foreach (var listener in listeners)
                {
                    listener.OnRemoved(entity, value);
                }
            }
        }

        private List<IRemoveListener<T>> GetListeners()
        {
            _listenerBuffer.Clear();
            foreach (var entity in _listeners)
            {
                _listenerBuffer.AddRange(entity.Get<RemoveListeners<T>>().Value);
            }
            return _listenerBuffer;
        }
    }
}