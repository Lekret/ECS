using System;
using UnityEngine;

namespace LkEcs
{
    public static class EcsError
    {
        public static Action<string> OnError { get; set; } = Debug.LogError;
        
        internal static void Handle(string message)
        {
            OnError?.Invoke(message);
        }
    }
}