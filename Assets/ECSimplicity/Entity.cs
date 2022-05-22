using System;
using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Entity other)
        {
            return Id == other.Id && Owner == other.Owner;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Entity left, Entity right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            return !IsNull() && Owner.IsAlive(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet<T>(out T value)
        {
            return Owner.TryGet(this, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>()
        {
            return Owner.Get<T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Set<T>(T value = default)
        {
            Owner.Set(this, value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Remove<T>()
        {
            Owner.Remove<T>(this);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>()
        {
            return Owner.Has<T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Has(int componentIndex)
        {
            return Owner.Has(this, componentIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy()
        {
            Owner.Destroy(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasComponent<T>()
        {
            return Owner.Has<T>(this);
        }
    }
}