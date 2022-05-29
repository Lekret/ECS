using System.Collections.Generic;

namespace LkEcs.Internal
{
    internal class EcsIndices
    {
        private readonly List<int> _included = new List<int>();
        private readonly List<int> _excluded = new List<int>();
        public List<int> Included => _included;
        public List<int> Excluded => _excluded;

        public void Inc<T>()
        {
            var index = ComponentMeta<T>.Index;
            if (_included.Contains(index))
            {
                LogError($"Type is already included: {typeof(T)}");
            }
            else if (_excluded.Contains(index))
            {
                LogError($"Can't include excluded type: {typeof(T)}");
            }
            else
            {
                _included.Add(index);
            }
        }

        public void Exc<T>()
        {
            var index = ComponentMeta<T>.Index;
            if (_excluded.Contains(index))
            {
                LogError($"Type is already excluded: {typeof(T)}");
            }
            else if (_included.Contains(index))
            {
                LogError($"Can't exclude included type: {typeof(T)}");
            }
            else
            {
                _excluded.Add(index);
            }
        }
        
        public void Clear()
        {
            _included.Clear();
            _excluded.Clear();
        }
        
        private static void LogError(string message)
        {
            EcsError.Handle(message);
        }
    }
}