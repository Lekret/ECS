using System.Collections.Generic;

namespace Lekret.Ecs.Extensions
{
    public class AnySetEventSystem<T> : ReactiveSystem
    {
        private readonly List<ISetListener<T>> _listenerBuffer = new List<ISetListener<T>>();
        private readonly Filter _listeners;

        public AnySetEventSystem(EcsManager manager) : base(manager)
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
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var value = entity.Get<T>();
                for (var k = 0; k < listeners.Count; k++)
                {
                    listeners[k].OnSet(entity, value);
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