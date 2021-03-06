﻿using IaS.GameState.Rotation;
using System;
using System.Collections.Generic;

namespace IaS.GameState
{
    public class EventRegistry
    {
        private readonly Dictionary<Type, object> _consumers = new Dictionary<Type, object>();


        public void RegisterConsumer<E>(EventConsumer<E> consumer) where E : IEvent
        {
            Type eventType = typeof(E);

            object genericList = null;
            if(!_consumers.TryGetValue(eventType, out genericList))
            {
                genericList = new List<EventConsumer<E>>();
                _consumers.Add(eventType, genericList);
            }

            ((List<EventConsumer<E>>)(genericList)).Add(consumer);
        }

        public void Notify<E>(E e) where E : IEvent
        {
            object consumersGeneric;
            if(_consumers.TryGetValue(typeof(E), out consumersGeneric)){
                var consumers = (List<EventConsumer<E>>) consumersGeneric;
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
