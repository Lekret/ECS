namespace ECSimplicity.Api.Extras
{
    public class DestroyComponentSystem<T> : IEcsTickSystem
    {
        private readonly EcsFilter _filter;

        public DestroyComponentSystem(EcsManager manager)
        {
            _filter = manager.Filter().Inc<T>().End();
        }

        public void Tick()
        {
            foreach (var entity in _filter)
            {
                entity.Remove<T>();
            }
        }
    }
}