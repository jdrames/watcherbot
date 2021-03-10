using FMX.Shared;

namespace FMX.The100Watcher.EventArgs
{
    /// <summary>
    /// Arguments for Game activity events.
    /// </summary>
    public class GameEventArgs : BaseEventArgs
    {
        /// <summary>
        /// The game that triggered the event.
        /// </summary>
        public Game Game { get; set; }

        internal GameEventArgs(string message, Game game) : base()
        {
            Message = message;
            Game = game;
        }
    }
}
