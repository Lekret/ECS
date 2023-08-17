namespace ECS.Runtime.Core
{
    public struct EcsConfig
    {
        public static readonly EcsConfig Default = new EcsConfig
        {
            InitialEntityCapacity = 512,
        };

        public int InitialEntityCapacity;
    }
}