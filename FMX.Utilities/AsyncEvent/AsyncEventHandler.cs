using System.Threading.Tasks;

namespace FMX.Utilities
{
    /// <summary>
    /// Handles async event of type <see cref="AsyncEvent{TSender, TArgs}"/>.
    /// </summary>
    /// <typeparam name="TSender">Type of object that dispatched this event.</typeparam>
    /// <typeparam name="TArgs">Type of object that holds arguments for this event.</typeparam>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="args">Arguments for the event.</param>
    /// <returns></returns>
    public delegate Task AsyncEventHandler<in TSender, in TArgs>(TSender sender, TArgs args) where TArgs : AsyncEventArgs;
}
