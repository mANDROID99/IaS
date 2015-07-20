using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IaS.GameState
{
    public class EventRegistry
    {
        private Dictionary<Type, Object> consumers = new Dictionary<Type, Object>();

        public void RegisterConsumer<E>(EventConsumer<E> consumer) where E : IEvent
        {
            Type eventType = typeof(E);
            Object consumersGeneric;
            List<EventConsumer<E>> consumers;
            if(!this.consumers.TryGetValue(eventType, out consumersGeneric))
            {
                consumers = new List<EventConsumer<E>>();
                this.consumers.Add(eventType, consumers);
            }
            else
            {
                consumers = (List<EventConsumer<E>>) consumersGeneric;
            }
            consumers.Add(consumer);
        }

        public void Notify<E>(E e) where E : IEvent
        {
            Object consumersGeneric;
            if(this.consumers.TryGetValue(typeof(E), out consumersGeneric)){
                List<EventConsumer<E>> consumers = (List<EventConsumer<E>>)consumersGeneric;
                foreach(EventConsumer<E> consumer in consumers)
                {
                    consumer.OnEvent(e);
                }
            }
        }
    }

    public interface EventConsumer<E> where E : IEvent
    {
        void OnEvent(E evt);
    }

    public interface IEvent
    {

    }
}
