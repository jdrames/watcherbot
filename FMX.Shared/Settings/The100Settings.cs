using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FMX.Shared.Settings
{
    public class The100Settings
    {
        /// <summary>
        /// The token used to access the100.io API
        /// </summary>
        [JsonPropertyName("token")]
        public string ApiToken { get; set; } = string.Empty;
    }
}
