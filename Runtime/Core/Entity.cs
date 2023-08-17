using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ECS.Runtime.Core
{
    [Serializable]
    public readonly struct Entity : IEquatable<Entity>
    {
        public const int NullId = -1;
        public const short NullGen = -1;
        public static readonly Entity Null = new Entity(NullId, NullGen, null);

        public readonly int Id;
        public readonly short Gen;
        public readonly EcsManager Owner;

        internal Entity(int id, short gen, EcsManager owner)
        {
            Id = id;
            Gen = gen;
            Owner = owner;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Entity other)
        {
            return Id == other.Id &&
                   Gen == other.Gen &&
                   Owner == other.Owner;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Gen, Owner);
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
            return Owner == null || Id == NullId || Gen == NullGen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive()
        {
            return Owner.IsAlive(this);
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
        public void GetAll(List<object> buffer)
        {
            Owner.GetComponents(this, buffer);
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
        internal bool HasAny(int[] indices)
        {
            return Owner.HasAny(this, indices);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasAll(int[] indices)
        {
            return Owner.HasAll(this, indices);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy()
        {
            Owner.Destroy(this);
        }
    }
}