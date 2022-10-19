using System.Threading;

namespace Lekret.Ecs.Internal
{
    internal class ComponentMeta
    {
        internal static int Count;
    }
    
    internal class ComponentMeta<T> : ComponentMeta
    {
        internal static readonly int Index;

        static ComponentMeta()
        {
            Index = Count;
            Interlocked.Increment(ref Count);
        }
    }
}