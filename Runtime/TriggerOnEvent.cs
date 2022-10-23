using Lekret.Ecs;

namespace SimpleEcs.Runtime
{
    public struct TriggerOnEvent
    {
        public readonly IMask Mask;
        public readonly FilterEvent FilterEvent;

        public TriggerOnEvent(IMask mask, FilterEvent filterEvent)
        {
            Mask = mask;
            FilterEvent = filterEvent;
        }
    }
    
    public static class TriggerOnEventExtensions
    {
        public static TriggerOnEvent Set(this IMask mask)
        {
            return new TriggerOnEvent(mask, FilterEvent.Set);
        }

        public static TriggerOnEvent Removed(this IMask mask)
        {
            return new TriggerOnEvent(mask, FilterEvent.Removed);
        }

        public static TriggerOnEvent SetOrRemoved(this IMask mask)
        {
            return new TriggerOnEvent(mask, FilterEvent.SetOrRemoved);
        }
        
        public static TriggerOnEvent Set(this MaskBuilder builder)
        {
            return new TriggerOnEvent(Mask.AllOf(builder), FilterEvent.Set);
        }
        
        public static TriggerOnEvent Removed(this MaskBuilder builder)
        {
            return new TriggerOnEvent(Mask.AllOf(builder), FilterEvent.Removed);
        }
        
        public static TriggerOnEvent SetOrRemoved(this MaskBuilder builder)
        {
            return new TriggerOnEvent(Mask.AllOf(builder), FilterEvent.SetOrRemoved);
        }
    }
}