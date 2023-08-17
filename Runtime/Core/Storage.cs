using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ECS.Runtime.Core
{
    internal sealed class Storage
    {
        private readonly World _world;
        private readonly List<Array> _components;
        private readonly List<bool[]> _flags;

        internal Storage(World world)
        {
            _world = world;
            _components = new List<Array>();
            _flags = new List<bool[]>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnEntityDestroyed(Entity entity)
        {
            var entityId = entity.Id;
            for (var i = 0; i < _components.Count; i++)
            {
                _components[i].SetValue(default, entityId);
            }

            for (var i = 0; i < _flags.Count; i++)
            {
                _flags[i][entityId] = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T GetComponent<T>(int entityId)
        {
            return GetPool<T>()[entityId];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool GetFlag<T>(int entityId)
        {
            return GetFlag(ComponentType<T>.Index, entityId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool GetFlag(int componentIndex, int entityId)
        {
            return GetFlags(componentIndex)[entityId];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetComponent<T>(Entity entity, T value)
        {
            var id = entity.Id;
            GetPool<T>()[id] = value;
            GetFlags(ComponentType<T>.Index)[id] = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool RemoveComponent<T>(Entity entity)
        {
            ref var hasComponent = ref GetFlags(ComponentType<T>.Index)[entity.Id];
            if (hasComponent)
            {
                GetPool<T>()[entity.Id] = default;
                hasComponent = false;
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void GetComponents(Entity entity, List<object> buffer)
        {
            buffer.Clear();
            for (var i = 0; i < ComponentType.Count; i++)
            {
                var flags = GetEntitiesSizedArray<bool>(_flags, i);
                if (flags[entity.Id])
                {
                    var components = GetEntitiesSizedArray<object>(_components, i);
                    buffer.Add(components[entity.Id]);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T[] GetPool<T>()
        {
            return GetEntitiesSizedArray<T>(_components, ComponentType<T>.Index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool[] GetFlags(int componentIndex)
        {
            return GetEntitiesSizedArray<bool>(_flags, componentIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T[] GetEntitiesSizedArray<T>(IList listOfArrays, int componentIndex)
        {
            while (listOfArrays.Count < ComponentType.Count)
            {
                listOfArrays.Add(null);
            }

            var requiredLength = (_world.MaxEntityId + 1) * 2;
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