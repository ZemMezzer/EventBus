using System;
using System.Collections.Generic;
using R3;
using TiredSiren.EventBus.EventChannels;

namespace TiredSiren.EventBus
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, EventChannel> _channelsContainer = new Dictionary<Type, EventChannel>();
        private readonly EventChannel _defaultEventChannel = new EventChannel();

        public EventChannel GetChannel<TChannel>() where TChannel : IEventChannel
        {
            if (_channelsContainer.TryGetValue(typeof(TChannel), out var channel)) 
                return channel;
            
            channel = new EventChannel();
            _channelsContainer[typeof(TChannel)] = channel;

            return channel;
        }

        public Observable<TEvent> Observe<TEvent>() where TEvent : IEvent
        {
            return _defaultEventChannel.Observe<TEvent>();
        }

        public void Broadcast<TEvent>(TEvent ev) where TEvent : IEvent
        {
            _defaultEventChannel.Publish(ev);
            
            foreach (var channelKeyValue in _channelsContainer)
            {
                var channel = channelKeyValue.Value;
                channel.Publish(ev);
            }
        }
    }
}