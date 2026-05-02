using System;
using System.Collections.Generic;
using R3;

namespace TiredSiren.EventBus
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<object>> _observersContainer = new Dictionary<Type, List<object>>();
        
        public Observable<T> Observe<T>() where T : IEvent
        {
            return Observable.Create<T>(observer =>
            {
                if (!_observersContainer.TryGetValue(typeof(T), out var observers))
                {
                    observers = new List<object>();
                    _observersContainer[typeof(T)] = observers;
                }
                
                observers.Add(observer);
                
                return Disposable.Create(() =>
                {
                    observers.Remove(observer);
                    
                    if(observers.Count <= 0)
                        _observersContainer.Remove(typeof(T));
                });
            });
        }

        public void Publish<T>(T ev) where T : IEvent
        {
            if (!_observersContainer.TryGetValue(typeof(T), out var observers))
                return;

            for (var i = observers.Count - 1; i >= 0; i--)
            {
                var observer = (Observer<T>)observers[i];

                try
                {
                    observer.OnNext(ev);
                }
                catch (Exception e)
                {
                    observer.OnErrorResume(e);
                }
            }
        }
    }
}