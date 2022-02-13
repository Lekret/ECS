using System;
using System.Collections;
using System.Collections.Generic;
using EcsLib.Internal;

namespace EcsLib.Api
{
    public sealed class EcsFilter : IEnumerable<Entity>
    {
        private readonly HashSet<Entity> _entities = new HashSet<Entity>();
        private readonly List<int> _withIndices;
        private readonly List<int> _withoutIndices;

        internal EcsFilter(List<int> withIndices, List<int> withoutIndices)
        {
            _withIndices = withIndices;
            _withoutIndices = withoutIndices;
        }

        public event Action<Entity> EntityAdded;
        public event Action<Entity> EntityRemoved;
        public int Count => _entities.Count;

        public static EcsFilterBuilder Create(EcsManager manager = null)
        {
            if (manager == null)
                manager = EcsManager.Instance;
            return manager.Filter();
        }
        
        public List<Entity> GetEntities(List<Entity> buffer)
        {
            buffer.AddRange(_entities);
            return buffer;
        }

        public EcsCollector ToCollector()
        {
            var collector = new EcsCollector();
            EntityAdded += collector.AddEntity;
            EntityRemoved += collector.RemoveEntity;
            return collector;
        }

        public MutableEntityEnumerator GetEnumerator()
        {
            return MutableEntityEnumerator.Create(_entities);
        }
        
        IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator() => GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal void HandleEntity(Entity entity)
        {
            if (CanAdd(entity))
            {
                AddEntity(entity);
            }
            else
            {
                RemoveEntity(entity);
            }
        }

        private bool CanAdd(Entity entity)
        {
            for (var i = 0; i < _withoutIndices.Count; i++)
            {
                if (entity.HasComponent(_withoutIndices[i]))
                    return false;
            }

            for (var i = 0; i < _withIndices.Count; i++)
            {
                if (!entity.HasComponent(_withIndices[i]))
                    return false;
            }

            return true;
        }
        
        private void AddEntity(Entity entity)
        {
            if (_entities.Add(entity))
            {
                EntityAdded?.Invoke(entity);
            }
        }
        
        internal void RemoveEntity(Entity entity)
        {
            if (_entities.Remove(entity))
            {
                EntityRemoved?.Invoke(entity);
            }
        }
    }
}