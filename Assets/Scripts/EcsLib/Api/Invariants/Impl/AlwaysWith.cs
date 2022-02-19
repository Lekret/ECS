using System.Collections.Generic;
using EcsLib.Internal;

namespace EcsLib.Api.Invariants.Impl
{
    public class AlwaysWith<TMain> : IEcsRemoveInvariant
    {
        private readonly int _mainIndex;
        private readonly HashSet<int> _indices = new HashSet<int>();

        public AlwaysWith()
        {
            _mainIndex = ComponentMeta<TMain>.Index;
        }

        public AlwaysWith<TMain> ShouldBe<T>()
        {
            _indices.Add(ComponentMeta<T>.Index);
            return this;
        }

        public bool CanRemove(Entity entity, int componentIndex)
        {
            return componentIndex == _mainIndex && !_indices.Contains(componentIndex);
        }
    }
}