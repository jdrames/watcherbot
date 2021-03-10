using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FMX.Shared.Settings
{
    /// <summary>
    /// Application settings used throughout The100 Discord bot.
    /// </summary>
    public sealed class AppSettings
    {
        /// <summary>
        /// Settings for the Discord bot.
        /// </summary>
        [JsonPropertyName("discord")]
        public DiscordSettings Discord { get; set; } = new DiscordSettings();

        /// <summary>
        /// Settings for MongoDB access.
        /// </summary>
        [JsonPropertyName("mongo")]
        public MongoSettings Mongo { get; set; } = new MongoSettings();

        /// <summary>
        /// Setting for The100.io API access.
        /// </summary>
        [JsonPropertyName("the100")]
        public The100Settings The100 { get; set; } = new The100Settings();
    }
}
