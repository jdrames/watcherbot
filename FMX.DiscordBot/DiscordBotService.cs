using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using FMX.Shared;
using FMX.Shared.Settings;
using FMX.The100Watcher;
using FMX.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FMX.DiscordBot
{
    public class DiscordBotService : IHostedService, IDisposable
    {
        private ILoggerFactory _factory;
        private readonly AppSettings _settings;
        private readonly IServiceProvider _sp;
        private readonly WatcherService _watcherService;
        private readonly MongoDbClient _dbClient;

        private DiscordClient _discordClient;
        private IMongoCollection<Guild> _guildCollection;

        public DiscordBotService(AppSettings settings, MongoDbClient dbClient, IServiceProvider sp, ILoggerFactory factory)
        {
            _settings = settings;
            _dbClient = dbClient;
            _sp = sp;
            _factory = factory;
            _watcherService = _sp.GetRequiredService<WatcherService>();

            _guildCollection = _dbClient.GetCollection<Guild>(_settings.Mongo.Collections.Guilds);
        }

        public void Dispose()
        {
            _watcherService?.Dispose();
            _discordClient?.Dispose();
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            // Configuration settings for bot
            var botConfig = new DiscordConfiguration
            {
                AutoReconnect = true,
                Token = _settings.Discord.Token,
                TokenType = TokenType.Bot,
                LoggerFactory = _factory
            };

            var cmdConfig = new CommandsNextConfiguration
            {
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                Services = _sp,
                StringPrefixes = _settings.Discord.CommandPrefixes
            };

            var interactivityConfig = new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromSeconds(30),
                PaginationBehaviour = DSharpPlus.Interactivity.Enums.PaginationBehaviour.WrapAround,
                PaginationDeletion = DSharpPlus.Interactivity.Enums.PaginationDeletion.DeleteEmojis
            };

            // Initialize the bot
            _discordClient = new DiscordClient(botConfig);
            _discordClient.UseCommandsNext(cmdConfig);
            _discordClient.UseInteractivity(interactivityConfig);

            // Add commands using assembly reflection
            // This will load all the commands from the commands folder
            _discordClient.GetCommandsNext().RegisterCommands(Assembly.GetEntryAssembly());

            // Event listeners for guild activity
            _discordClient.GuildCreated += _discordClient_GuildHandler;
            _discordClient.GuildAvailable += _discordClient_GuildHandler;
            _discordClient.GuildDeleted += _discordClient_GuildDeleted;

            // Event listeners for bot actions
            _watcherService.OnGameAction += _watcherService_OnGameActionHandler;
            _watcherService.OnPlayerAction += _watcherService_OnPlayerActionHandler;

            await _discordClient.ConnectAsync();
            await _watcherService.StartAsync(stoppingToken);
        }

        private async Task _watcherService_OnPlayerActionHandler(WatcherService sender, The100Watcher.EventArgs.PlayerEventArgs args)
        {
            var filterBuilder = Builders<Guild>.Filter;
            var filter = filterBuilder.AnyEq(x => x.The100Groups, args.Game.GroupId)
                & filterBuilder.Eq(x => x.PostPlayerNotifications, true);
            var results = (await _guildCollection.FindAsync(filter)).ToList();

            // Send the notifications to the guilds requesting updates for this groups games
            for (int i = 0; i < results.Count; i++)
            {
                try
                {
                    var guild = await _discordClient.GetGuildAsync(ulong.Parse(results[i].Id));
                    if(guild.Channels.TryGetValue(ulong.Parse(results[i].NotificationsChannel), out var channel))
                    {
                        await channel.SendMessageAsync(args.Message, args.Game.ToEmbedLimited());
                    }
                    else
                    {
                        await guild.GetDefaultChannel().SendMessageAsync(args.Message, args.Game.ToEmbedLimited());
                    }
                }catch(Exception ex)
                {
                    /* shush */
                }
            }
        }

        private async Task _watcherService_OnGameActionHandler(WatcherService sender, The100Watcher.EventArgs.GameEventArgs args)
        {
            var filterBuilder = Builders<Guild>.Filter;
            var filter = filterBuilder.AnyEq(x => x.The100Groups, args.Game.GroupId)
                & filterBuilder.Eq(x => x.PostGameNotifications, true);
            var results = (await _guildCollection.FindAsync(filter)).ToList();

            // Send the notifications to the guilds requesting updates for this groups games
            for (int i = 0; i < results.Count; i++)
            {
                try
                {
                    var guild = await _discordClient.GetGuildAsync(ulong.Parse(results[i].Id));
                    if (guild.Channels.TryGetValue(ulong.Parse(results[i].NotificationsChannel), out var channel))
                    {
                        await channel.SendMessageAsync(args.Message, args.Game.ToEmbed());
                    }
                    else
                    {
                        await guild.GetDefaultChannel().SendMessageAsync(args.Message, args.Game.ToEmbed());
                    }
                }
                catch (Exception ex)
                {
                    /* shush */
                }
            }
        }

        /// <summary>
        /// Deletes associated guild from the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Discord Guild that will be deleted.</param>
        /// <returns></returns>
        private async Task _discordClient_GuildDeleted(DiscordClient sender, DSharpPlus.EventArgs.GuildDeleteEventArgs e)
        {
            var filter = Builders<Guild>.Filter.Eq(x => x.Id, e.Guild.Id.ToString());
            await _guildCollection.DeleteOneAsync(filter);
        }

        /// <summary>
        /// Ensures that any loaded guild is in the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Discord Guild to check.</param>
        /// <returns></returns>
        private async Task _discordClient_GuildHandler(DiscordClient sender, DSharpPlus.EventArgs.GuildCreateEventArgs e)
        {
            var guild = new Guild() { Id = e.Guild.Id.ToString(), NotificationsChannel = e.Guild.GetDefaultChannel().Id.ToString() };
            var filter = Builders<Guild>.Filter.Eq(x => x.Id, guild.Id);
            var result = (await _guildCollection.FindAsync(filter)).FirstOrDefault();
            if (result == null)
                await _guildCollection.InsertOneAsync(guild);
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            await _watcherService.StopAsync(stoppingToken);
            await _discordClient.UpdateStatusAsync(userStatus: DSharpPlus.Entities.UserStatus.Offline);
            await _discordClient.DisconnectAsync();
        }
    }
}
