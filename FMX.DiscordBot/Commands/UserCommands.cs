using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using FMX.Shared;
using FMX.Shared.Settings;
using FMX.Utilities;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FMX.DiscordBot.Commands
{
    [RequireGuild]
    public class UserCommands : BaseCommandModule
    {
        private MongoDbClient _dbClient;
        private AppSettings _settings;

        private IMongoCollection<Guild> _guildCollection;
        private IMongoCollection<Game> _gameCollection;

        public UserCommands(AppSettings settings, MongoDbClient dbClient)
        {
            _settings = settings;
            _dbClient = dbClient;

            _guildCollection = _dbClient.GetCollection<Guild>(_settings.Mongo.Collections.Guilds);
            _gameCollection = _dbClient.GetCollection<Game>(_settings.Mongo.Collections.Games);            
        }

        [Description("Shows a list of current games.")]
        [Command("games")]
        public async Task ShowGames(CommandContext ctx)
        {
            var filter = Builders<Guild>.Filter.Eq(x => x.Id, ctx.Guild.Id.ToString());
            var guild = (await _guildCollection.FindAsync(filter)).FirstOrDefault();
            if (guild == null)
                return;

            var gameFilterBuilder = Builders<Game>.Filter;
            var gameFilter = gameFilterBuilder.In(x => x.GroupId, guild.The100Groups)
                & gameFilterBuilder.Eq(x=>x.IsActive, true)
                & gameFilterBuilder.Gt(x => x.StartTime, DateTime.UtcNow.AddMinutes(-15));
            var games = (await _gameCollection.FindAsync(gameFilter)).ToList();

            if(games.Count == 0)
            {
                await ctx.RespondAsync("No games were found.");
                return;
            }

            if(games.Count == 1)
            {
                await ctx.Channel.SendMessageAsync("Found 1 game.", games[0].ToEmbed());
                return;
            }

            var pages = new List<Page>();
            games.ForEach(g => pages.Add(new Page($"Game {pages.Count + 1} of {games.Count}.", g.ToEmbed())));

            await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
        }

    }
}
