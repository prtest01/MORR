﻿using System.Collections.Generic;
using System.Composition;
using System.Text.Json;
using System.Threading.Tasks;
using MORR.Core.Data.Sample.Metadata;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy.SingleConsumer;

namespace MORR.Core.Data.Capture.Metadata
{
    [Export(typeof(IReadOnlyEventQueue<MetadataSample>))]
    public class MetadataSampleProducer : BoundedSingleConsumerEventQueue<MetadataSample>
    {
        [ImportMany]
        private IEnumerable<IReadOnlyEventQueue<Event>> EventQueues { get; set; }

        /// <summary>
        ///     Creates <see cref="MetadataSample" /> instances from <see cref="Event" /> instances using serialization to JSON.
        /// </summary>
        /// <param name="event">The <see cref="Event" /> instance to convert</param>
        /// <returns>The <see cref="MetadataSample" /> that the event was converted to</returns>
        private static MetadataSample MakeMetadataSample(Event @event)
        {
            var eventType = @event.GetType();
            var serializedData = JsonSerializer.SerializeToUtf8Bytes(@event);

            var sample = new MetadataSample
            {
                EventType = eventType,
                SerializedData = serializedData,
                IssuingModule = MetadataCapture.Identifier
            };

            return sample;
        }

        private async void LinkQueue(IReadOnlyEventQueue<Event> eventQueue)
        {
            await foreach (var @event in eventQueue.GetEvents())
            {
                var sample = MakeMetadataSample(@event);
                Enqueue(sample);
            }
        }

        /// <summary>
        ///     Initializes the producer.
        /// </summary>
        public void Initialize()
        {
            foreach (var eventQueue in EventQueues)
            {
                Task.Run(() => LinkQueue(eventQueue));
            }
        }
    }
}