using System.Collections.Generic;
using Lekret.Ecs.Internal;

namespace Lekret.Ecs
{
    public abstract class ReactiveSystem : IUpdateSystem
    {
        private static readonly Pool<List<Entity>> BufferPool = Pool<List<Entity>>.Instance;
        private readonly Collector _collector;

        protected ReactiveSystem(EcsManager manager)
        {
            _collector = GetCollector(manager);
        }

        protected abstract Collector GetCollector(EcsManager manager);

        protected abstract bool Filter(Entity entity);

        protected abstract void Execute(List<Entity> entities);

        public void Update()
        {
            if (_collector.Count == 0)
                return;

            var buffer = BufferPool.Spawn();
            
            foreach (var e in _collector)
            {
                if (Filter(e))
                {
                    buffer.Add(e);
                }
            }

            _collector.Clear();

            if (buffer.Count > 0)
            {
                try
                {
                    Execute(buffer);
                }
                finally
                {
                    buffer.Clear();
                    BufferPool.Release(buffer);
                }
            }
            else
            {
                BufferPool.Release(buffer);
            }
        }
    }
}