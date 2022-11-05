using System.Collections.Generic;

namespace Lekret.Ecs
{
    public sealed class MaskBuilder
    {
        private readonly List<int> _indices = new List<int>();

        public List<int> Indices => _indices;

        public MaskBuilder With<T>()
        {
            _indices.Add(Component<T>.Index);
            return this;
        }
    }
}