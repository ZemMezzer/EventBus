using R3;
using TiredSiren.EventBus.EventChannels;

namespace TiredSiren.EventBus
{
    public interface IEventBus
    {
        EventChannel GetChannel<TChannel>() where TChannel : IEventChannel;
        Observable<TEvent> Observe<TEvent>() where TEvent : IEvent;
        void Broadcast<TEvent>(TEvent ev) where TEvent : IEvent;
    }
}