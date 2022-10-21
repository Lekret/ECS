namespace Lekret.Ecs.Extensions
{
    public static class EcsGameExtensions
    {
        public static EcsGame NotifySet<T>(this EcsGame game)
        {
            return game.AddSystem(new SetEventSystem<T>(game.Manager));
        }

        public static EcsGame NotifyRemove<T>(this EcsGame game)
        {
            return game.AddSystem(new RemoveEventSystem<T>(game.Manager));
        }

        public static EcsGame Remove<T>(this EcsGame game)
        {
            return game.AddSystem(new RemoveComponentSystem<T>(game.Manager));
        }
    }
}