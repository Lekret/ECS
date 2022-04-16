using System.Collections.Generic;
using EcsLib.Internal;

namespace EcsLib.Api
{
    public struct EcsFilterBuilder
    {
        private static readonly Queue<List<int>> ListPool = new Queue<List<int>>();
        private readonly List<int> _includedIndices;
        private readonly List<int> _excludedIndices;
        private readonly EcsAccessor _accessor;
        private bool _filterIsEnd;

        internal EcsFilterBuilder(EcsAccessor accessor)
        {
            _accessor = accessor;
            _includedIndices = GetList();
            _excludedIndices = GetList();
            _filterIsEnd = false;
        }

        private static List<int> GetList()
        {
            if (ListPool.Count > 0)
                return ListPool.Dequeue();
            return new List<int>();
        }

        private static void ReleaseList(List<int> list)
        {
            ListPool.Enqueue(list);
        }

        public EcsFilterBuilder Inc<T>()
        {
            if (CheckFilterEnd())
                return this;
            var index = ComponentMeta<T>.Index;
            if (_excludedIndices.Contains(index))
            {
                EcsError.Handle($"Can't include already excluded type {typeof(T)}");
                return this;
            }
            _includedIndices.Add(index);
            return this;
        }

        public EcsFilterBuilder Exc<T>()
        {
            if (CheckFilterEnd())
                return this;
            var index = ComponentMeta<T>.Index;
            if (_includedIndices.Contains(index))
            {
                EcsError.Handle($"Can't exclude already included type {typeof(T)}");
                return this;
            }
            _excludedIndices.Add(index);
            return this;
        }
        
        public EcsFilter End()
        {
            var filter = _accessor.InternalBuildFilter(_includedIndices.ToArray(), _excludedIndices.ToArray());
            _filterIsEnd = true;
            ReleaseList(_includedIndices);
            ReleaseList(_excludedIndices);
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