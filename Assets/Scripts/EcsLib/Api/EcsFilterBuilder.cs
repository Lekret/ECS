using System.Collections.Generic;
using EcsLib.Internal;

namespace EcsLib.Api
{
    public readonly struct EcsFilterBuilder
    {
        private readonly EcsAccessHandler _accessHandler;
        private readonly List<int> _includedIndices;
        private readonly List<int> _excludedIndices;

        internal EcsFilterBuilder(EcsAccessHandler accessHandler)
        {
            _accessHandler = accessHandler;
            _includedIndices = new List<int>();
            _excludedIndices = new List<int>();
        }

        public EcsFilterBuilder Inc<T>()
        {
            var index = ComponentMeta<T>.Index;
            if (_excludedIndices.Contains(index))
            {
                Error.Handle($"Can't include already excluded type {typeof(T)}");
                return this;
            }
            _includedIndices.Add(index);
            return this;
        }

        public EcsFilterBuilder Exc<T>()
        {
            var index = ComponentMeta<T>.Index;
            if (_includedIndices.Contains(index))
            {
                Error.Handle($"Can't exclude already included type {typeof(T)}");
                return this;
            }
            _excludedIndices.Add(index);
            return this;
        }
        
        public EcsFilter End()
        {
            return _accessHandler.InternalBuildFilter(_includedIndices, _excludedIndices);
        }
    }
}