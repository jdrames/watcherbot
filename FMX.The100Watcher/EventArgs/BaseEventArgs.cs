using FMX.Utilities;

namespace FMX.The100Watcher.EventArgs
{
    public class BaseEventArgs : AsyncEventArgs
    {
        /// <summary>
        /// A message indicating what event has taken place.
        /// </summary>
        public string Message {get; set;}

        internal BaseEventArgs() : base() { }
    }
}
