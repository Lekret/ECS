using System;
using System.Collections.Generic;

namespace Lekret.Ecs.Internal
{
    internal class FilterIndices
    {
        private readonly List<int> _included = new List<int>();
        private readonly List<int> _excluded = new List<int>();
        public List<int> Included => _included;
        public List<int> Excluded => _excluded;

        internal void Inc<T>()
        {
            var index = ComponentMeta<T>.Index;
            if (_included.Contains(index))
                throw new Exception($"Type is already included: {typeof(T)}");
            
            if (_excluded.Contains(index))
                throw new Exception($"Can't include excluded type: {typeof(T)}");
            
            _included.Add(index);
        }

        internal void Exc<T>()
        {
            var index = ComponentMeta<T>.Index;
            if (_excluded.Contains(index))
                throw new Exception($"Type is already excluded: {typeof(T)}");

            if (_included.Contains(index))
                throw new Exception($"Can't exclude included type: {typeof(T)}");

            _excluded.Add(index);
        }
        
        internal void Clear()
        {
            _included.Clear();
            _excluded.Clear();
        }
    }
}