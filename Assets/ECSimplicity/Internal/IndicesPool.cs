using System.Collections.Generic;

namespace ECSimplicity.Internal
{
    internal static class IndicesPool
    {
        private static readonly Queue<EcsIndices> Pool = new Queue<EcsIndices>();

        internal static EcsIndices Get()
        {
            if (Pool.Count > 0)
                return Pool.Dequeue();
            return new EcsIndices();
        }

        internal static void Release(EcsIndices indices)
        {
            indices.Clear();
            Pool.Enqueue(indices);
        }
    }
}