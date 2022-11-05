using System.Collections;
using System.Collections.Generic;

namespace Lekret.Ecs
{
    public struct EntityEnumerator : IEnumerator<Entity>
    {
        private readonly List<Entity> _entities;
        private List<Entity>.Enumerator _enumerator;

        internal static EntityEnumerator Create(IEnumerable<Entity> collection)
        {
            var buffer = Pool<List<Entity>>.Spawn();
            buffer.AddRange(collection);
            return new EntityEnumerator(buffer);
        }

        private EntityEnumerator(List<Entity> entities)
        {
            _entities = entities;
            _enumerator = entities.GetEnumerator();
        }

        public Entity Current => _enumerator.Current;
        object IEnumerator.Current => Current;
            
        public bool MoveNext() => _enumerator.MoveNext();
            
        public void Reset() { }
            
        public void Dispose()
        {
            _entities.Clear();
            Pool<List<Entity>>.Release(_entities);
        }
    }
}