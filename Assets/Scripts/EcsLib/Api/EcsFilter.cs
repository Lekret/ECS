using System;
using System.Collections;
using System.Collections.Generic;
using EcsLib.Internal;

namespace EcsLib.Api
{
    public sealed class EcsFilter : IEnumerable<Entity>
    {
        private readonly HashSet<Entity> _entities = new HashSet<Entity>();
        private readonly int[] _included;
        private readonly int[] _excluded;

        internal EcsFilter(int[] included, int[] excluded)
        {
            _included = included;
            _excluded = excluded;
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
            for (var i = 0; i < _excluded.Length; i++)
            {
                if (entity.HasComponent(_excluded[i]))
                    return false;
            }

            for (var i = 0; i < _included.Length; i++)
            {
                if (!entity.HasComponent(_included[i]))
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

        public bool MatchesIndices(IReadOnlyCollection<int> included, IReadOnlyCollection<int> excluded)
        {
            if (included.Count != _included.Length) return false;
            if (excluded.Count != _excluded.Length) return false;
            return IndicesEquals(_included, included) && IndicesEquals(_excluded, excluded);
        }

        private static bool IndicesEquals(IReadOnlyCollection<int> left, IReadOnlyCollection<int> right)
        {
            for (var i = 0; i < left.Count; i++)
            {
                var contains = false;
                for (var k = 0; k < right.Count; k++)
                {
                    if (i == k)
                    {
                        contains = true;
                        break;
                    }
                }
                if (!contains)
                    return false;
            }
            return true;
        }
    }
}