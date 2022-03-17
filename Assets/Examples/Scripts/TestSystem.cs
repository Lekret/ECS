using System.Linq;
using EcsLib.Api;
using UnityEngine;

namespace Examples.Scripts
{
    public struct Health
    {
        public int Value;
    }

    public struct Duration
    {
        public int Value;
    }

    public class TestSystem : IEcsTickSystem
    {
        private readonly EcsFilter _filter;

        public TestSystem(EcsManager manager)
        {
            _filter = EcsFilter.Create()
                .Inc<Health>()
                .Inc<Duration>()
                .End();

            foreach (var _ in Enumerable.Range(0, 10000))
            {
                Entity.Create()
                    .Set(new Health
                    {
                        Value = 100
                    })
                    .Set(new Duration
                    {
                        Value = 25
                    });
            }
        }

        public void Tick()
        {
            foreach (var entity in _filter)
            {
                Debug.Log(entity.Remove<Health>());
                Debug.LogError("YES " + entity.Get<Health>());
            }
        }
    }
}