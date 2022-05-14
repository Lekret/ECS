using System;
using System.Runtime.CompilerServices;
using ECSimplicity.Internal;

namespace ECSimplicity
{
    public readonly struct Entity : IEquatable<Entity>
    {
        public const int NullId = -1;
        public static readonly Entity Null = new Entity(NullId, null);

        public readonly int Id;
        public readonly EcsAdmin Owner;

        internal Entity(int id, EcsAdmin owner)
        {
            Id = id;
            Owner = owner;
        }

        public bool Equals(Entity other)
        {
            return Id == other.Id && Owner == other.Owner;
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
            return $"Entity({Id})";
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull()
        {
            return Id == NullId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive()
        {
            return Owner.IsAlive(this);
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
            return Owner.Components.GetComponent<T>(Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Set<T>(T value = default)
        {
            if (IsAlive())
            {
                Owner.Components.SetComponent(this, value);
                Owner.OnComponentChanged(this, ComponentMeta<T>.Index);
            }
            else
            {
                LogError($"Cannot set {typeof(T)} for non alive entity: {this}");
            }
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

            var removed = Owner.Components.RemoveComponent<T>(this);
            if (removed)
                Owner.OnComponentChanged(this, ComponentMeta<T>.Index);
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
            if (IsAlive())
                Owner.OnEntityDestroyed(this);
            else
                LogError($"Cannot destroy non alive entity: {this}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasComponent<T>()
        {
            return Owner.Components.GetFlag<T>(Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasComponent(int componentIndex)
        {
            return Owner.Components.GetFlag(componentIndex, Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LogError(string message)
        {
            EcsError.Handle(message);
        }
    }
}