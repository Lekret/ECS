using ECSimplicity.Internal;

namespace ECSimplicity
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
    
    public sealed class EcsAdmin
    {
        private readonly EcsAccessor _accessor;
        private readonly EcsWorld _world;

        internal readonly EcsComponents Components;
        
        public EcsAdmin() : this(EcsConfig.Default) { }
        
        public EcsAdmin(EcsConfig config)
        {
            _world = new EcsWorld(config.InitialEntityCapacity);
            _accessor = new EcsAccessor(_world);
            Components = new EcsComponents(_world, config.InitialComponentsCapacity);
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
            var builder = _accessor.CreateFilterBuilder();
            return builder;
        }

        public void DestroyAllEntities()
        {
            _world.DestroyAll();
        }
        
        public bool IsAlive(Entity entity)
        {
            return _world.IsAlive(entity);
        }

        internal void OnComponentChanged(Entity entity, int componentIndex)
        {
            _accessor.OnComponentChanged(entity, componentIndex);
        }
        
        internal void OnEntityDestroyed(Entity entity)
        {
            Components.OnEntityDestroyed(entity);
            _world.OnEntityDestroyed(entity);
            _accessor.OnEntityDestroyed(entity);
        }
    }
}