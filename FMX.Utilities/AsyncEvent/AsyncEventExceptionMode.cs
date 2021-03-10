
namespace FMX.Utilities
{
    /// <summary>
    /// Define behavior for exceptions from <see cref="AsyncEvent{TSender, TArgs}.InvokeAsync(TSender, TArgs, AsyncEventExceptionMode)"/>.
    /// </summary>
    public enum AsyncEventExceptionMode : int
    {
        /// <summary>
        /// No exceptions should be thrown. Only exception handlers will be used.
        /// </summary>
        IgnoreAll = 0,

        /// <summary>
        /// Only fatal exceptions should be thrown
        /// </summary>
        ThrowFatal = 1,

        /// <summary>
        /// Only non-fatal exceptions should be thrown
        /// </summary>
        ThrowNonFatal = 2,

        /// <summary>
        /// All exceptions should be thrown.
        /// </summary>
        ThrowAll = ThrowFatal | ThrowNonFatal,

        /// <summary>
        /// Defines that only fatal exceptions should be handled by specified exception handler.
        /// </summary>
        HandleFatal = 4,

        /// <summary>
        /// Defines that only non-fatal exceptions should be handled by specified exception handler.
        /// </summary>
        HandleNonFatal = 8,

        /// <summary>
        /// Defines that all exceptions should be handled by specified exception handler
        /// </summary>
        HandleAll = HandleFatal | HandleNonFatal,

        /// <summary>
        /// Defines all exceptions should be thrown and handled by specified exception handler.
        /// </summary>
        ThrowAllHandleAll = ThrowAll | HandleAll,

        /// <summary>
        /// Default mode, equivalent to <see cref="HandleAll"/>
        /// </summary>
        Default = HandleAll
    }
}
