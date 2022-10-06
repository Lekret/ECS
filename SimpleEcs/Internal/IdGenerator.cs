using System.Collections.Generic;

namespace SimpleEcs.Internal
{
    internal sealed class IdGenerator
    {
        private readonly Queue<int> _recycled = new Queue<int>();
        private int _currentId;
        internal int CurrentId => _currentId;
        
        internal int Next()
        {
            if (_recycled.Count > 0)
                return _recycled.Dequeue();
            _currentId++;
            return _currentId;
        }

        internal void ReleaseId(int id)
        {
            _recycled.Enqueue(id);
        }
    }
}