using System.Collections.Generic;

namespace Lekret.Ecs.Extensions
{
    public struct SetListeners<T>
    {
        public List<ISetListener<T>> Value;
    }
}