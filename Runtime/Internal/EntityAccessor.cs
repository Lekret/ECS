using System.Collections.Generic;
using System.Linq;

namespace Lekret.Ecs.Internal
{
    internal sealed class EntityAccessor
    {
        private readonly List<List<Filter>> _typeToFilter = new List<List<Filter>>();
        private readonly World _world;

        internal EntityAccessor(World world)
        {
            _world = world;
        }

        internal FilterBuilder CreateFilterBuilder()
        {
            return new FilterBuilder(this);
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

        internal Filter GetFilter(List<int> included, List<int> excluded)
        {
            foreach (var filters in _typeToFilter)
            {
                foreach (var filter in filters)
                {
                    if (filter.MatchesIndices(included, excluded))
                        return filter;
                }
            }
            
            return CreateNewFilter(included.ToArray(), excluded.ToArray());
        }

        private Filter CreateNewFilter(int[] included, int[] excluded)
        {
            var filter = new Filter(included, excluded);
            IncreaseFiltersRegistry();
            RegisterFilter(filter, included);
            RegisterFilter(filter, excluded);
            foreach (var entity in _world.Entities)
            {
                filter.HandleEntity(entity);
            }
            return filter;
        }

        private void RegisterFilter(Filter filter, int[] componentIndices)
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
                _typeToFilter.Add(new List<Filter>());
            }
        }
    }
}