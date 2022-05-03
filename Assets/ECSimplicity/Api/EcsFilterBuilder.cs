using System.Collections.Generic;
using ECSimplicity.Internal;

namespace ECSimplicity
{
    public struct EcsFilterBuilder
    {
        private static readonly Queue<EcsIndices> IndicesPool = new Queue<EcsIndices>();
        private readonly EcsAccessor _accessor;
        private readonly EcsIndices _indices;
        private bool _filterIsEnd;

        internal EcsFilterBuilder(EcsAccessor accessor)
        {
            _accessor = accessor;
            _filterIsEnd = false;
            _indices = GetIndices();
        }

        private static EcsIndices GetIndices()
        {
            if (IndicesPool.Count > 0)
                return IndicesPool.Dequeue();
            return new EcsIndices();
        }

        private static void ReleaseList(EcsIndices indices)
        {
            indices.Clear();
            IndicesPool.Enqueue(indices);
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
            ReleaseList(_indices);
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