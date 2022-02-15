using EcsLib.Internal;

namespace EcsLib.Api
{
    public sealed partial class EcsManager
    {
        public static EcsManager Instance { get; set; }

        private bool _isDestroyed;
        private readonly EcsAccessHandler _accessHandler;
        private readonly EntityWorld _world;
        internal readonly EcsComponents Components;
        
        public EcsManager(int componentsInitialCapacity = 30)
        {
            _world = new EntityWorld();
            _accessHandler = new EcsAccessHandler(_world);
            Components = new EcsComponents(_world, componentsInitialCapacity);
            _singletonEntity = CreateEntity();

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
            if (CheckDestroyed())
                return default;
            return _accessHandler.CreateFilter();
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
            _accessHandler.OnComponentChanged(entity, componentIndex);
        }
        
        internal void OnEntityDestroyed(Entity entity)
        {
            if (CheckDestroyed())
                return;
            Components.OnEntityDestroyed(entity);
            _world.OnEntityDestroyed(entity);
            _accessHandler.OnEntityDestroyed(entity);
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
    }
}