namespace PlainEcs.Internal
{
    internal class ComponentMeta
    {
        internal static int Count { get; private protected set; }
    }
    
    internal class ComponentMeta<T> : ComponentMeta
    {
        internal static readonly int Index;

        static ComponentMeta()
        {
            Index = Count;
            Count++;
        }
    }
}