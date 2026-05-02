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
            eventBus.Broadcast(new Events.DataEvent { Data = DataResult });
            
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
            
            eventBus.Broadcast(new Events.DataEvent { Data = DataResult });
            
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
            
            eventBus.Broadcast(new Events.DataEvent { Data = DataResult });
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
            
            eventBus.Broadcast(new Events.DataEvent { Data = DataResult });
            
            Assert.AreEqual(DataResult, result);
            Assert.AreEqual(string.Empty, result2);
            Assert.AreEqual(DataResult, result3);
        }

        [Test]
        public void SingleChannelTest()
        {
            var result = string.Empty;
            var eventBus = new EventBus();
            
            eventBus.GetChannel<TestChannel1>().Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result = ev.Data;
            }).Dispose();
            
            eventBus.GetChannel<TestChannel1>().Publish(new Events.DataEvent { Data = DataResult });
            Assert.AreEqual(string.Empty, result);
        }
        
        [Test]
        public void ManyChannelsTest()
        {
            var result = string.Empty;
            const string testData = "Some Random Data";
            var eventBus = new EventBus();
            
            eventBus.GetChannel<TestChannel1>().Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result = testData;
            });
            
            eventBus.GetChannel<TestChannel2>().Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result = ev.Data;
            });
            
            eventBus.GetChannel<TestChannel2>().Publish(new Events.DataEvent { Data = DataResult });
            Assert.AreEqual(DataResult, result);
        }
        
        [Test]
        public void ManyChannelsTest2()
        {
            var result = string.Empty;
            const string testData = "Some Random Data";
            var eventBus = new EventBus();
            
            eventBus.GetChannel<TestChannel1>().Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result = testData;
            });
            
            eventBus.GetChannel<TestChannel2>().Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result = ev.Data;
            });
            
            eventBus.GetChannel<TestChannel1>().Publish(new Events.DataEvent { Data = DataResult });
            Assert.AreEqual(testData, result);
        }
        
        [Test]
        public void ManyChannelsTest3()
        {
            var result = string.Empty;
            var eventBus = new EventBus();
            
            eventBus.GetChannel<TestChannel1>().Observe<Events.DataEvent>().Subscribe(ev =>
            {
                result = DataResult;
            });
            
            eventBus.GetChannel<TestChannel2>().Publish(new Events.DataEvent { Data = DataResult });
            Assert.AreEqual(string.Empty, result);
        }
    }
}