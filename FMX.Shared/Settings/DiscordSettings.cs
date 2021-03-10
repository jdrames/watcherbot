using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FMX.Shared.Settings
{
    public class DiscordSettings
    {
        /// <summary>
        /// Discord bot token used to access the discord gateway
        /// <para>Bot tokens can be obtained from Discord Developer portal: https://discord.com/developers/docs</para>
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Command prefixes used to trigger bot commands
        /// </summary>
        [JsonPropertyName("command_prefixes")]
        public string[] CommandPrefixes { get; set; } = new[] { "!watcher" };

        /// <summary>
        /// Number of shards that should be created.
        /// <para>This should be 1 unless the bot is 
        /// added to a large number of groups and 
        /// you're informed by Discord otherwise.</para>
        /// </summary>
        [JsonPropertyName("shards")]
        public int ShardCount { get; set; } = 1;
    }
}
