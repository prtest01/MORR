﻿using System;
using System.Collections.Generic;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for <see cref="Event" /> types.
    /// </summary>
    /// <typeparam name="T">The type of the event</typeparam>
    public interface ITranscodeableEventQueue<T> where T : Event
    {
        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="T" />.
        /// </summary>
        /// <returns>A stream of <typeparamref name="T" /></returns>
        IAsyncEnumerable<T> GetEvents();

        /// <summary>
        ///     The actual type of the events enqueued in this queue.
        /// </summary>
        Type EventType => typeof(T);

        /// <summary>
        ///     Asynchronously enqueues a new event.
        /// </summary>
        /// <param name="event">The event to enqueue</param>
        void Enqueue(T @event);
    }
}