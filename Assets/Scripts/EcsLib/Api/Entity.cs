using System.Runtime.CompilerServices;
using EcsLib.Internal;

namespace EcsLib.Api
{
    public sealed class Entity
    {
        public static readonly Entity Null = new Entity(null, NULL_ID);
        private const int SINGLETON_ID = 0;
        private const int DESTROYED_ID = -1;
        private const int NULL_ID = -2;
        private readonly EcsManager _owner;
        private int _id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity Create(EcsManager ecsManager = null)
        {
            if (ecsManager == null)
                ecsManager = EcsManager.Instance;
            return ecsManager.CreateEntity();
        }
        
        internal Entity(EcsManager owner, int id)
        {
            _owner = owner;
            _id = id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsManager GetOwner()
        {
            return _owner;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetId()
        {
            return _id;
        }

        public void Destroy()
        {
            if (_id == SINGLETON_ID)
            {
                LogError("Can't destroy singleton entity");
                return;
            }
        
            if (_id == DESTROYED_ID)
            {
                LogError(ToString());
                return;
            }

            _owner.OnEntityDestroyed(this);
            _id = DESTROYED_ID;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasComponent(int componentIndex)
        {
            if (_id == DESTROYED_ID)
                return false;
            return GetFlag(componentIndex);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>()
        {
            if (_id == DESTROYED_ID)
                return false;
            return GetFlagRef<T>();
        }

        public T Get<T>()
        {
            if (_id == DESTROYED_ID || !Has<T>())
            {
                LogError($"{this}, can't get component {typeof(T)}, returning default");
                return default;
            }
            return GetComponentRef<T>();
        }

        public Entity Set<T>(T value)
        {
            if (_id == DESTROYED_ID)
            {
                LogError($"{this}, can't set component {typeof(T)}");
                return this;
            }
            
            GetFlagRef<T>() = true;
            GetComponentRef<T>() = value;
            var componentIndex = ComponentMeta<T>.Index;
            _owner.OnComponentChanged(this, componentIndex);
            return this;
        }

        public Entity Remove<T>() 
        {
            if (_id == DESTROYED_ID)
            {
                LogError($"{this} is destroyed, can't remove component {typeof(T)}");
                return this;
            }

            ref var flag = ref GetFlagRef<T>();
            if (flag)
            {
                flag = false;
                GetComponentRef<T>() = default;
                var componentIndex = ComponentMeta<T>.Index;
                _owner.OnComponentChanged(this, componentIndex);
                return this;
            }
            return this;
        }

        public override string ToString()
        {
            var str = EntityToString();
            if (_owner == null)
                return $"{str} is null";
            if (_id == DESTROYED_ID)
                return $"{str} is destroyed";
            return $"{str}({_id})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string EntityToString()
        {
            if (_id == NULL_ID)
                return "NullEntity";
            if (_id == SINGLETON_ID)
                return "SingletonEntity";
            return "Entity";
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref T GetComponentRef<T>()
        {
            return ref _owner.Components.GetRawPool<T>()[_id];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref bool GetFlagRef<T>()
        {
            return ref _owner.Components.GetFlags<T>()[_id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref bool GetFlag(int componentIndex)
        {
            return ref _owner.Components.GetFlags(componentIndex)[_id];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LogError(string message)
        {
            Error.Handle(message);
        }
    }
}