using System.Collections;
using System.Collections.Generic;
using ECSimplicity.Internal;

namespace ECSimplicity
{
    public struct MutableEntityEnumerator : IEnumerator<Entity>
    {
        private static readonly Pool<List<Entity>> BufferPool = Pool<List<Entity>>.Instance;
        private readonly List<Entity> _entities;
        private List<Entity>.Enumerator _enumerator;

        internal static MutableEntityEnumerator Create(IEnumerable<Entity> collection)
        {
            var buffer = BufferPool.Get();
            buffer.AddRange(collection);
            return new MutableEntityEnumerator(buffer);
        }

        private MutableEntityEnumerator(List<Entity> entities)
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
            BufferPool.Release(_entities);
        }
    }
}