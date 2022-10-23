using System.Collections.Generic;
using Lekret.Ecs.Internal;

namespace SimpleEcs.Runtime
{
    public class MaskBuilder
    {
        private readonly List<int> _indices = new List<int>();

        public List<int> Indices => _indices;

        public MaskBuilder With<T>()
        {
            _indices.Add(ComponentMeta<T>.Index);
            return this;
        }
    }
}