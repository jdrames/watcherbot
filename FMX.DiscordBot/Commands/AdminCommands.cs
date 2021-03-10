using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FMX.Shared;
using FMX.Shared.Settings;
using FMX.Utilities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FMX.DiscordBot.Commands
{
    [RequireGuild]
    [RequireUserPermissions(DSharpPlus.Permissions.ManageGuild)]
    public class AdminCommands : BaseCommandModule
    {
        private MongoDbClient _dbClient;
        private AppSettings _settings;

        private IMongoCollection<Guild> _guildCollection;

        public AdminCommands(AppSettings settings, MongoDbClient dbClient)
        {
            _settings = settings;
            _dbClient = dbClient;

            _guildCollection = _dbClient.GetCollection<Guild>(_settings.Mongo.Collections.Guilds);
        }

        [Description("Sets the current channel as the channel to post notifications to.")]
        [Command("PostHere")]
        public async Task SetNotificationChannel(CommandContext ctx)
        {
            var filter = Builders<Guild>.Filter.Eq(x => x.Id, ctx.Guild.Id.ToString());
            var update = Builders<Guild>.Update.Set(x => x.NotificationsChannel, ctx.Channel.Id.ToString());
            await _guildCollection.UpdateOneAsync(filter, update);
            await ctx.RespondAsync("Notications will now be posted in this channel.");
        }

        [Description("Add a group from the100.io to monitor for game activity.")]
        [Command("AddGroup")]
        public async Task AddGroup(CommandContext ctx, int groupId)
        {
            var filter = Builders<Guild>.Filter.Eq(x => x.Id, ctx.Guild.Id.ToString());
            var update = Builders<Guild>.Update.AddToSet(x => x.The100Groups, groupId);
            await _guildCollection.UpdateOneAsync(filter, update);
            await ctx.RespondAsync($"Added {groupId} to the list of groups currently being monitored.");
        }

        [Description("Removes a the100.io group from being monitored.")]
        [Command("RemoveGroup")]
        public async Task RemoveGroup(CommandContext ctx, int groupId)
        {
            var filter = Builders<Guild>.Filter.Eq(x => x.Id, ctx.Guild.Id.ToString());
            var update = Builders<Guild>.Update.Pull(x => x.The100Groups, groupId);
            await _guildCollection.UpdateOneAsync(filter, update);
            await ctx.RespondAsync($"Removed {groupId} from monitoring.");
        }

        [Description("Set the bot to display new game messages.")]
        [Command("ShowGameMessages")]
        public async Task ShowGameMessages(CommandContext ctx, bool showMessages = true)
        {
            var filter = Builders<Guild>.Filter.Eq(x => x.Id, ctx.Guild.Id.ToString());
            var update = Builders<Guild>.Update.Set(x => x.PostGameNotifications, showMessages);
            await _guildCollection.UpdateOneAsync(filter, update);
            await ctx.RespondAsync(showMessages ? "Game notifications will now be shown." : "Game notifications will no longer be shown.");
        }

        [Description("Set the bot to display player action messages.")]
        [Command("ShowPlayerMessages")]
        public async Task ShowPlayerMessages(CommandContext ctx, bool showMessages = true)
        {
            var filter = Builders<Guild>.Filter.Eq(x => x.Id, ctx.Guild.Id.ToString());
            var update = Builders<Guild>.Update.Set(x => x.PostPlayerNotifications, showMessages);
            await _guildCollection.UpdateOneAsync(filter, update);
            await ctx.RespondAsync(showMessages ? "Player notfications will now be shown." : "Player notifications will no longer be shown.");
        }
    }
}
