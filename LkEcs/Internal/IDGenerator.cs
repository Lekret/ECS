using System.Collections.Generic;

namespace LkEcs.Internal
{
    internal sealed class IDGenerator
    {
        private readonly HashSet<int> _takenIds = new HashSet<int>();
        private int _currentId;
        internal int CurrentId => _currentId;
        
        internal int Next()
        {
            while (_takenIds.Contains(_currentId))
            {
                _currentId++;
            }
            _takenIds.Add(_currentId);
            return _currentId;
        }

        internal void ReleaseId(int id)
        {
            _takenIds.Remove(id);
        }
    }
}