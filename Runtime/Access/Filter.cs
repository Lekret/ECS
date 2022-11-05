using System;
using System.Collections;
using System.Collections.Generic;

namespace Lekret.Ecs
{
    public sealed class Filter : IEnumerable<Entity>
    {
        private readonly HashSet<Entity> _entities = new HashSet<Entity>();
        private readonly CompoundMask _mask;

        internal Filter(CompoundMask mask)
        {
            _mask = mask;
        }

        public int Count => _entities.Count;
        public CompoundMask Mask => _mask;
        public event Action<Entity> EntityAdded;
        public event Action<Entity> EntityRemoved;

        public Entity GetFirst()
        {
            var enumerator = _entities.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }
            return Entity.Null;
        }

        public List<Entity> GetEntities(List<Entity> buffer)
        {
            buffer.AddRange(_entities);
            return buffer;
        }

        public EntityEnumerator GetEnumerator()
        {
            return EntityEnumerator.Create(_entities);
        }
        
        IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator() => GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal void HandleEntity(Entity entity)
        {
            if (_mask.Matches(entity))
            {
                AddEntity(entity);
            }
            else
            {
                RemoveEntity(entity);
            }
        }

        internal void RemoveEntity(Entity entity)
        {
            if (_entities.Remove(entity))
            {
                EntityRemoved?.Invoke(entity);
            }
        }
        
        private void AddEntity(Entity entity)
        {
            _entities.Add(entity);
            EntityAdded?.Invoke(entity);
        }
    }
}