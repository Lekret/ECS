using System.Collections.Generic;

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

        internal Filter GetFilter(IMask mask)
        {
            foreach (var filters in _typeToFilter)
            {
                foreach (var filter in filters)
                {
                    if (filter.Mask.Equals(mask))
                        return filter;
                }
            }
            return CreateNewFilter(mask);
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

        private Filter CreateNewFilter(IMask mask)
        {
            var filter = new Filter(mask);
            IncreaseFiltersRegistry();
            RegisterFilter(filter);
            foreach (var entity in _world.Entities)
            {
                filter.HandleEntity(entity);
            }
            return filter;
        }

        private void RegisterFilter(Filter filter)
        {
            foreach (var index in filter.Mask.Indices)
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