using EcsLib.Internal;

namespace EcsLib.Api
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
        public static EcsManager Instance { get; set; }

        private bool _isDestroyed;
        private readonly EcsAccessor _accessor;
        private readonly EcsWorld _world;
        
        internal readonly EcsComponents Components;
        
        public EcsManager() : this(EcsConfig.Default) { }
        
        public EcsManager(EcsConfig config)
        {
            _world = new EcsWorld(config.InitialEntityCapacity);
            _accessor = new EcsAccessor(_world);
            Components = new EcsComponents(_world, config.InitialComponentsCapacity);

            if (Instance == null)
                Instance = this;
        }

        public bool IsDestroyed()
        {
            return _isDestroyed;
        }

        public Entity CreateEntity()
        {
            if (CheckDestroyed())
                return Entity.Null;
            return _world.CreateEntity(this);
        }

        public Entity GetEntityById(int id)
        {
            if (CheckDestroyed())
                return Entity.Null;
            return _world.GetEntityById(id);
        }

        public EcsFilterBuilder Filter()
        {
            return _accessor.CreateFilterBuilder();
        }
        
        public void Destroy()
        {
            if (CheckDestroyed())
                return;
            _world.DestroyAll();
            _isDestroyed = true;
        }

        internal void OnComponentChanged(Entity entity, int componentIndex)
        {
            if (CheckDestroyed())
                return;
            _accessor.OnComponentChanged(entity, componentIndex);
        }
        
        internal void OnEntityDestroyed(Entity entity)
        {
            if (CheckDestroyed())
                return;
            Components.OnEntityDestroyed(entity);
            _world.OnEntityDestroyed(entity);
            _accessor.OnEntityDestroyed(entity);
        }

        private bool CheckDestroyed()
        {
            if (_isDestroyed)
            {
                EcsError.Handle($"{nameof(EcsManager)} is already disposed");
                return true;
            }
            return false;
        }
        
        internal bool IsAlive(Entity entity)
        {
            return _world.IsAlive(entity);
        }
    }
}