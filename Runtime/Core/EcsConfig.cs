namespace ECS.Runtime.Core
{
    public struct EcsConfig
    {
        public static readonly EcsConfig Default = new EcsConfig
        {
            InitialComponentsCapacity = 128,
            InitialEntityCapacity = 512,
        };

        public int InitialComponentsCapacity;
        public int InitialEntityCapacity;
    }
}