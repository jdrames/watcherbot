using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FMX.Shared.Settings
{
    public class MongoSettings
    {
        /// <summary>
        /// MongoDB server connection string.
        /// </summary>
        [JsonPropertyName("connection_string")]
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// MongoDB database name that will be used.
        /// </summary>
        [JsonPropertyName("database_name")]
        public string DatabaseName { get; set; } = "The100Monitor";

        /// <summary>
        /// Names for the MongoDB collections that will be used.
        /// </summary>
        [JsonPropertyName("collection_names")]
        public Collections Collections { get; set; } = new Collections();        
    }
}
