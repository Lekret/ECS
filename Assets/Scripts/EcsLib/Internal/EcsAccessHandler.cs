using System.Collections.Generic;
using EcsLib.Api;

namespace EcsLib.Internal
{
    internal sealed class EcsAccessHandler
    {
        private readonly List<List<EcsFilter>> _filtersByType = new List<List<EcsFilter>>();
        private readonly EntityWorld _world;

        public EcsAccessHandler(EntityWorld world)
        {
            _world = world;
        }

        internal EcsFilterBuilder CreateFilter()
        {
            return new EcsFilterBuilder(this);
        }
        
        internal void OnComponentChanged(Entity entity, int componentIndex)
        {
            IncreaseFiltersRegistry();
            var filters = _filtersByType[componentIndex];
            foreach (var filter in filters)
            {
                filter.HandleEntity(entity);
            }
        }

        internal void OnEntityDestroyed(Entity entity)
        {
            foreach (var filters in _filtersByType)
            {
                foreach (var filter in filters)
                {
                    filter.RemoveEntity(entity);
                }
            }
        }

        internal EcsFilter InternalBuildFilter(List<int> included, List<int> excluded)
        {
            var filter = new EcsFilter(included, excluded);
            IncreaseFiltersRegistry();
            RegisterFilter(filter, included);
            RegisterFilter(filter, excluded);
            foreach (var entity in _world.Entities)
            {
                filter.HandleEntity(entity);
            }
            return filter;
        }
        
        private void RegisterFilter(EcsFilter filter, IEnumerable<int> componentIndices)
        {
            foreach (var index in componentIndices)
            {
                _filtersByType[index].Add(filter);
            }
        }

        private void IncreaseFiltersRegistry()
        {
            while (_filtersByType.Count < ComponentMeta.Count)
            {
                _filtersByType.Add(new List<EcsFilter>());
            }
        }
    }
}