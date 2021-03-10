using DSharpPlus.Entities;
using FMX.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMX.DiscordBot
{
    public static class Helpers
    {
        /// <summary>
        /// Converts a Game to a Discord embedable object.
        /// </summary>
        /// <param name="game">The game to convert.</param>
        /// <returns></returns>
        public static DiscordEmbedBuilder ToEmbed(this Game game)
        {
            var builder = new DiscordEmbedBuilder();
            builder.Title = $"{game.GameName} - {game.GameActivity}";
            builder.Description = $"**{game.GroupName}**\r\nStarts: {game.StartTime:MM/dd/yyyy hh:mm tt K}";
            if (!string.IsNullOrEmpty(game.Notes)) 
                builder.Description += $"\r\n{game.Notes}";

            builder.Url = $"https://www.the100.io/gaming_sessions/{game.Id}";
            if (!string.IsNullOrEmpty(game.ImageUrl))
                builder.ImageUrl = game.ImageUrl;

            if (game.Players.Where(player=>!player.IsReserve).Count() > 0)
                builder.AddField("Players", string.Join("\r\n", game.Players.Where(player=>!player.IsReserve)), true);

            if (game.Players.Where(player => player.IsReserve).Count() > 0)
                builder.AddField("Reserves", string.Join("\r\n", game.Players.Where(player => player.IsReserve)), true);

            return builder;
        }

        /// <summary>
        /// Converts a game to a Discord embedable object.
        /// <para>This is a reduced version with only the game title and description.</para>
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static DiscordEmbedBuilder ToEmbedLimited(this Game game)
        {
            var builder = new DiscordEmbedBuilder();
            builder.Title = $"{game.GameName} - {game.GameActivity}";
            builder.Description = $"Starts: {game.StartTime:MM/dd/yyyy hh:mm tt K}";
            if (!string.IsNullOrEmpty(game.Notes))
                builder.Description += $"\r\n{game.Notes}";

            builder.Url = $"https://www.the100.io/gaming_sessions/{game.Id}";
            return builder;
        }
    }
}
