using EcsLib.Api.Invariants;
using EcsLib.Internal;

namespace EcsLib.Api
{
    public struct EcsConfig
    {
        public int ComponentsInitialCapacity;
    }
    
    public sealed class EcsManager
    {
        public static EcsManager Instance { get; set; }

        private bool _isDestroyed;
        private readonly EcsAccessor _accessor;
        private readonly EcsInvariance _invariance;
        private readonly EcsWorld _world;
        
        internal readonly EcsComponents Components;
        
        public EcsManager(EcsConfig config = default)
        {
            _world = new EcsWorld();
            _accessor = new EcsAccessor(_world);
            _invariance = new EcsInvariance();
            Components = new EcsComponents(_world, _invariance, config.ComponentsInitialCapacity);

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
            return _accessor.CreateFilter();
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

        public void Invariant(IEcsAddInvariant invariant)
        {
            _invariance.AddInvariant(invariant);
        }

        public void Invariant(IEcsRemoveInvariant invariant)
        {
            _invariance.AddInvariant(invariant);
        }
    }
}