namespace Lekret.Ecs.Extensions
{
    public static class EcsGameExtensions
    {
        public static EcsGame NotifyOnSet<T>(this EcsGame game)
        {
            return game.AddSystem(new SetSelfEventSystem<T>(game.Manager));
        }

        public static EcsGame NotifyAllOnSet<T>(this EcsGame game)
        {
            return game.AddSystem(new SetAllEventSystem<T>(game.Manager));
        }
        
        public static EcsGame NotifyOnRemove<T>(this EcsGame game)
        {
            return game.AddSystem(new RemoveSelfEventSystem<T>(game.Manager));
        }
        
        public static EcsGame NotifyAllOnRemove<T>(this EcsGame game)
        {
            return game.AddSystem(new RemoveAllEventSystem<T>(game.Manager));
        }

        public static EcsGame Remove<T>(this EcsGame game)
        {
            return game.AddSystem(new RemoveComponentSystem<T>(game.Manager));
        }
    }
}