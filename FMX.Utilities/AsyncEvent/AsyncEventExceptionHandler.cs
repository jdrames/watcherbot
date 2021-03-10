using System;

namespace FMX.Utilities
{
    /// <summary>
    /// Handles any exception raised by <see cref="AsyncEvent{TSender, TArgs}"/>
    /// </summary>
    /// <typeparam name="TSender">Type of object dispatching event.</typeparam>
    /// <typeparam name="TArgs">Type of object that holds arguments for event.</typeparam>
    /// <param name="asyncEvent">Async event that threw exception.</param>
    /// <param name="ex">Exception that was thrown.</param>
    /// <param name="handler">Handler that threw exception.</param>
    /// <param name="sender">Object that dispatched event.</param>
    /// <param name="eventArgs">Arguments included in event dispatch</param>
    public delegate void AsyncEventExceptionHandler<TSender, TArgs>(AsyncEvent<TSender, TArgs> asyncEvent, Exception ex, AsyncEventHandler<TSender, TArgs> handler, TSender sender, TArgs eventArgs) where TArgs : AsyncEventArgs;
}
