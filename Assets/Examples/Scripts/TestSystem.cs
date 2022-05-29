using LkEcs;

namespace Examples.Scripts
{
    public struct Health
    {
        public int Value;
    }

    public class TestSystem : IEcsInitSystem, IEcsTickSystem
    {
        private readonly EcsAdmin _admin;
        private readonly EcsFilter _filter;

        public TestSystem(EcsAdmin admin)
        {
            _admin = admin;
            _filter = admin.Filter()
                .Inc<Health>()
                .End();
        }
        
        public void Init()
        {
            _admin.CreateEntity().Set(new Health {Value = 100});
        }

        public void Tick()
        {
            foreach (var entity in _filter)
            {
                var health = entity.Get<Health>();
                health.Value += 5;
                entity.Set(health);
            }
        }
    }
}