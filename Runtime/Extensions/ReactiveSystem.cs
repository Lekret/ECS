﻿using System.Collections.Generic;
using ECS.Runtime.Access.Collector;
using ECS.Runtime.Core;
using ECS.Runtime.Systems;
using ECS.Runtime.Utils;

namespace ECS.Runtime.Extensions
{
    public abstract class ReactiveSystem : IUpdateSystem
    {
        private readonly Collector _collector;

        protected ReactiveSystem(World world)
        {
            _collector = GetCollector(world);
        }

        protected abstract Collector GetCollector(World world);

        protected abstract bool Filter(Entity entity);

        protected abstract void Execute(List<Entity> entities);

        public void Update()
        {
            if (_collector.Count == 0)
                return;

            var buffer = SpawnBuffer();

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
                    ReleaseBuffer(buffer);
                }
            }
            else
            {
                ReleaseBuffer(buffer);
            }
        }

        private List<Entity> SpawnBuffer()
        {
            return Pool<List<Entity>>.Spawn();
        }

        private void ReleaseBuffer(List<Entity> buffer)
        {
            Pool<List<Entity>>.Release(buffer);
        }
    }
}