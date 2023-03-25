using System.Threading;

namespace Lekret.Ecs
{
    public class Component
    {
        internal static int Count;
    }

    public sealed class Component<T> : Component
    {
        public static readonly int Index;

        static Component()
        {
            Index = Count;
            Interlocked.Increment(ref Count);
        }
    }
}