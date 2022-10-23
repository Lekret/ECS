using System.Collections.Generic;

namespace Lekret.Ecs.Internal
{
    internal sealed class World
    {
        private struct RecycledEntity
        {
            public int Id;
            public short Version;
        }
        
        private readonly Queue<RecycledEntity> _recycledEntities = new Queue<RecycledEntity>();
        private readonly Dictionary<int, Entity> _entities;
        private int _currentId;
        
        internal int MaxEntityId => _currentId - _recycledEntities.Count;
        internal IEnumerable<Entity> Entities => _entities.Values;

        internal World(int initialEntityCapacity)
        {
            _entities = new Dictionary<int, Entity>(initialEntityCapacity);
        }

        internal Entity CreateEntity(EcsManager owner)
        {
            Entity entity;
            if (_recycledEntities.Count > 0)
            {
                var recycledEntity = _recycledEntities.Dequeue();
                entity = new Entity(recycledEntity.Id, recycledEntity.Version++, owner);
            }
            else
            {
                while (_entities.ContainsKey(_currentId))
                {
                    _currentId++;
                }
                entity = new Entity(_currentId, 0, owner);
            }
            _entities.Add(entity.Id, entity);
            return entity;
        }
        
        internal Entity GetOrCreateEntityWithId(EcsManager owner, int id)
        {
            if (_entities.TryGetValue(id, out var entity))
                return entity;
            entity = new Entity(id, 0, owner);
            _entities.Add(id, entity);
            return entity;
        }
        
        internal Entity GetEntityById(int id)
        {
            if (_entities.TryGetValue(id, out var entity))
                return entity;
            return Entity.Null;
        }

        internal bool IsAlive(Entity entity)
        {
            return _entities.TryGetValue(entity.Id, out var existingEntity) && 
                   existingEntity.Version == entity.Version;
        }

        internal void OnEntityDestroyed(Entity entity)
        {
            _entities.Remove(entity.Id);
            _recycledEntities.Enqueue(new RecycledEntity
            {
                Id = entity.Id,
                Version = entity.Version
            });
        }
        
        internal void DestroyAll()
        {
            foreach (var entity in _entities.Values)
            {
                entity.Destroy();
            }
            _entities.Clear();
        }
    }
}