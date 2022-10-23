using System.Collections.Generic;
using SimpleEcs.Runtime;

namespace Lekret.Ecs.Extensions
{
    public class SetAllEventSystem<T> : ReactiveSystem
    {
        private readonly List<ISetListener<T>> _listenerBuffer = new List<ISetListener<T>>();
        private readonly Filter _listeners;

        public SetAllEventSystem(EcsManager manager) : base(manager)
        {
            _listeners = manager.Filter(Mask.With<SetListeners<T>>());
        }

        protected override Collector GetCollector(EcsManager manager)
        {
            return manager.Collector(Mask.With<T>().Set());
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
                    listener.OnSet(entity, value);
                }
            }
        }

        private List<ISetListener<T>> GetListeners()
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