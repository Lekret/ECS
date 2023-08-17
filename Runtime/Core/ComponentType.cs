using System.Threading;

namespace ECS.Runtime.Core
{
    public class ComponentType
    {
        internal static int Count;
    }

    public sealed class ComponentType<T> : ComponentType
    {
        public static readonly int Index;
        
        static ComponentType()
        {
            Index = Count;
            Interlocked.Increment(ref Count);
        }
    }
}