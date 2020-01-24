﻿using MORR.Shared.Events.Queue;
using MORR.Modules.WindowManagement.Events;
using System.ComponentModel.Composition;
using MORR.Shared.Events.Queue.Strategy.MultiConsumer;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowResizingEvent
    /// </summary>
    [Export(typeof(WindowResizingEventProducer))]
    [Export(typeof(IReadOnlyEventQueue<WindowResizingEvent>))]
    public class WindowResizingEventProducer : BoundedMultiConsumerEventQueue<WindowResizingEvent>
    {
        // TODO: Implement this
    }
}