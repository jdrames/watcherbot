using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace FMX.Utilities
{
    /// <summary>
    /// Abstract class for <see cref="AsyncEvent{TSender, TArgs}"/> for using instances without knowing instance type params.
    /// </summary>
    public abstract class AsyncEvent
    {
        public string Name { get; }

        private protected AsyncEvent(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Asyncronous event. The handlers of these events are executed async, but sequentially.
    /// </summary>
    /// <typeparam name="TSender">Type of object that dispathes the event.</typeparam>
    /// <typeparam name="TArgs">Type of argument passed to event handlers.</typeparam>
    public sealed class AsyncEvent<TSender, TArgs> : AsyncEvent where TArgs : AsyncEventArgs
    {
        /// <summary>
        /// Max execution time for all handlers. 
        /// </summary>
        public TimeSpan MaxExecutionTime { get; }

        private readonly object _lock = new object();
        private ImmutableArray<AsyncEventHandler<TSender, TArgs>> _handlers;
        private readonly AsyncEventExceptionHandler<TSender, TArgs> _exceptionHandler;

        /// <summary>
        /// Create a async event with specified name and execution handler.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="maxExecutionTime">Maximum handler execution time. Value of <see cref="TimeSpan.Zero"/> is infinite.</param>
        /// <param name="exceptionHandler">Delegat that handles exceptions caused by this event.</param>
        public AsyncEvent(string name, TimeSpan maxExecutionTime, AsyncEventExceptionHandler<TSender, TArgs> exceptionHandler) : base(name)
        {
            _handlers = ImmutableArray<AsyncEventHandler<TSender, TArgs>>.Empty;
            _exceptionHandler = exceptionHandler;
            MaxExecutionTime = maxExecutionTime;
        }

        /// <summary>
        /// Registers a new event handler.
        /// </summary>
        /// <param name="handler"></param>
        public void Register(AsyncEventHandler<TSender, TArgs> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (_lock) _handlers = _handlers.Add(handler);
        }

        /// <summary>
        /// Removes existing event handler.
        /// </summary>
        /// <param name="handler"></param>
        public void Unregister(AsyncEventHandler<TSender, TArgs> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (_lock) _handlers = _handlers.Remove(handler);
        }

        /// <summary>
        /// Removes all event handlers.
        /// </summary>
        public void UnregisterAll()
        {
            _handlers = ImmutableArray<AsyncEventHandler<TSender, TArgs>>.Empty;
        }

        /// <summary>
        /// Raises this event by running its handlers.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="args">Arguments for this event.</param>
        /// <param name="exceptionMode">How to handle exception from event handlers.</param>
        /// <returns></returns>
        public async Task InvokeAsync(TSender sender, TArgs args, AsyncEventExceptionMode exceptionMode = AsyncEventExceptionMode.Default)
        {
            var handlers = _handlers;
            if (handlers.Length == 0) return;

            List<Exception> exceptions = null;
            if ((exceptionMode & AsyncEventExceptionMode.ThrowAll) != 0)
                exceptions = new List<Exception>(handlers.Length * 2);

            var timeout = MaxExecutionTime > TimeSpan.Zero ? Task.Delay(MaxExecutionTime) : null;

            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                try
                {
                    var handlerTask = handler(sender, args);
                    if (handlerTask != null && timeout != null)
                    {
                        var result = await Task.WhenAny(timeout, handlerTask).ConfigureAwait(false);
                        if (result == timeout)
                        {
                            timeout = null;
                            var timeoutEx = new AsyncEventTimeoutException<TSender, TArgs>(this, handler);

                            if ((exceptionMode & AsyncEventExceptionMode.HandleNonFatal) == AsyncEventExceptionMode.HandleNonFatal)
                                HandleException(timeoutEx, handler, sender, args);

                            if ((exceptionMode & AsyncEventExceptionMode.ThrowNonFatal) == AsyncEventExceptionMode.ThrowNonFatal)
                                exceptions.Add(timeoutEx);

                            await handlerTask.ConfigureAwait(false);
                        }
                    }
                    else if (handlerTask != null)
                    {
                        await handlerTask.ConfigureAwait(false);
                    }

                    if (args.Handled)
                        break;
                }
                catch (Exception ex)
                {
                    args.Handled = false;

                    if ((exceptionMode & AsyncEventExceptionMode.HandleFatal) == AsyncEventExceptionMode.HandleFatal)
                        HandleException(ex, handler, sender, args);

                    if ((exceptionMode & AsyncEventExceptionMode.ThrowFatal) == AsyncEventExceptionMode.ThrowFatal)
                        exceptions.Add(ex);
                }
            }

            if ((exceptionMode & AsyncEventExceptionMode.ThrowAll) != 0 && exceptions.Count > 0)
                throw new AggregateException("Exceptions thrown during execution of event handlers.", exceptions);
        }

        private void HandleException(Exception ex, AsyncEventHandler<TSender, TArgs> handler, TSender sender, TArgs args)
        {
            if (_exceptionHandler != null)
                _exceptionHandler(this, ex, handler, sender, args);
        }
    }
}
