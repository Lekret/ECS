using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EcsLib.Api;

namespace EcsLib.Internal
{
    internal sealed class EcsComponents
    {
        private readonly EcsWorld _world;
        private readonly List<Array> _rawComponents;
        private readonly List<bool[]> _flags;

        internal EcsComponents(EcsWorld world, int capacity)
        {
            _world = world;
            _rawComponents = new List<Array>(capacity);
            _flags = new List<bool[]>(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnEntityDestroyed(Entity entity)
        {
            var entityId = entity.GetId();
            foreach (var pool in _rawComponents)
            {
                pool.SetValue(default, entityId);
            }
            foreach (var pool in _flags)
            {
                pool[entityId] = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T GetComponent<T>(int entityId)
        {
            return GetRawPool<T>()[entityId];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool GetFlag<T>(int entityId)
        {
            return GetFlag(ComponentMeta<T>.Index, entityId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool GetFlag(int componentIndex, int entityId)
        {
            return GetFlags(componentIndex)[entityId];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetComponent<T>(Entity entity, T value)
        {
            var componentIndex = ComponentMeta<T>.Index;
            var id = entity.GetId();
            GetRawPool<T>()[id] = value;
            GetFlags(componentIndex)[id] = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool RemoveComponent<T>(Entity entity)
        {
            var componentIndex = ComponentMeta<T>.Index;
            var id = entity.GetId();
            ref var hasComponent = ref GetFlags(componentIndex)[id];
            if (hasComponent)
            {
                GetRawPool<T>()[id] = default;
                hasComponent = false;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T[] GetRawPool<T>()
        {
            var componentIndex = ComponentMeta<T>.Index;
            return GetUpdatedArray<T>(_rawComponents, componentIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool[] GetFlags(int componentIndex)
        {
            return GetUpdatedArray<bool>(_flags, componentIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T[] GetUpdatedArray<T>(IList list, int componentIndex)
        {
            while (list.Count < ComponentMeta.Count)
            {
                list.Add(null);
            }

            var requiredLength = _world.MaxEntityId + 1;
            if (list[componentIndex] == null)
            {
                var newArray = new T[requiredLength];
                list[componentIndex] = newArray;
                return newArray;
            }

            var array = (T[]) list[componentIndex];
            if (array.Length < requiredLength)
            {
                Array.Resize(ref array, requiredLength);
                list[componentIndex] = array;
            }
            
            return array;
        }
    }
}