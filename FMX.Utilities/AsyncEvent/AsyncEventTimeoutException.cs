using System;
using System.Collections.Generic;
using System.Text;

namespace FMX.Utilities
{
    /// <summary>
    /// Abstract Class for <see cref="AsyncEventHandler{TSender, TArgs}"/>, allowing using instances without know the instances type params.
    /// </summary>
    public abstract class AsyncEventTimeoutException : Exception
    {
        /// <summary>
        /// Event that invoked timoute
        /// </summary>
        public AsyncEvent Event { get; }

        /// <summary>
        /// Handler that caused timeout
        /// </summary>
        public AsyncEventHandler<object, AsyncEventArgs> Handler { get; }


        private protected AsyncEventTimeoutException(AsyncEvent asyncEvent, AsyncEventHandler<object, AsyncEventArgs> eventHandler, string message) : base(message)
        {
            Event = asyncEvent;
            Handler = eventHandler;
        }
    }

    /// <summary>
    /// Thrown when <see cref="AsyncEventHandler{TSender, TArgs}"/> exceeds max time allowed.
    /// </summary>
    /// <typeparam name="TSender">Type of object that dispatched async event.</typeparam>
    /// <typeparam name="TArgs">Type of arguments for the async event.</typeparam>
    public class AsyncEventTimeoutException<TSender, TArgs> : AsyncEventTimeoutException where TArgs : AsyncEventArgs
    {
        /// <summary>
        /// Get event that caused the timeout
        /// </summary>
        public new AsyncEvent<TSender, TArgs> Event => base.Event as AsyncEvent<TSender, TArgs>;

        /// <summary>
        /// Get handler that caused timeoute
        /// </summary>
        public new AsyncEventHandler<TSender, TArgs> Handler => base.Handler as AsyncEventHandler<TSender, TArgs>;

        /// <summary>
        /// Create timeout exception
        /// </summary>
        /// <param name="asyncEvent">Event that caused timeout.</param>
        /// <param name="eventHandler">Handler that caused timeout.</param>
        public AsyncEventTimeoutException(AsyncEvent<TSender, TArgs> asyncEvent, AsyncEventHandler<TSender, TArgs> eventHandler) : base(asyncEvent, eventHandler as AsyncEventHandler<object, AsyncEventArgs>, "An event handler caused an async event to time out.")
        {

        }
    }
}
