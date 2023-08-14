using System;
using System.Collections;
using System.Collections.Generic;
using ECS.Runtime.Core;

namespace ECS.Runtime.Access.Collector
{
    public sealed class Collector : IEnumerable<Entity>, IDisposable
    {
        private readonly HashSet<Entity> _entities;
        private readonly Filter[] _filters;
        private readonly FilterEvent[] _filterEvents;
        private readonly Action<Entity> _cachedAdd;

        public Collector(Filter[] filters, FilterEvent[] filterEvents)
        {
            _filters = filters;
            _entities = new HashSet<Entity>();
            _filterEvents = filterEvents;

            if (filters.Length != filterEvents.Length)
                throw new Exception(
                    $"Filters ({filters.Length}) and filter events ({filterEvents.Length}) must be equal");

            _cachedAdd = e => _entities.Add(e);
            ObserveFilter();
        }

        public int Count => _entities.Count;

        public void Clear()
        {
            _entities.Clear();
        }

        public EntityEnumerator GetEnumerator()
        {
            return EntityEnumerator.Create(_entities);
        }

        IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            for (var i = 0; i < _filters.Length; i++)
            {
                var filter = _filters[i];
                filter.EntityAdded -= _cachedAdd;
                filter.EntityRemoved -= _cachedAdd;
            }

            Clear();
        }

        private void ObserveFilter()
        {
            for (var i = 0; i < _filters.Length; i++)
            {
                var filter = _filters[i];
                var filterEvent = _filterEvents[i];
                switch (filterEvent)
                {
                    case FilterEvent.Set:
                        filter.EntityAdded -= _cachedAdd;
                        filter.EntityAdded += _cachedAdd;
                        break;
                    case FilterEvent.Removed:
                        filter.EntityRemoved -= _cachedAdd;
                        filter.EntityRemoved += _cachedAdd;
                        break;
                    case FilterEvent.SetOrRemoved:
                        filter.EntityAdded -= _cachedAdd;
                        filter.EntityAdded += _cachedAdd;
                        filter.EntityRemoved -= _cachedAdd;
                        filter.EntityRemoved += _cachedAdd;
                        break;
                }
            }
        }
    }

    public static class CollectorExtensions
    {
        public static Collector Collector(
            this EcsManager manager,
            params TriggerOnEvent[] triggers)
        {
            var filters = new Filter[triggers.Length];
            var filterEvents = new FilterEvent[triggers.Length];

            for (var i = 0; i < triggers.Length; i++)
            {
                filters[i] = manager.Filter(triggers[i].Mask);
                filterEvents[i] = triggers[i].FilterEvent;
            }

            return new Collector(filters, filterEvents);
        }
    }
}