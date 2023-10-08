using System;
using System.Collections.Generic;
using ECS.Runtime.Access;

namespace ECS.Runtime.Core
{
    public sealed class World : IDisposable
    {
        private readonly EntityAccessor _accessor;
        private readonly EntityStorage _entityStorage;
        private readonly ComponentStorage _componentStorage;

        public World() : this(EcsConfig.Default)
        {
        }

        public World(EcsConfig config)
        {
            _entityStorage = new EntityStorage(config.InitialEntityCapacity);
            _accessor = new EntityAccessor(_entityStorage);
            _componentStorage = new ComponentStorage(_entityStorage, config.InitialEntityCapacity);
        }

        public int EntitiesCount => _entityStorage.EntitiesCount;
        
        public void GetEntities(List<Entity> buffer)
        {
            buffer.Clear();
            buffer.AddRange(_entityStorage.Entities);
        }

        public List<Entity> GetAllEntities()
        {
            return new List<Entity>(_entityStorage.Entities);
        }

        public Entity CreateEntity()
        {
            return _entityStorage.CreateEntity(this);
        }

        public Entity GetEntityById(int id)
        {
            return _entityStorage.GetEntityById(id);
        }

        public Filter Filter(CompoundMask mask)
        {
            return _accessor.GetFilter(mask);
        }

        public Filter Filter(MaskBuilder maskBuilder)
        {
            return Filter(Mask.AllOf(maskBuilder));
        }

        public void Query(CompoundMask mask, ICollection<Entity> buffer)
        {
            _accessor.CollectEntities(mask, buffer);
        }

        public void Query(MaskBuilder maskBuilder, ICollection<Entity> buffer)
        {
            _accessor.CollectEntities(Mask.AllOf(maskBuilder), buffer);
        }

        public List<Entity> Query(CompoundMask mask)
        {
            var buffer = new List<Entity>();
            Query(mask, buffer);
            return buffer;
        }

        public List<Entity> Query(MaskBuilder maskBuilder)
        {
            return Query(Mask.AllOf(maskBuilder));
        }

        public bool IsAlive(Entity entity)
        {
            return !entity.IsNull() && _entityStorage.IsAlive(entity);
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
            if (!_componentStorage.GetFlag<T>(entity.Id))
                throw new Exception($"Entity does not have a component {typeof(T)}: {entity}");
            return _componentStorage.GetComponent<T>(entity.Id);
        }

        public void GetComponents(Entity entity, List<object> buffer)
        {
            _componentStorage.GetComponents(entity, buffer);
        }

        public void Set<T>(Entity entity, T value = default)
        {
            if (IsAlive(entity))
            {
                _componentStorage.SetComponent(entity, value);
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
                var removed = _componentStorage.RemoveComponent<T>(entity);
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
            return IsAlive(entity) && _componentStorage.GetFlag<T>(entity.Id);
        }

        public void Destroy(Entity entity)
        {
            if (IsAlive(entity))
            {
                _componentStorage.OnEntityDestroyed(entity);
                _entityStorage.OnEntityDestroyed(entity);
                _accessor.OnEntityDestroyed(entity);
            }
            else
            {
                throw new Exception($"Entity is not alive: {entity}");
            }
        }

        public bool Has(Entity entity, int componentIndex)
        {
            return IsAlive(entity) && _componentStorage.GetFlag(componentIndex, entity.Id);
        }

        public bool HasAny(Entity entity, int[] indices)
        {
            if (!IsAlive(entity))
                return false;
            
            for (var i = 0; i < indices.Length; i++)
            {
                if (_componentStorage.GetFlag(indices[i], entity.Id))
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
                if (!_componentStorage.GetFlag(indices[i], entity.Id))
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
            _componentStorage.Dispose();
            _entityStorage.Dispose();
            _accessor.Dispose();
        }
    }
}