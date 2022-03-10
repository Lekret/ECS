using System.Collections.Generic;
using EcsLib.Internal;

namespace EcsLib.Api
{
    public struct EcsFilterBuilder
    {
        private static readonly List<int> IncludedIndices = new List<int>();
        private static readonly List<int> ExcludedIndices = new List<int>();
        private readonly EcsAccessor _accessor;
        private bool _isInvalid;

        internal static readonly EcsFilterBuilder Null = new EcsFilterBuilder
        {
            _isInvalid = true
        };

        internal EcsFilterBuilder(EcsAccessor accessor)
        {
            _isInvalid = false;
            _accessor = accessor;
            if (IncludedIndices.Count > 0 || ExcludedIndices.Count > 0)
                LogError($"Previous {nameof(EcsFilterBuilder)} isn't ended");
            CleanIndices();
        }

        public EcsFilterBuilder Inc<T>()
        {
            if (CheckInvalid())
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
            if (CheckInvalid())
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
            CheckInvalid();
            var filter = _accessor.InternalBuildFilter(IncludedIndices, ExcludedIndices);
            CleanIndices();
            return filter;
        }

        private bool CheckInvalid()
        {
            if (_isInvalid)
                LogError($"{nameof(EcsFilterBuilder)} is invalid");
            return _isInvalid;
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