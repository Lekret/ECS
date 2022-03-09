using System.Collections.Generic;
using EcsLib.Api;

namespace EcsLib.Internal
{
    internal sealed class EcsWorld
    {
        private readonly Queue<Entity> _destroyedEntities = new Queue<Entity>();
        private readonly IDGenerator _idGenerator = new IDGenerator();
        private readonly List<Entity> _entities;

        internal int MaxEntityId => _idGenerator.CurrentId;
        internal IEnumerable<Entity> Entities => _entities;

        public EcsWorld(int initialEntityCapacity)
        {
            _entities = new List<Entity>(initialEntityCapacity);
        }
        
        internal Entity CreateEntity(EcsManager owner)
        {
            Entity entity;
            if (_destroyedEntities.Count > 0)
            {
                entity = _destroyedEntities.Dequeue();
                entity.Resurrect();
            }
            else
            {
                entity = new Entity(owner, _idGenerator.Next());
                _entities.Add(entity);
            }
            
            return entity;
        }

        internal Entity GetEntityById(int id)
        {
            if (id < 0 || id >= _entities.Count)
            {
                LogError($"Id is out of bounds ({id})");
                return Entity.Null;
            }

            var entity = _entities[id];
            if (entity.IsDestroyed())
                return Entity.Null;
            return entity;
        }
        
        internal void OnEntityDestroyed(Entity entity)
        {
            _destroyedEntities.Enqueue(entity);
        }

        internal void DestroyAll()
        {
            foreach (var entity in _entities)
            {
                if (entity.IsDestroyed())
                    continue;
                entity.Destroy();
            }
            _entities.Clear();
        }

        private static void LogError(string message)
        {
            EcsError.Handle(message);
        }
    }
}