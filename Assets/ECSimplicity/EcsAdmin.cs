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
        private bool _isDestroyed;

        internal readonly EcsComponents Components;
        
        public EcsAdmin() : this(EcsConfig.Default) { }
        
        public EcsAdmin(EcsConfig config)
        {
            _world = new EcsWorld(config.InitialEntityCapacity);
            _accessor = new EcsAccessor(_world);
            Components = new EcsComponents(_world, config.InitialComponentsCapacity);
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
            var builder = _accessor.CreateFilterBuilder();
            if (CheckDestroyed())
                builder.End();
            return builder;
        }

        public void Destroy()
        {
            if (CheckDestroyed())
                return;
            _world.DestroyAll();
            _isDestroyed = true;
        }
        
        public bool IsAlive(Entity entity)
        {
            return _world.IsAlive(entity);
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
                EcsError.Handle($"{nameof(EcsAdmin)} is already disposed");
                return true;
            }
            return false;
        }
    }
}