using System;
using System.Collections.Generic;

namespace ECS.Runtime.Core
{
    internal sealed class Storage : IDisposable
    {
        private const int ResizeMultiplier = 2;
        private readonly World _world;
        private readonly List<Array> _components;
        private readonly List<bool[]> _flags;
        private int _flagsCapacity;

        internal Storage(World world, int initialEntityCapacity)
        {
            _components = new List<Array>();
            _flags = new List<bool[]>();
            _world = world;
            _flagsCapacity = initialEntityCapacity;
            _world.MaxEntityIdChanged += OnMaxEntityIdChanged;
            ComponentType.CountChanged += OnComponentsCountChanged;
            OnComponentsCountChanged();
        }
        
        public void Dispose()
        {
            _components.Clear();
            _flags.Clear();
            _world.MaxEntityIdChanged -= OnMaxEntityIdChanged;
            ComponentType.CountChanged -= OnComponentsCountChanged;
        }

        internal void OnEntityDestroyed(Entity entity)
        {
            for (var i = 0; i < _flags.Count; i++)
            {
                if (_flags[i][entity.Id])
                    _components[i].SetValue(default, entity.Id);
            }
        }

        internal T GetComponent<T>(int entityId)
        {
            return GetPool<T>()[entityId];
        }

        internal bool GetFlag<T>(int entityId)
        {
            return GetFlag(ComponentType<T>.Index, entityId);
        }

        internal bool GetFlag(int componentIndex, int entityId)
        {
            return _flags[componentIndex][entityId];
        }

        internal void SetComponent<T>(Entity entity, T value)
        {
            var id = entity.Id;
            GetPool<T>()[id] = value;
            _flags[ComponentType<T>.Index][id] = true;
        }

        internal bool RemoveComponent<T>(Entity entity)
        {
            ref var hasComponent = ref _flags[ComponentType<T>.Index][entity.Id];
            if (hasComponent)
            {
                GetPool<T>()[entity.Id] = default;
                hasComponent = false;
                return true;
            }

            return false;
        }

        internal void GetComponents(Entity entity, List<object> buffer)
        {
            buffer.Clear();
            for (var i = 0; i < ComponentType.Count; i++)
            {
                if (_flags[i][entity.Id])
                {
                    buffer.Add(_components[i].GetValue(entity.Id));
                }
            }
        }

        private T[] GetPool<T>()
        {
            var componentIndex = ComponentType<T>.Index;
            while (_components.Count < ComponentType.Count)
            {
                _components.Add(null);
            }

            var requiredLength = GetRequiredRowLength();
            var array = (T[]) _components[componentIndex];
            if (array == null)
            {
                array = new T[requiredLength * 2];
                _components[componentIndex] = array;
            }
            else
            {
                if (array.Length < requiredLength)
                {
                    Array.Resize(ref array, requiredLength * ResizeMultiplier);
                    _components[componentIndex] = array;
                }
            }

            return array;
        }

        private void OnMaxEntityIdChanged()
        {
            var requiredLength = GetRequiredRowLength();
            if (_flagsCapacity >= requiredLength)
                return;

            _flagsCapacity = requiredLength * ResizeMultiplier;
            for (var i = 0; i < _flags.Count; i++)
            {
                var flags = _flags[i];
                Array.Resize(ref flags, _flagsCapacity);
                _flags[i] = flags;
            }
        }

        private void OnComponentsCountChanged()
        {
            var requiredLength = _world.MaxEntityId + 1;
            if (_flagsCapacity < requiredLength)
                _flagsCapacity = requiredLength;
            
            while (_flags.Count < ComponentType.Count)
            {
                var flags = _flagsCapacity > 0 ? new bool[_flagsCapacity] : Array.Empty<bool>();
                _flags.Add(flags);
            }
        }

        private int GetRequiredRowLength()
        {
            return _world.MaxEntityId + 1;
        }
    }
}