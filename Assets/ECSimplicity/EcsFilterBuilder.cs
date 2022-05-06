using ECSimplicity.Internal;

namespace ECSimplicity
{
    public struct EcsFilterBuilder
    {
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
            if (CheckFilterEnd())
                return this;
            _indices.Inc<T>();
            return this;
        }

        public EcsFilterBuilder Exc<T>()
        {
            if (CheckFilterEnd())
                return this;
            _indices.Exc<T>();
            return this;
        }
        
        public EcsFilter End()
        {
            var filter = _accessor.InternalBuildFilter(_indices.Included, _indices.Excluded);
            IndicesPool.Release(_indices);
            _filterIsEnd = true;
            return filter;
        }

        private bool CheckFilterEnd()
        {
            if (_filterIsEnd)
                LogError($"{nameof(EcsFilterBuilder)} is already end");
            return _filterIsEnd;
        }

        private static void LogError(string message)
        {
            EcsError.Handle(message);
        }
    }
}