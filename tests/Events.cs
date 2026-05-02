using TiredSiren.EventBus;

namespace tests
{
    public class Events
    {
        public record DataEvent : IEvent
        {
            public string Data { get; set; }
        }
    }
}