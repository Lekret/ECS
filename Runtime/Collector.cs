using System;
using System.Collections;
using System.Collections.Generic;
using SimpleEcs.Runtime;

namespace Lekret.Ecs
{
    public sealed class Collector : IEnumerable<Entity>
    {
        private readonly HashSet<Entity> _entities;
        private readonly Filter[] _filters;
        
        public Collector(Filter[] filters, FilterEvent[] filterEvents)
        {
            _filters = filters;
            _entities = new HashSet<Entity>();

            if (filters.Length != filterEvents.Length)
                throw new Exception($"Filters ({filters.Length}) and filter events ({filterEvents.Length}) must be equal");

            ObserveFilter(filterEvents, AddEntity);
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

        private void ObserveFilter(FilterEvent[] filterEvents, Action<Entity> addEntity)
        {
            for (var i = 0; i < _filters.Length; i++)
            {
                var filter = _filters[i];
                var filterEvent = filterEvents[i];
                switch (filterEvent)
                {
                    case FilterEvent.Set:
                        filter.EntityAdded -= addEntity;
                        filter.EntityAdded += addEntity;
                        break;
                    case FilterEvent.Removed:
                        filter.EntityRemoved -= addEntity;
                        filter.EntityRemoved += addEntity;
                        break;
                    case FilterEvent.SetOrRemoved:
                        filter.EntityAdded -= addEntity;
                        filter.EntityAdded += addEntity;
                        filter.EntityRemoved -= addEntity;
                        filter.EntityRemoved += addEntity;
                        break;
                }
            }
        }

        private void AddEntity(Entity entity)
        {
            _entities.Add(entity);
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