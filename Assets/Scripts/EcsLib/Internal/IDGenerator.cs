namespace EcsLib.Internal
{
    internal sealed class IDGenerator
    {
        private int _currentId = -1;
        internal int CurrentId => _currentId;
        
        internal int Next()
        {
            _currentId++;
            return _currentId;
        }
    }
}