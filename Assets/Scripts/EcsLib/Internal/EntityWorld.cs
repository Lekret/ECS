using System.Collections.Generic;
using EcsLib.Api;

namespace EcsLib.Internal
{
    internal sealed class EntityWorld
    {
        private readonly Queue<Entity> _destroyedEntities = new Queue<Entity>();
        private readonly List<Entity> _entities = new List<Entity>();
        private readonly IDGenerator _idGenerator = new IDGenerator();

        internal int MaxEntityId => _idGenerator.CurrentId;
        internal IEnumerable<Entity> Entities => _entities;

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
        
        internal void DestroyEntity(Entity entity)
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
            ErrorHelper.Handle(message);
        }
    }
}