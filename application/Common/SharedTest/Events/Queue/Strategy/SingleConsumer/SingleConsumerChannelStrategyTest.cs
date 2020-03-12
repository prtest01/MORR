﻿using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events.Queue.Strategy;

namespace SharedTest.Events.Queue.Strategy.SingleConsumer
{
    public abstract class SingleConsumerChannelStrategyTest<T> : EventQueueStorageStrategyTest<T> where T: IEventQueueStorageStrategy<TestEvent>
    {
        private const int maxWaitTime = 1000;

        [TestMethod]
        public void TestBoundedSingleConsumer_IsOccupied()
        {
            /* PRECONDITION */
            Debug.Assert(Strategy != null);

            var allowedConsumerDidNotFailed = new ManualResetEvent(true);
            var invalidConsumerFailed = new ManualResetEvent(false);

            /* GIVEN */
            var consumer = new TestConsumer(Strategy);
            consumer.Consume(
                false,
                (@event, index) => true,
                result => result?.EventSuccess(allowedConsumerDidNotFailed));

            /* WHEN */
            var invalidConsumer = new TestConsumer(Strategy);
            invalidConsumer.Consume(
                true,
                (@event, num) => true,
                result => result?.EventThrows<ChannelConsumingException>(invalidConsumerFailed));


            /* THEN */
            Assert.IsTrue(allowedConsumerDidNotFailed.WaitOne(maxWaitTime), "An Error occurred while consuming using valid consumers.");
            Assert.IsTrue(invalidConsumerFailed.WaitOne(maxWaitTime), "InvalidConsumer should fail.");
        }
    }
}