using System.Collections.Generic;

namespace Lekret.Ecs.Extensions
{
    public class RemovedListeners<T>
    {
        public List<IRemovedListener<T>> Value;
    }
}