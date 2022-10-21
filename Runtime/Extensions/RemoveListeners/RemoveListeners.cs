using System.Collections.Generic;

namespace Lekret.Ecs.Extensions
{
    public class RemoveListeners<T>
    {
        public List<IRemoveListener<T>> Value;
    }
}