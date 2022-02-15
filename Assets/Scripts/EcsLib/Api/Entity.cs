using System;
using System.Runtime.CompilerServices;
using EcsLib.Internal;
using UnityEngine;

namespace EcsLib.Api
{
    public sealed class Entity : IEquatable<Entity>
    {
        public static readonly Entity Null = new Entity(null, NULL_ID);
        private const int SINGLETON_ID = 0;
        private const int NULL_ID = -1;
        private readonly EcsManager _owner;
        private readonly int _id;
        private bool _isDestroyed;

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

        public bool Equals(Entity other)
        {
            return other != null && _id == other._id && _owner == other._owner;
        }

        public override string ToString()
        {
            return $"Entity({_id})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDestroyed()
        {
            return _isDestroyed;
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

        public T Get<T>()
        {
            if (_id == NULL_ID)
            {
                LogError($"[{nameof(Get)}<{typeof(T)}>] Entity is null");
                return default;
            }
            if (_isDestroyed)
            {
                LogError($"[{nameof(Get)}<{typeof(T)}>] {this} is destroyed");
                return default;
            }
            if (!Has<T>())
            {
                LogError($"[{nameof(Get)}<{typeof(T)}>] {this} don't have component");
                return default;
            }
            return GetComponentRef<T>();
        }

        public Entity Set<T>(T value)
        {
            if (_id == NULL_ID)
            {
                LogError($"[{nameof(Set)}<{typeof(T)}>] Entity is null");
                return this;
            }
            if (_isDestroyed)
            {
                LogError($"[{nameof(Set)}<{typeof(T)}>] {this} is destroyed");
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
            if (_id == NULL_ID)
            {
                LogError($"[{nameof(Remove)}<{typeof(T)}>] Entity is null");
                return this;
            }
            if (_isDestroyed)
            {
                LogError($"[{nameof(Remove)}<{typeof(T)}>] {this} is destroyed");
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>()
        {
            if (_id == NULL_ID)
            {
                LogError($"[{nameof(Has)}<{typeof(T)}>] Entity is null");
                return false;
            }
            if (_isDestroyed)
                return false;
            return GetFlagRef<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasComponent(int componentIndex)
        {
            if (_isDestroyed)
                return false;
            return GetFlag(componentIndex);
        }
        
        public void Destroy()
        {
            if (_id == NULL_ID)
            {
                LogError($"[{nameof(Destroy)}] Entity is null");
                return;
            }
            if (_id == SINGLETON_ID)
            {
                LogError($"[{nameof(Destroy)}] Entity is singleton");
                return;
            }
            if (_isDestroyed)
            {
                LogError($"[{nameof(Destroy)}] {this} is already destroyed");
                return;
            }

            _isDestroyed = true;
            _owner.OnEntityDestroyed(this);
        }

        internal void Resurrect()
        {
            _isDestroyed = false;
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
            ErrorHelper.Handle(message);
        }
    }
}