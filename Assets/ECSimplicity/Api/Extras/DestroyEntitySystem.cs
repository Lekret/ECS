namespace ECSimplicity.Api.Extras
{
    public class DestroyEntitySystem<T> : IEcsTickSystem
    {
        private readonly EcsFilter _filter;

        public DestroyEntitySystem(EcsManager manager)
        {
            _filter = manager.Filter().Inc<T>().End();
        }

        public void Tick()
        {
            foreach (var entity in _filter)
            {
                entity.Destroy();
            }
        }
    }
}