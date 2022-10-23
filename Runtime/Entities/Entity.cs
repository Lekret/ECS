using System;
using System.Runtime.CompilerServices;

namespace Lekret.Ecs
{
    [Serializable]
    public readonly struct Entity : IEquatable<Entity>
    {
        public const int NullId = -1;
        public const short NullVersion = -1;
        public static readonly Entity Null = new Entity(NullId, NullVersion, null);

        public readonly int Id;
        public readonly short Version;
        public readonly EcsManager Owner;

        internal Entity(int id, short version, EcsManager owner)
        {
            Id = id;
            Version = version;
            Owner = owner;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Entity other)
        {
            return Id == other.Id && 
                   Version == other.Version && 
                   Owner == other.Owner;
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
            return Owner == null || Id == NullId || Version == NullVersion;
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