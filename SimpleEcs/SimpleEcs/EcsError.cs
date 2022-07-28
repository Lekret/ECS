using System;

namespace SimpleEcs
{
    public static class EcsError
    {
        public static Action<string> OnError { get; set; } = message => throw new Exception(message);
        
        internal static void Handle(string message)
        {
            OnError?.Invoke(message);
        }
    }
}