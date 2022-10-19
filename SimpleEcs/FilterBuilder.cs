using System;
using SimpleEcs.Internal;

namespace SimpleEcs
{
    public struct FilterBuilder
    {
        private static readonly Pool<FilterIndices> IndicesPool = Pool<FilterIndices>.Instance;
        private readonly EntityAccessor _accessor;
        private readonly FilterIndices _indices;
        private bool _filterIsEnd;

        internal FilterBuilder(EntityAccessor accessor)
        {
            _accessor = accessor;
            _filterIsEnd = false;
            _indices = IndicesPool.Spawn();
        }

        public FilterBuilder Inc<T>()
        {
            ThrowIfEnd();
            _indices.Inc<T>();
            return this;
        }

        public FilterBuilder Exc<T>()
        {
            ThrowIfEnd();
            _indices.Exc<T>();
            return this;
        }
        
        public Filter End()
        {
            ThrowIfEnd();
            var filter = _accessor.GetFilter(_indices.Included, _indices.Excluded);
            _indices.Clear();
            IndicesPool.Release(_indices);
            _filterIsEnd = true;
            return filter;
        }

        private void ThrowIfEnd()
        {
            if (_filterIsEnd)
                throw new Exception($"{nameof(FilterBuilder)} is already end");
        }
    }
}