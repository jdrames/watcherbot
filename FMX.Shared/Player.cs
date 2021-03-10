using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FMX.Shared
{
    /// <summary>
    /// A player object that represents a player from the100.io.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Player
    {
        /// <summary>
        /// The ID of the100.io user.
        /// </summary>
        [JsonPropertyName("user_id")]
        public int Id { get; set; }

        /// <summary>
        /// Indicates if this player is on the reserve list.
        /// </summary>
        [JsonPropertyName("is_reserve")]
        public bool IsReserve { get; set; }

        /// <summary>
        /// User object containing the100.io user data.
        /// </summary>
        [JsonPropertyName("user")]
        public The100User User { get; set; }

        /// <summary>
        /// Should only return the gamertag for this user.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return User.Gamertag;
        }
    }
}
