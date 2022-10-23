using System.Collections.Generic;
using SimpleEcs.Runtime;

namespace Lekret.Ecs.Extensions
{
    public class SetSelfEventSystem<T> : ReactiveSystem
    {
        private readonly List<ISetListener<T>> _listenerBuffer = new List<ISetListener<T>>();

        public SetSelfEventSystem(EcsManager manager) : base(manager)
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
            foreach (var entity in entities)
            {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(entity.Get<SetListeners<T>>().Value);
                var value = entity.Get<T>();
                
                foreach (var listener in _listenerBuffer)
                {
                    listener.OnSet(entity, value);
                }
            }
        }
    }
}