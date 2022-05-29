namespace PlainEcs.Extras
{
    public class DestroyComponentSystem<T> : IEcsTickSystem
    {
        private readonly EcsFilter _filter;

        public DestroyComponentSystem(EcsAdmin admin)
        {
            _filter = admin.Filter().Inc<T>().End();
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