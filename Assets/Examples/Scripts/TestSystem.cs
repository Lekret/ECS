using EcsLib.Api;
using UnityEngine;

namespace Examples.Scripts
{
    public struct Health
    {
        public int Value;
    }

    public class TestSystem : IEcsInitSystem, IEcsTickSystem
    {
        private readonly EcsFilter _filter;
        private readonly EcsFilter _filterN;

        public TestSystem(EcsManager manager)
        {
            _filter = EcsFilter.Create()
                .Inc<Health>()
                .Exc<GameObject>()
                .End();
            _filterN = EcsFilter.Create()
                .Inc<Health>()
                .End();
        }
    
        public void Init()
        {
            Entity.Create()
                .Set(new GameObject("Entity").transform)
                .Set(new Health { Value = 25 });
            Entity.Create()
                .Set(new GameObject("Singleton").transform)
                .Set(new Health());
            Entity.Create()
                .Set(new Health { Value = 23 });
        }

        public void Tick()
        {
            
        }
    }
}