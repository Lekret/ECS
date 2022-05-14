namespace ECSimplicity.Internal
{
    internal sealed class IDGenerator
    {
        private int _currentId;
        internal int CurrentId => _currentId;
        
        internal int Next()
        {
            _currentId++;
            if (_currentId == int.MaxValue)
                _currentId = 0;
            return _currentId;
        }
    }
}