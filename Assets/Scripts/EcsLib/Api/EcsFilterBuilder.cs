using System.Collections.Generic;
using EcsLib.Internal;

namespace EcsLib.Api
{
    public struct EcsFilterBuilder
    {
        private static readonly List<int> IncludedIndices = new List<int>();
        private static readonly List<int> ExcludedIndices = new List<int>();
        private readonly EcsAccessor _accessor;
        private EcsFilter _filter;

        internal EcsFilterBuilder(EcsAccessor accessor)
        {
            _accessor = accessor;
            _filter = null;
            CleanIndices();
        }

        public EcsFilterBuilder Inc<T>()
        {
            if (IsEnded())
                return this;
            var index = ComponentMeta<T>.Index;
            if (ExcludedIndices.Contains(index))
            {
                EcsError.Handle($"Can't include already excluded type {typeof(T)}");
                return this;
            }
            IncludedIndices.Add(index);
            return this;
        }

        public EcsFilterBuilder Exc<T>()
        {
            if (IsEnded())
                return this;
            var index = ComponentMeta<T>.Index;
            if (IncludedIndices.Contains(index))
            {
                EcsError.Handle($"Can't exclude already included type {typeof(T)}");
                return this;
            }
            ExcludedIndices.Add(index);
            return this;
        }
        
        public EcsFilter End()
        {
            if (!IsEnded())
                _filter = _accessor.InternalBuildFilter(IncludedIndices, ExcludedIndices);
            CleanIndices();
            return _filter;
        }

        private bool IsEnded()
        {
            var ended = _filter != null;
            if (ended)
                LogError("Filter is ended");
            return ended;
        }

        private static void LogError(string message)
        {
            EcsError.Handle(message);
        }

        private static void CleanIndices()
        {
            if (IncludedIndices.Count > 0)
                IncludedIndices.Clear();
            if (ExcludedIndices.Count > 0)
                ExcludedIndices.Clear();
        }
    }
}