using System.Collections.Generic;
using EcsLib.Internal;

namespace EcsLib.Api.Invariants.Impl
{
    public class NeverTogether : IEcsAddInvariant
    {
        private readonly HashSet<int> _indices = new HashSet<int>();

        public NeverTogether Inc<T>()
        {
            _indices.Add(ComponentMeta<T>.Index);
            return this;
        }

        public bool CanAdd(Entity entity, int componentIndex)
        {
            return !_indices.Contains(componentIndex);
        }
    }
}