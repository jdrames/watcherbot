using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FMX.Shared
{
    [BsonIgnoreExtraElements]
    public class Guild
    {
        /// <summary>
        /// Discord guild snowflake ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Discord guild channel where bot notifications should post.
        /// </summary>
        public string NotificationsChannel { get; set; }

        /// <summary>
        /// Whether the bot should post game notifications.
        /// </summary>
        public bool PostGameNotifications { get; set; } = true;

        /// <summary>
        /// Whether the both should post player notifications.
        /// </summary>
        public bool PostPlayerNotifications { get; set; } = true;

        /// <summary>
        /// A list of groups from the 100 that this guild wants game data for.
        /// </summary>
        public List<int> The100Groups { get; set; } = new List<int>();
    }
}
