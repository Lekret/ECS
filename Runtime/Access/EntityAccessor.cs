using System;
using System.Collections.Generic;
using ECS.Runtime.Core;

namespace ECS.Runtime.Access
{
    internal sealed class EntityAccessor : IDisposable
    {
        private readonly List<List<Filter>> _typeToFilter = new List<List<Filter>>();
        private readonly EntityStorage _entityStorage;

        internal EntityAccessor(EntityStorage entityStorage)
        {
            _entityStorage = entityStorage;
        }
        
        public void Dispose()
        {
            _typeToFilter.Clear();
        }

        internal Filter GetFilter(CompoundMask mask)
        {
            for (var i = 0; i < _typeToFilter.Count; i++)
            {
                var filters = _typeToFilter[i];
                for (var k = 0; k < filters.Count; k++)
                {
                    var filter = filters[k];
                    if (filter.Mask.Equals(mask))
                        return filter;
                }
            }

            return CreateNewFilter(mask);
        }

        internal void CollectEntities(CompoundMask mask, ICollection<Entity> buffer)
        {
            foreach (var entity in _entityStorage.Entities)
            {
                if (mask.Matches(entity))
                    buffer.Add(entity);
            }
        }

        internal void OnComponentChanged(Entity entity, int componentIndex)
        {
            IncreaseFiltersRegistry();
            var filters = _typeToFilter[componentIndex];
            for (var i = 0; i < filters.Count; i++)
            {
                filters[i].HandleEntity(entity);
            }
        }

        internal void OnEntityDestroyed(Entity entity)
        {
            for (var i = 0; i < _typeToFilter.Count; i++)
            {
                var filters = _typeToFilter[i];
                for (var k = 0; k < filters.Count; k++)
                {
                    filters[k].RemoveEntity(entity);
                }
            }
        }

        private Filter CreateNewFilter(CompoundMask mask)
        {
            var filter = new Filter(mask);
            IncreaseFiltersRegistry();
            RegisterFilter(filter);
            foreach (var entity in _entityStorage.Entities)
            {
                filter.HandleEntity(entity);
            }

            return filter;
        }

        private void RegisterFilter(Filter filter)
        {
            var indices = filter.Mask.Indices;
            for (var i = 0; i < indices.Length; i++)
            {
                _typeToFilter[indices[i]].Add(filter);
            }
        }

        private void IncreaseFiltersRegistry()
        {
            while (_typeToFilter.Count < ComponentType.Count)
            {
                _typeToFilter.Add(new List<Filter>());
            }
        }
    }
}