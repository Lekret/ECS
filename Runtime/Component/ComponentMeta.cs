using System.Threading;

namespace Lekret.Ecs.Internal
{
    public class ComponentMeta
    {
        internal static int Count;
    }
    
    public sealed class ComponentMeta<T> : ComponentMeta
    {
        public static readonly int Index;

        static ComponentMeta()
        {
            Index = Count;
            Interlocked.Increment(ref Count);
        }
    }
}