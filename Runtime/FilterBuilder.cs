using System;
using Lekret.Ecs.Internal;

namespace Lekret.Ecs
{
    public struct FilterBuilder
    {
        private readonly EntityAccessor _accessor;
        private readonly FilterIndices _indices;
        private bool _isEnd;

        internal FilterBuilder(EntityAccessor accessor)
        {
            _accessor = accessor;
            _indices = Pool<FilterIndices>.Spawn();
            _isEnd = false;
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
            Pool<FilterIndices>.Release(_indices);
            _isEnd = true;
            return filter;
        }

        public Collector ToCollector()
        {
            return End().ToCollector();
        }

        private void ThrowIfEnd()
        {
            if (_isEnd)
                throw new Exception($"{nameof(FilterBuilder)} is already end");
        }
    }
}