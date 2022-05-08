namespace ECSimplicity.Extras
{
    public class DestroyEntitySystem<T> : IEcsTickSystem
    {
        private readonly EcsFilter _filter;

        public DestroyEntitySystem(EcsAdmin admin)
        {
            _filter = admin.Filter().Inc<T>().End();
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