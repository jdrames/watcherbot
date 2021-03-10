using System;

namespace FMX.Utilities
{
    /// <summary>
    /// Arguments passed to async event
    /// </summary>
    public class AsyncEventArgs : EventArgs
    {
        /// <summary>
        /// <para>Whether this event was handled.</para>
        /// <para>Setting to true will prevent other handlers from running.</para>
        /// </summary>
        public bool Handled { get; set; } = false;
    }
}
