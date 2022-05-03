using System.Collections.Generic;
using ECSimplicity.Internal;

namespace ECSimplicity
{
    public struct EcsFilterBuilder
    {
        private static readonly Queue<List<int>> ListPool = new Queue<List<int>>();
        private readonly List<int> _included;
        private readonly List<int> _excluded;
        private readonly EcsAccessor _accessor;
        private bool _filterIsEnd;

        internal EcsFilterBuilder(EcsAccessor accessor)
        {
            _accessor = accessor;
            _included = GetBuffer();
            _excluded = GetBuffer();
            _filterIsEnd = false;
        }

        private static List<int> GetBuffer()
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
            if (_excluded.Contains(index))
            {
                LogError($"Can't include already excluded type {typeof(T)}");
                return this;
            }
            if (_included.Contains(index))
            {
                LogError($"Can't included already included type {typeof(T)}");
                return this;
            }
            _included.Add(index);
            return this;
        }

        public EcsFilterBuilder Exc<T>()
        {
            if (CheckFilterEnd())
                return this;
            var index = ComponentMeta<T>.Index;
            if (_included.Contains(index))
            {
                LogError($"Can't exclude already included type {typeof(T)}");
                return this;
            }
            if (_excluded.Contains(index))
            {
                LogError($"Can't exclude already excluded type {typeof(T)}");
                return this;
            }
            _excluded.Add(index);
            return this;
        }
        
        public EcsFilter End()
        {
            var filter = _accessor.InternalBuildFilter(_included, _excluded);
            _filterIsEnd = true;
            ReleaseList(_included);
            ReleaseList(_excluded);
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