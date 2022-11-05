using System.Collections.Generic;

namespace Lekret.Ecs.Extensions
{
    public class AnyRemoveEventSystem<T> : ReactiveSystem
    {
        private readonly List<IRemoveListener<T>> _listenerBuffer = new List<IRemoveListener<T>>();
        private readonly Filter _listeners;

        public AnyRemoveEventSystem(EcsManager manager) : base(manager)
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
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var value = entity.Get<T>();
                for (var k = 0; k < listeners.Count; k++)
                {
                    listeners[k].OnRemoved(entity, value);
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