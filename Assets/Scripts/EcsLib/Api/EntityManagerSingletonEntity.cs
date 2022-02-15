using System.Runtime.CompilerServices;

namespace EcsLib.Api
{
    public sealed partial class EcsManager
    {
        private readonly Entity _singletonEntity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet<T>(out T value)
        {
            if (Has<T>())
            {
                value = Get<T>();
                return true;
            }
            value = default;
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>()
        {
            return _singletonEntity.Has<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>()
        {
            return _singletonEntity.Get<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsManager Set<T>(T value)
        {
            _singletonEntity.Set(value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsManager Remove<T>()
        {
            _singletonEntity.Remove<T>();
            return this;
        }
    }
}