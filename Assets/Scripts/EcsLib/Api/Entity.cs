using System;
using System.Runtime.CompilerServices;
using EcsLib.Internal;

namespace EcsLib.Api
{
    public sealed class Entity : IEquatable<Entity>
    {
        private const int SINGLETON_ID = 0;
        private const int NULL_ID = -1;
        public static readonly Entity Null = new Entity(null, NULL_ID);
        private readonly EcsManager _owner;
        private readonly int _id;
        private bool _isDestroyed;

        internal Entity(EcsManager owner, int id)
        {
            _owner = owner;
            _id = id;
        }

        public bool Equals(Entity other)
        {
            return other != null && _id == other._id && _owner == other._owner;
        }
        
        public override string ToString()
        {
            return $"Entity({_id})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity Create(EcsManager ecsManager = null)
        {
            if (ecsManager == null)
                ecsManager = EcsManager.Instance;
            return ecsManager.CreateEntity();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDestroyed()
        {
            return _isDestroyed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSingleton()
        {
            return _id == SINGLETON_ID;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull()
        {
            return _id == NULL_ID;
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
        public T Get<T>()
        {
            if (IsNull())
            {
                LogError($"[{nameof(Get)}<{typeof(T)}>] Entity is null");
                return default;
            }

            if (IsDestroyed())
            {
                LogError($"[{nameof(Get)}<{typeof(T)}>] {this} is destroyed");
                return default;
            }

            if (!Has<T>())
            {
                LogError($"[{nameof(Get)}<{typeof(T)}>] {this} don't have component");
                return default;
            }

            return GetComponent<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Set<T>(T value)
        {
            if (IsNull())
            {
                LogError($"[{nameof(Set)}<{typeof(T)}>] Entity is null");
                return this;
            }

            if (IsDestroyed())
            {
                LogError($"[{nameof(Set)}<{typeof(T)}>] {this} is destroyed");
                return this;
            }

            if (SetComponent(value))
            {
                _owner.OnComponentChanged(this, ComponentMeta<T>.Index);
            }
            else
            {
                LogError($"[{nameof(SetComponent)}<{typeof(T)}>] {this} Can't add component by invariant");
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Remove<T>()
        {
            if (IsNull())
            {
                LogError($"[{nameof(Remove)}<{typeof(T)}>] Entity is null");
                return this;
            }

            if (IsDestroyed())
            {
                LogError($"[{nameof(Remove)}<{typeof(T)}>] {this} is destroyed");
                return this;
            }

            var flag = GetFlag<T>();
            if (flag)
            {
                if (RemoveComponent<T>())
                {
                    _owner.OnComponentChanged(this, ComponentMeta<T>.Index);
                }
                else
                {
                    LogError($"[{nameof(RemoveComponent)}<{typeof(T)}>] {this} Can't remove component by invariant");
                }
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>()
        {
            if (IsNull()) return false;
            if (IsDestroyed()) return false;
            return GetFlag<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasComponent(int componentIndex)
        {
            if (IsDestroyed())
                return false;
            return GetFlag(componentIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy()
        {
            if (IsNull())
            {
                LogError($"[{nameof(Destroy)}] Entity is null");
                return;
            }

            if (IsSingleton())
            {
                LogError($"[{nameof(Destroy)}] Entity is singleton");
                return;
            }

            if (IsDestroyed())
            {
                LogError($"[{nameof(Destroy)}] {this} is already destroyed");
                return;
            }

            _isDestroyed = true;
            _owner.OnEntityDestroyed(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Resurrect()
        {
            _isDestroyed = false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool SetComponent<T>(T value)
        {
            return _owner.Components.SetComponent(this, value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool RemoveComponent<T>()
        {
            return _owner.Components.RemoveComponent<T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetComponent<T>()
        {
            return _owner.Components.GetComponent<T>(_id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool GetFlag<T>()
        {
            return _owner.Components.GetFlag<T>(_id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool GetFlag(int componentIndex)
        {
            return _owner.Components.GetFlag(componentIndex, _id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LogError(string message)
        {
            EcsError.Handle(message);
        }
    }
}