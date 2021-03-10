using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FMX.Shared
{
    /// <summary>
    /// A user object representing a user from the100.io.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class The100User
    {
        /// <summary>
        /// The ID of the100.io user.
        /// <para>This is a duplicate value of <see cref="Player.Id"/> due to the100.io API formatting.</para>
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// GamerTag of the100.io user.
        /// </summary>
        [JsonPropertyName("gamertag")]
        public string Gamertag { get; set; }
    }
}
