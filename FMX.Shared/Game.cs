using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FMX.Shared
{
    /// <summary>
    /// Game object representing a game from the100.io
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Game
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("group_id")]
        public int GroupId { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("game_name")]
        public string GameName { get; set; }

        [JsonPropertyName("category")]
        public string GameActivity { get; set; }

        [JsonPropertyName("name")]
        public string Notes { get; set; }

        [JsonPropertyName("computed_image")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("confirmed_sessions")]
        public List<Player> Players { get; set; }

        [JsonPropertyName("group_name")]
        public string GroupName { get; set; }

        [JsonIgnore]
        public bool IsActive { get; set; } = true;
    }
}
