using System.Collections.Generic;
using System.Linq;
using EcsLib.Api;

namespace EcsLib.Internal
{
    internal sealed class EcsAccessor
    {
        private readonly List<List<EcsFilter>> _filtersByType = new List<List<EcsFilter>>();
        private readonly EcsWorld _world;

        internal EcsAccessor(EcsWorld world)
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
            if (TryGetExistingFilter(included, excluded, out var filter))
                return filter;
            return CreateNewFilter(included, excluded);
        }

        private bool TryGetExistingFilter(List<int> included, List<int> excluded, out EcsFilter filter)
        {
            foreach (var filters in _filtersByType)
            {
                foreach (var f in filters)
                {
                    if (f.MatchesIndices(included, excluded))
                    {
                        filter = f;
                        return true;
                    }
                }
            }
            filter = null;
            return false;
        }
        
        private EcsFilter CreateNewFilter(IEnumerable<int> included, IEnumerable<int> excluded)
        {
            var filter = new EcsFilter(included.ToArray(), excluded.ToArray());
            IncreaseFiltersRegistry();
            RegisterFilter(filter, included);
            RegisterFilter(filter, excluded);
            foreach (var entity in _world.Entities)
            {
                if (entity.IsDestroyed()) 
                    continue;
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