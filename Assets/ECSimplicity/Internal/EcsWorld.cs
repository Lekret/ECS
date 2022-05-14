using System.Collections.Generic;

namespace ECSimplicity.Internal
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

        internal Entity CreateEntity(EcsAdmin owner)
        {
            var id = _idGenerator.Next();
            var entity = new Entity(id, owner);
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
            return _entities.ContainsKey(entity.Id);
        }

        internal void OnEntityDestroyed(Entity entity)
        {
            _entities.Remove(entity.Id);
            _idGenerator.ReleaseId(entity.Id);
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