using FMX.Shared;

namespace FMX.The100Watcher.EventArgs
{
    /// <summary>
    /// Arguments for Player activity events.
    /// </summary>
    public class PlayerEventArgs : BaseEventArgs
    {
        /// <summary>
        /// The player that triggered the action.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// The game the player belonged to.
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="player"></param>
        /// <param name="game"></param>
        internal PlayerEventArgs(string message, Player player, Game game) : base()
        {
            Message = message;
            Player = player;
            Game = game;
        }
    }
}
