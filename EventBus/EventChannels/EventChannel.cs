using System;
using System.Collections.Generic;
using R3;

namespace TiredSiren.EventBus.EventChannels
{
    public class EventChannel
    {
        private readonly Dictionary<Type, List<object>> _observersContainer = new Dictionary<Type, List<object>>();
        public Observable<TEvent> Observe<TEvent>() where TEvent : IEvent
        {
            return Observable.Create<TEvent>(observer =>
            {
                if (!_observersContainer.TryGetValue(typeof(TEvent), out var observers))
                {
                    observers = new List<object>();
                    _observersContainer[typeof(TEvent)] = observers;
                }
                
                observers.Add(observer);
                
                return Disposable.Create(() =>
                {
                    observers.Remove(observer);
                    
                    if(observers.Count <= 0)
                        _observersContainer.Remove(typeof(TEvent));
                });
            });
        }

        public void Publish<TEvent>(TEvent ev) where TEvent : IEvent
        {
            if (!_observersContainer.TryGetValue(typeof(TEvent), out var observers))
                return;
            
            var snapshot = observers.ToArray();

            foreach (var observer in snapshot)
            {
                try
                {
                    ((Observer<TEvent>)observer).OnNext(ev);
                }
                catch (Exception e)
                {
                    ((Observer<TEvent>)observer).OnErrorResume(e);
                }
            }
        }
    }
}