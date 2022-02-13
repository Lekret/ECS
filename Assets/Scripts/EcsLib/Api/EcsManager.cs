using EcsLib.Internal;
using System;

namespace EcsLib.Api
{
    public sealed partial class EcsManager : IDisposable
    {
        public static EcsManager Instance { get; set; }
        
        private readonly EcsAccessHandler _accessHandler;
        private readonly EntityWorld _world;
        internal readonly EcsComponents Components;
        
        public bool IsDestroyed { get; private set; }

        public EcsManager(int componentsInitialCapacity = 30)
        {
            _world = new EntityWorld();
            _accessHandler = new EcsAccessHandler(_world);
            Components = new EcsComponents(_world, componentsInitialCapacity);
            _singletonEntity = CreateEntity();

            if (Instance == null)
                Instance = this;
        }

        public Entity CreateEntity()
        {
            if (CheckDestroyed())
                return Entity.Null;
            return _world.CreateEntity(this);
        }

        public Entity GetEntity(int id)
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
        
        public void Dispose()
        {
            _world.DestroyAll();
            IsDestroyed = true;
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
            Components.ResetEntityData(entity);
            _world.RemoveEntity(entity);
            _accessHandler.OnEntityDestroyed(entity);
        }

        private bool CheckDestroyed()
        {
            if (IsDestroyed)
            {
                Error.Handle($"{nameof(EcsManager)} is already disposed");
                return true;
            }
            return false;
        }
    }
}