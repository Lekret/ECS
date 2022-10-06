using System.Collections.Generic;
using System.Linq;

namespace SimpleEcs.Internal
{
    internal sealed class EcsAccessor
    {
        private readonly List<List<EcsFilter>> _typeToFilter = new List<List<EcsFilter>>();
        private readonly EcsWorld _world;

        internal EcsAccessor(EcsWorld world)
        {
            _world = world;
        }

        internal EcsFilterBuilder CreateFilterBuilder()
        {
            return new EcsFilterBuilder(this);
        }
        
        internal void OnComponentChanged(Entity entity, int componentIndex)
        {
            IncreaseFiltersRegistry();
            var filters = _typeToFilter[componentIndex];
            foreach (var filter in filters)
            {
                filter.HandleEntity(entity);
            }
        }

        internal void OnEntityDestroyed(Entity entity)
        {
            foreach (var filters in _typeToFilter)
            {
                foreach (var filter in filters)
                {
                    filter.RemoveEntity(entity);
                }
            }
        }

        internal EcsFilter GetFilter(List<int> included, List<int> excluded)
        {
            if (TryGetExistingFilter(included, excluded, out var filter))
                return filter;
            return CreateNewFilter(included.ToArray(), excluded.ToArray());
        }

        private bool TryGetExistingFilter(List<int> included, List<int> excluded, out EcsFilter result)
        {
            foreach (var filters in _typeToFilter)
            {
                foreach (var filter in filters)
                {
                    if (filter.MatchesIndices(included, excluded))
                    {
                        result = filter;
                        return true;
                    }
                }
            }
            result = null;
            return false;
        }
        
        private EcsFilter CreateNewFilter(int[] included, int[] excluded)
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

        private void RegisterFilter(EcsFilter filter, int[] componentIndices)
        {
            foreach (var index in componentIndices)
            {
                _typeToFilter[index].Add(filter);
            }
        }

        private void IncreaseFiltersRegistry()
        {
            while (_typeToFilter.Count < ComponentMeta.Count)
            {
                _typeToFilter.Add(new List<EcsFilter>());
            }
        }
    }
}