using System;
using System.Runtime.CompilerServices;
using EcsLib.Internal;

namespace EcsLib.Api
{
    public readonly struct Entity : IEquatable<Entity>
    {
        public const int NULL_ID = -1;
        public static readonly Entity Null = new Entity(null, NULL_ID);

        private readonly EcsManager _owner;
        private readonly int _id;

        internal Entity(EcsManager owner, int id)
        {
            _owner = owner;
            _id = id;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity Create(EcsManager ecsManager = null)
        {
            if (ecsManager == null)
                ecsManager = EcsManager.Instance;
            return ecsManager.CreateEntity();
        }

        public bool Equals(Entity other)
        {
            return _id == other._id && _owner == other._owner;
        }
        
        public static bool operator ==(Entity left, Entity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"Entity({_id})";
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull()
        {
            return _id == NULL_ID;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive()
        {
            return _owner.IsAlive(this);
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
            if (!IsAlive())
                throw new Exception($"Cannot get component from non alive entity: {this}");
            if (!HasComponent<T>())
                throw new Exception($"Cannot get component from entity: {this}");
            return _owner.Components.GetComponent<T>(_id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Set<T>(T value)
        {
            if (!IsAlive())
            {
                LogError($"Cannot set {typeof(T)} for non alive entity: {this}");
                return this;
            }

            _owner.Components.SetComponent(this, value);
            _owner.OnComponentChanged(this, ComponentMeta<T>.Index);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Remove<T>()
        {
            if (!IsAlive())
            {
                LogError($"Cannot remove {typeof(T)} from non alive entity: {this}");
                return this;
            }

            var removed = _owner.Components.RemoveComponent<T>(this);
            if (removed)
            {
                _owner.OnComponentChanged(this, ComponentMeta<T>.Index);
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>()
        {
            return IsAlive() && HasComponent<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Has(int componentIndex)
        {
            return IsAlive() && HasComponent(componentIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy()
        {
            if (!IsAlive())
            {
                LogError($"Cannot destroy non alive entity: {this}");
                return;
            }

            _owner.OnEntityDestroyed(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasComponent<T>()
        {
            return _owner.Components.GetFlag<T>(_id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasComponent(int componentIndex)
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