using System.Collections;
using System.Collections.Generic;

namespace ECSimplicity
{
    public struct MutableEntityEnumerator : IEnumerator<Entity>
    {
        private static readonly Stack<List<Entity>> Pools = new Stack<List<Entity>>();
        private readonly List<Entity> _entities;
        private List<Entity>.Enumerator _enumerator;

        internal static MutableEntityEnumerator Create(IEnumerable<Entity> collection)
        {
            var buffer = GetBuffer();
            buffer.AddRange(collection);
            return new MutableEntityEnumerator(buffer);
        }

        private static List<Entity> GetBuffer()
        {
            if (Pools.Count == 0)
                return new List<Entity>();
            var buffer = Pools.Pop();
            buffer.Clear();
            return buffer;
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
            Pools.Push(_entities);
        }
    }
}