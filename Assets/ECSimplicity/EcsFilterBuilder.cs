using System;
using ECSimplicity.Internal;

namespace ECSimplicity
{
    public struct EcsFilterBuilder
    {
        private static readonly Pool<EcsIndices> IndicesPool = new Pool<EcsIndices>();
        private readonly EcsAccessor _accessor;
        private readonly EcsIndices _indices;
        private bool _filterIsEnd;

        internal EcsFilterBuilder(EcsAccessor accessor)
        {
            _accessor = accessor;
            _filterIsEnd = false;
            _indices = IndicesPool.Get();
        }

        public EcsFilterBuilder Inc<T>()
        {
            ThrowIfEnd();
            _indices.Inc<T>();
            return this;
        }

        public EcsFilterBuilder Exc<T>()
        {
            ThrowIfEnd();
            _indices.Exc<T>();
            return this;
        }
        
        public EcsFilter End()
        {
            ThrowIfEnd();
            var filter = _accessor.GetFilter(_indices.Included, _indices.Excluded);
            IndicesPool.Release(_indices);
            _filterIsEnd = true;
            return filter;
        }

        private void ThrowIfEnd()
        {
            if (_filterIsEnd)
                throw new Exception($"{nameof(EcsFilterBuilder)} is already end");
        }
    }
}