using System.Collections.Generic;
using EcsLib.Api;

namespace EcsLib.Internal
{
    internal sealed class EcsWorld
    {
        private readonly IDGenerator _idGenerator = new IDGenerator();
        private readonly Dictionary<int, Entity> _entities;

        internal int MaxEntityId => _idGenerator.CurrentId;
        internal IEnumerable<Entity> Entities => _entities.Values;

        internal EcsWorld(int initialEntityCapacity)
        {
            _entities = new Dictionary<int, Entity>(initialEntityCapacity);
        }

        internal Entity CreateEntity(EcsManager owner)
        {
            var id = _idGenerator.Next();
            var entity = new Entity(owner, id);
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
            return !entity.IsNull() && _entities.ContainsKey(entity.GetId());
        }

        internal void OnEntityDestroyed(Entity entity)
        {
            _entities.Remove(entity.GetId());
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