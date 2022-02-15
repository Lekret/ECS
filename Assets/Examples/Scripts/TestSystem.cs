using System;
using EcsLib.Api;
using UnityEngine;

namespace Example
{
    public struct Health
    {
        public int Value;
    }

    public class TestSystem : IEcsInitSystem, IEcsTickSystem
    {
        private readonly EcsFilter _filter;
        private readonly EcsFilter _filterN;
        private readonly ILogger _logger;

        public TestSystem(EcsManager manager)
        {
            _logger = manager.Get<ILogger>();
            _filter = manager.Filter()
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
            foreach (var enti in _filterN)
            {
                foreach (var entity in _filter)
                {
                    Debug.Log($"Count: {_filter.Count}, should be 1. Value: {entity.Get<Health>().Value}");
                }
            }
        }
    }
}