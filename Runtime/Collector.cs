using System.Collections;
using System.Collections.Generic;

namespace Lekret.Ecs
{
    public sealed class Collector : IEnumerable<Entity>
    {
        private readonly HashSet<Entity> _entities = new HashSet<Entity>();
        
        public int Count => _entities.Count;
        
        public void Clear()
        {
            _entities.Clear();
        }

        public EntityEnumerator GetEnumerator()
        {
            return EntityEnumerator.Create(_entities);
        }
        
        IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator() => GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal void AddEntity(Entity entity)
        {
            _entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
        }
    }
}