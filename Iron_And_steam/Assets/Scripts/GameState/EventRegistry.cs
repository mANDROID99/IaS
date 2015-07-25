﻿using System;
using System.Collections.Generic;

namespace IaS.GameState
{
    public class EventRegistry
    {
        private readonly Dictionary<Type, object> _consumers = new Dictionary<Type, object>();

        public void RegisterConsumer<E>(EventConsumer<E> consumer) where E : IEvent
        {
            Type eventType = typeof(E);
            object consumersGeneric;
            List<EventConsumer<E>> consumers;
            if(!this._consumers.TryGetValue(eventType, out consumersGeneric))
            {
                consumers = new List<EventConsumer<E>>();
                this._consumers.Add(eventType, consumers);
            }
            else
            {
                consumers = (List<EventConsumer<E>>) consumersGeneric;
            }
            consumers.Add(consumer);
        }

        public void Notify<E>(E e) where E : IEvent
        {
            object consumersGeneric;
            if(this._consumers.TryGetValue(typeof(E), out consumersGeneric)){
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
