using System;
using System.Threading;

namespace ECS.Runtime.Core
{
    public class ComponentType
    {
        internal static int Count;
        internal static event Action CountChanged;
        
        protected static void NotifyCountChanged() => CountChanged?.Invoke();
    }

    public sealed class ComponentType<T> : ComponentType
    {
        public static readonly int Index;
        
        static ComponentType()
        {
            Index = Count;
            Interlocked.Increment(ref Count);
            NotifyCountChanged();
        }
    }
}