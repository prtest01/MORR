﻿using System.Collections.Generic;

namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a read-only queue for <see cref="Event" /> types intended for decoding.
    /// </summary>
    /// <typeparam name="T">The type of the event</typeparam>
    public interface IDecodableEventQueue<out T> where T : Event
    {
        /// <summary>
        /// Describes whether the queue is currently enabled to queue new events or not.
        /// </summary>
        public bool IsClosed { get; }

        /// <summary>
        ///     Asynchronously gets all events as concrete type <typeparamref name="T" />.
        /// </summary>
        /// <returns>A stream of <typeparamref name="T" /></returns>
        IAsyncEnumerable<T> GetEvents();
    }
}