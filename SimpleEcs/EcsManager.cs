using System;
using SimpleEcs.Internal;

namespace SimpleEcs
{
    public struct EcsConfig
    {
        public static readonly EcsConfig Default = new EcsConfig
        {
            InitialComponentsCapacity = 128,
            InitialEntityCapacity = 512,
        };
        
        public int InitialComponentsCapacity;
        public int InitialEntityCapacity;
    }
    
    public sealed class EcsManager
    {
        private readonly EcsAccessor _accessor;
        private readonly EcsWorld _world;
        private readonly EcsComponents _components;
        
        public EcsManager() : this(EcsConfig.Default) { }
        
        public EcsManager(EcsConfig config)
        {
            _world = new EcsWorld(config.InitialEntityCapacity);
            _accessor = new EcsAccessor(_world);
            _components = new EcsComponents(_world, config.InitialComponentsCapacity);
        }

        public Entity CreateEntity()
        {
            return _world.CreateEntity(this);
        }

        public Entity GetEntityById(int id)
        {
            return _world.GetEntityById(id);
        }

        public EcsFilterBuilder Filter()
        {
            return _accessor.CreateFilterBuilder();
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
                throw new Exception($"Cannot get component from entity: {entity}");
            return _components.GetComponent<T>(entity.Id);
        }

        public void Set<T>(Entity entity, T value = default)
        {
            if (IsAlive(entity))
            {
                _components.SetComponent(entity, value);
                OnComponentChanged(entity, ComponentMeta<T>.Index);
            }
            else
            {
                LogError($"Cannot set {typeof(T)} for non alive entity: {entity}");
            }
        }

        public void Remove<T>(Entity entity)
        {
            if (IsAlive(entity))
            {
                var removed = _components.RemoveComponent<T>(entity);
                if (removed)
                    OnComponentChanged(entity, ComponentMeta<T>.Index);
            }
            else
            {
                LogError($"Cannot remove {typeof(T)} from non alive entity: {entity}");
            }
        }

        public bool Has<T>(Entity entity)
        {
            return IsAlive(entity) && HasComponent<T>(entity);
        }

        public void Destroy(Entity entity)
        {
            if (IsAlive(entity))
                OnEntityDestroyed(entity);
            else
                LogError($"Cannot destroy non alive entity: {entity}");
        }
        
        internal bool Has(Entity entity, int componentIndex)
        {
            return IsAlive(entity) && HasComponent(entity, componentIndex);
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
        
        private static void LogError(string message)
        {
            EcsError.Handle(message);
        }
    }
}