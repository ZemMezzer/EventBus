using NUnit.Framework;
using R3;
using TiredSiren.EventBus;

namespace tests
{
    [TestFixture]
    public class Tests
    {
        private const string DataResult = "Some Data";
        
        [Test]
        public void Observe()
        {
            var result = string.Empty;
            var eventBus = new EventBus();
            eventBus.Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result = ev.Data;
            });
            eventBus.Publish(new Events.DataEvent { Data = DataResult });
            
            Assert.AreEqual(DataResult, result);
        }
        
        [Test]
        public void ObserveMany()
        {
            var result = string.Empty;
            var result2 = string.Empty;
            var result3 = string.Empty;
            
            var eventBus = new EventBus();
            eventBus.Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result = ev.Data;
            });
            
            eventBus.Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result2 = ev.Data;
            });
            
            eventBus.Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result3 = ev.Data;
            });
            
            eventBus.Publish(new Events.DataEvent { Data = DataResult });
            
            Assert.AreEqual(DataResult, result);
            Assert.AreEqual(DataResult, result2);
            Assert.AreEqual(DataResult, result3);
        }
        
        [Test]
        public void Dispose()
        {
            var result = string.Empty;
            var eventBus = new EventBus();
            eventBus.Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result = ev.Data;
            }).Dispose();
            
            eventBus.Publish(new Events.DataEvent { Data = DataResult });
            Assert.AreEqual(string.Empty, result);
        }
        
        [Test]
        public void DisposeMany()
        {
            var result = string.Empty;
            var result2 = string.Empty;
            var result3 = string.Empty;
            
            var eventBus = new EventBus();
            eventBus.Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result = ev.Data;
            });
            
            eventBus.Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result2 = ev.Data;
            }).Dispose();
            
            eventBus.Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result3 = ev.Data;
            });
            
            eventBus.Publish(new Events.DataEvent { Data = DataResult });
            
            Assert.AreEqual(DataResult, result);
            Assert.AreEqual(string.Empty, result2);
            Assert.AreEqual(DataResult, result3);
        }
    }
}