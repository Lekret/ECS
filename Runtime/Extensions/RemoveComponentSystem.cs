namespace Lekret.Ecs.Extensions
{
    public class RemoveComponentSystem<T> : IUpdateSystem
    {
        private readonly Filter _filter;

        public RemoveComponentSystem(EcsManager manager)
        {
            _filter = manager.Inc<T>().End();
        }
        
        public void Update()
        {
            foreach (var entity in _filter)
            {
                entity.Remove<T>();
            }
        }
    }
}