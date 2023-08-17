using System;
using System.Collections.Generic;

namespace ECS.Runtime.Core
{
    internal sealed class World : IDisposable
    {
        private struct PooledEntity
        {
            public int Id;
            public short Gen;
        }

        private readonly Queue<PooledEntity> _pooledEntities = new Queue<PooledEntity>();
        private readonly Dictionary<int, Entity> _entities;
        private int _maxEntityId;

        internal int MaxEntityId => _maxEntityId;
        internal IEnumerable<Entity> Entities => _entities.Values;
        internal int EntitiesCount => _entities.Values.Count;

        internal World(int initialEntityCapacity)
        {
            _entities = new Dictionary<int, Entity>(initialEntityCapacity);
        }

        internal Entity CreateEntity(EcsManager owner)
        {
            Entity entity;
            if (_pooledEntities.TryDequeue(out var recycledEntity))
            {
                entity = new Entity(recycledEntity.Id, recycledEntity.Gen++, owner);
            }
            else
            {
                while (_entities.ContainsKey(_maxEntityId))
                {
                    _maxEntityId++;
                }

                entity = new Entity(_maxEntityId, 0, owner);
            }

            _entities.Add(entity.Id, entity);
            return entity;
        }

        internal Entity GetOrCreateEntityWithId(EcsManager owner, int id)
        {
            if (_entities.TryGetValue(id, out var entity))
                return entity;

            while (_maxEntityId < id)
            {
                _maxEntityId++;

                if (GetEntityById(id) == Entity.Null)
                {
                    _pooledEntities.Enqueue(new PooledEntity
                    {
                        Id = _maxEntityId,
                        Gen = 0
                    });
                }
            }
            
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
                   existingEntity.Gen == entity.Gen;
        }

        internal void OnEntityDestroyed(Entity entity)
        {
            _entities.Remove(entity.Id);
            _pooledEntities.Enqueue(new PooledEntity
            {
                Id = entity.Id,
                Gen = entity.Gen
            });
        }

        public void Dispose()
        {
            _entities.Clear();
            _pooledEntities.Clear();
            _maxEntityId = 0;
        }
    }
}