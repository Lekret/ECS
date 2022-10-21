namespace Lekret.Ecs
{
    public class EcsGame
    {
        public EcsGame(EcsSystems systems, EcsManager manager)
        {
            Systems = systems;
            Manager = manager;
        }

        public EcsSystems Systems { get; }
        public EcsManager Manager { get; }

        public EcsGame AddSystem(ISystem system)
        {
            Systems.Add(system);
            return this;
        }
    }
}