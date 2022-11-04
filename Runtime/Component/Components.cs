using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Lekret.Ecs.Internal
{
    internal sealed class Components
    {
        private readonly World _world;
        private readonly List<Array> _rawComponents;
        private readonly List<bool[]> _flags;

        internal Components(World world, int capacity)
        {
            _world = world;
            _rawComponents = new List<Array>(capacity);
            _flags = new List<bool[]>(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnEntityDestroyed(Entity entity)
        {
            var entityId = entity.Id;
            for (var i = 0; i < _rawComponents.Count; i++)
            {
                _rawComponents[i].SetValue(default, entityId);
            }

            for (var i = 0; i < _flags.Count; i++)
            {
                _flags[i][entityId] = false;
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
            return GetFlag(Component<T>.Index, entityId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool GetFlag(int componentIndex, int entityId)
        {
            return GetFlags(componentIndex)[entityId];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetComponent<T>(Entity entity, T value)
        {
            var componentIndex = Component<T>.Index;
            var id = entity.Id;
            GetRawPool<T>()[id] = value;
            GetFlags(componentIndex)[id] = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool RemoveComponent<T>(Entity entity)
        {
            var componentIndex = Component<T>.Index;
            var id = entity.Id;
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
            var componentIndex = Component<T>.Index;
            return GetArrayForComponent<T>(_rawComponents, componentIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool[] GetFlags(int componentIndex)
        {
            return GetArrayForComponent<bool>(_flags, componentIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T[] GetArrayForComponent<T>(IList listOfArrays, int componentIndex)
        {
            while (listOfArrays.Count < Component.Count)
            {
                listOfArrays.Add(null);
            }

            var requiredLength = _world.MaxEntityId + 1;
            var array = (T[]) listOfArrays[componentIndex];
            if (array == null)
            {
                array = new T[requiredLength];
                listOfArrays[componentIndex] = array;
            }
            else
            {
                if (array.Length < requiredLength)
                {
                    Array.Resize(ref array, requiredLength);
                    listOfArrays[componentIndex] = array;
                }
            }
            
            return array;
        }
    }
}