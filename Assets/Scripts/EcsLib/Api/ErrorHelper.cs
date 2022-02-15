using System;
using UnityEngine;

namespace EcsLib.Api
{
    public static class ErrorHelper
    {
        public static Action<string> OnError { get; set; } = Debug.LogError;
        
        internal static void Handle(string message)
        {
            OnError?.Invoke(message);
        }
    }
}