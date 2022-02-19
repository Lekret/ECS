using System.Collections.Generic;
using EcsLib.Internal;

namespace EcsLib.Api.Invariants.Impl
{
    public class AlwaysTogether : IEcsRemoveInvariant
    {
        private readonly HashSet<int> _indices = new HashSet<int>();

        public AlwaysTogether Inc<T>()
        {
            _indices.Add(ComponentMeta<T>.Index);
            return this;
        }

        public bool CanRemove(Entity entity, int componentIndex)
        {
            return !_indices.Contains(componentIndex);
        }
    }
}