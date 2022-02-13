namespace EcsLib.Api
{
    public sealed partial class EcsManager
    {
        private readonly Entity _singletonEntity;
        
        public bool Has<T>()
        {
            return _singletonEntity.Has<T>();
        }

        public T Get<T>()
        {
            return _singletonEntity.Get<T>();
        }

        public EcsManager Set<T>(T value)
        {
            _singletonEntity.Set(value);
            return this;
        }

        public EcsManager Remove<T>()
        {
            _singletonEntity.Remove<T>();
            return this;
        }
    }
}