using EcsLib.Api;

namespace Examples.Scripts
{
    public interface ILogger
    {
        void Info(string message);
        void Error(string message);
    }
    
    [EcsSingleton]
    public class Logger : ILogger
    {
        public void Info(string message)
        {
        }

        public void Error(string message)
        {
        }
    }
}