using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FMX.Shared.Settings
{
    public class Collections
    {
        /// <summary>
        /// The name of the collection that contains the games.
        /// </summary>
        [JsonPropertyName("games")]
        public string Games { get; private set; } = "games";

        /// <summary>
        /// The name of the collection that contains the guilds.
        /// </summary>
        [JsonPropertyName("guilds")]
        public string Guilds { get; private set; } = "guilds";
    }
}
