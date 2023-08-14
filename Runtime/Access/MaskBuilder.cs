using System.Collections.Generic;
using ECS.Runtime.Core;

namespace ECS.Runtime.Access
{
    public sealed class MaskBuilder
    {
        private readonly List<int> _indices = new List<int>();

        public List<int> Indices => _indices;

        public MaskBuilder With<T>()
        {
            _indices.Add(ComponentType<T>.Index);
            return this;
        }
    }
}