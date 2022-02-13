using System;
using System.Collections.Generic;
using EcsLib.Api;
using UnityEngine;

namespace EcsLib.Internal
{
    internal sealed class EntityWorld
    {
        private readonly Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        private readonly IDGenerator _idGenerator = new IDGenerator();

        internal int MaxEntityId => _idGenerator.CurrentId;
        internal IReadOnlyCollection<Entity> Entities => _entities.Values;

        internal Entity CreateEntity(EcsManager owner)
        {
            var entity = new Entity(owner, _idGenerator.Next());
            _entities.Add(entity.GetId(), entity);
            return entity;
        }

        internal Entity GetEntityById(int id)
        {
            if (_entities.TryGetValue(id, out var entity))
            {
                return entity;
            }
            return Entity.Null;
        }

        internal void RemoveEntity(Entity entity)
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