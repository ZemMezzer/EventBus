using System;
using R3;

namespace TiredSiren.EventBus
{
    public interface IEventBus
    {
        Observable<T> Observe<T>() where T : IEvent;
        void Publish<T>(T ev) where T : IEvent;
    }
}