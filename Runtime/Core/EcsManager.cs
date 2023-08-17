using System;
using System.Collections.Generic;
using ECS.Runtime.Access;

namespace ECS.Runtime.Core
{
    public sealed class EcsManager : IDisposable
    {
        private readonly EntityAccessor _accessor;
        private readonly World _world;
        private readonly Storage _storage;

        public EcsManager() : this(EcsConfig.Default)
        {
        }

        public EcsManager(EcsConfig config)
        {
            _world = new World(config.InitialEntityCapacity);
            _accessor = new EntityAccessor(_world);
            _storage = new Storage(_world);
        }

        public int EntitiesCount => _world.EntitiesCount;
        
        public void GetEntities(List<Entity> buffer)
        {
            buffer.Clear();
            buffer.AddRange(_world.Entities);
        }

        public List<Entity> GetEntitiesCopy()
        {
            return new List<Entity>(_world.Entities);
        }

        public Entity CreateEntity()
        {
            return _world.CreateEntity(this);
        }

        public Entity GetOrCreateEntityWithId(int id)
        {
            return _world.GetOrCreateEntityWithId(this, id);
        }

        public Entity GetEntityById(int id)
        {
            return _world.GetEntityById(id);
        }

        public Filter Filter(CompoundMask mask)
        {
            return _accessor.GetFilter(mask);
        }

        public List<Entity> Query(MaskBuilder maskBuilder)
        {
            var buffer = new List<Entity>();
            _accessor.CollectEntities(Mask.AllOf(maskBuilder), buffer);
            return buffer;
        }

        public void Query(MaskBuilder maskBuilder, ICollection<Entity> buffer)
        {
            _accessor.CollectEntities(Mask.AllOf(maskBuilder), buffer);
        }

        public Filter Filter(MaskBuilder maskBuilder)
        {
            return Filter(Mask.AllOf(maskBuilder));
        }

        public bool IsAlive(Entity entity)
        {
            return !entity.IsNull() && _world.IsAlive(entity);
        }

        public bool TryGet<T>(Entity entity, out T value)
        {
            if (Has<T>(entity))
            {
                value = Get<T>(entity);
                return true;
            }

            value = default;
            return false;
        }

        public T Get<T>(Entity entity)
        {
            if (!IsAlive(entity))
                throw new Exception($"Cannot get component from non alive entity: {entity}");
            if (!_storage.GetFlag<T>(entity.Id))
                throw new Exception($"Entity does not have a component {typeof(T)}: {entity}");
            return _storage.GetComponent<T>(entity.Id);
        }

        public void GetComponents(Entity entity, List<object> buffer)
        {
            _storage.GetComponents(entity, buffer);
        }

        public void Set<T>(Entity entity, T value = default)
        {
            if (IsAlive(entity))
            {
                _storage.SetComponent(entity, value);
                OnComponentChanged(entity, ComponentType<T>.Index);
            }
            else
            {
                throw new Exception($"Cannot set {typeof(T)} for non alive entity: {entity}");
            }
        }

        public void Remove<T>(Entity entity)
        {
            if (IsAlive(entity))
            {
                var removed = _storage.RemoveComponent<T>(entity);
                if (removed)
                    OnComponentChanged(entity, ComponentType<T>.Index);
            }
            else
            {
                throw new Exception($"Cannot remove {typeof(T)} for non alive entity: {entity}");
            }
        }

        public bool Has<T>(Entity entity)
        {
            return IsAlive(entity) && _storage.GetFlag<T>(entity.Id);
        }

        public void Destroy(Entity entity)
        {
            if (IsAlive(entity))
            {
                _storage.OnEntityDestroyed(entity);
                _world.OnEntityDestroyed(entity);
                _accessor.OnEntityDestroyed(entity);
            }
            else
            {
                throw new Exception($"Entity is not alive: {entity}");
            }
        }

        public bool Has(Entity entity, int componentIndex)
        {
            return IsAlive(entity) && _storage.GetFlag(componentIndex, entity.Id);
        }

        public bool HasAny(Entity entity, int[] indices)
        {
            if (!IsAlive(entity))
                return false;
            
            for (var i = 0; i < indices.Length; i++)
            {
                if (_storage.GetFlag(indices[i], entity.Id))
                    return true;
            }

            return false;
        }

        public bool HasAll(Entity entity, int[] indices)
        {
            if (!IsAlive(entity))
                return false;
            
            for (var i = 0; i < indices.Length; i++)
            {
                if (!_storage.GetFlag(indices[i], entity.Id))
                    return false;
            }

            return true;
        }

        private void OnComponentChanged(Entity entity, int componentIndex)
        {
            _accessor.OnComponentChanged(entity, componentIndex);
        }

        public void Dispose()
        {
            _storage?.Dispose();
            _world?.Dispose();
        }
    }
}