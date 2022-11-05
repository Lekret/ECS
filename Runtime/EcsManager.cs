using System;
using System.Collections.Generic;

namespace Lekret.Ecs
{
    public sealed class EcsManager
    {
        private readonly EntityAccessor _accessor;
        private readonly World _world;
        private readonly Components _components;
        
        public EcsManager() : this(EcsConfig.Default) { }
        
        public EcsManager(EcsConfig config)
        {
            _world = new World(config.InitialEntityCapacity);
            _accessor = new EntityAccessor(_world);
            _components = new Components(_world, config.InitialComponentsCapacity);
        }

        public List<Entity> GetEntities()
        {
            return new List<Entity>(_world.Entities);
        }
        
        public Entity CreateEntity()
        {
            return _world.CreateEntity(this);
        }
        
        public Entity? GetOrCreateEntityWithId(int id)
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

        public void DestroyAllEntities()
        {
            _world.DestroyAll();
        }
        
        public bool IsAlive(Entity entity)
        {
            return _world.IsAlive(entity);
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
            if (!HasComponent<T>(entity))
                throw new Exception($"Entity do not have a component {typeof(T)}: {entity}");
            return _components.GetComponent<T>(entity.Id);
        }

        public void Set<T>(Entity entity, T value = default)
        {
            if (IsAlive(entity))
            {
                _components.SetComponent(entity, value);
                OnComponentChanged(entity, Component<T>.Index);
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
                var removed = _components.RemoveComponent<T>(entity);
                if (removed)
                    OnComponentChanged(entity, Component<T>.Index);
            }
            else
            {
                throw new Exception($"Cannot remove {typeof(T)} for non alive entity: {entity}");
            }
        }

        public bool Has<T>(Entity entity)
        {
            return IsAlive(entity) && HasComponent<T>(entity);
        }

        public void Destroy(Entity entity)
        {
            if (IsAlive(entity))
            {
                OnEntityDestroyed(entity);
            }
            else
            {
                throw new Exception($"Entity is not alive: {entity}");
            }
        }

        public bool Has(Entity entity, int componentIndex)
        {
            return IsAlive(entity) && HasComponent(entity, componentIndex);
        }

        public bool HasAny(Entity entity, int[] indices)
        {
            for (var i = 0; i < indices.Length; i++)
            {
                if (_components.GetFlag(indices[i], entity.Id))
                    return true;
            }
            return false;
        }
        
        public bool HasAll(Entity entity, int[] indices)
        {
            for (var i = 0; i < indices.Length; i++)
            {
                if (!_components.GetFlag(indices[i], entity.Id))
                    return false;
            }
            return true;
        }
        
        private bool HasComponent<T>(Entity entity)
        {
            return _components.GetFlag<T>(entity.Id);
        }

        private bool HasComponent(Entity entity, int componentIndex)
        {
            return _components.GetFlag(componentIndex, entity.Id);
        }

        private void OnComponentChanged(Entity entity, int componentIndex)
        {
            _accessor.OnComponentChanged(entity, componentIndex);
        }

        private void OnEntityDestroyed(Entity entity)
        {
            _components.OnEntityDestroyed(entity);
            _world.OnEntityDestroyed(entity);
            _accessor.OnEntityDestroyed(entity);
        }
    }
}