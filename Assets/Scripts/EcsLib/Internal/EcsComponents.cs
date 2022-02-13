using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections;
using System;
using EcsLib.Api;

namespace EcsLib.Internal
{
    internal sealed class EcsComponents
    {
        private readonly EntityWorld _entityWorld;
        private readonly List<Array> _rawComponents;
        private readonly List<bool[]> _flags;

        internal EcsComponents(EntityWorld entityWorld, int capacity)
        {
            _entityWorld = entityWorld;
            _rawComponents = new List<Array>(capacity);
            _flags = new List<bool[]>(capacity);
        }

        internal void ResetEntityData(Entity entity)
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
        internal T[] GetRawPool<T>()
        {
            var componentIndex = ComponentMeta<T>.Index;
            return GetUpdatedArray<T>(_rawComponents, componentIndex);
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool[] GetFlags<T>()
        {
            var componentIndex = ComponentMeta<T>.Index;
            return GetFlags(componentIndex);
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool[] GetFlags(int componentIndex)
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

            var requiredLength = _entityWorld.MaxEntityId + 1;
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