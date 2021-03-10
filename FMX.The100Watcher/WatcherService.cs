using FMX.Shared;
using FMX.Shared.Settings;
using FMX.The100Watcher.EventArgs;
using FMX.Utilities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FMX.The100Watcher
{
    

    public class WatcherService : IDisposable
    {
        // Event handler objects
        public event AsyncEventHandler<WatcherService, PlayerEventArgs> OnPlayerAction;
        public event AsyncEventHandler<WatcherService, GameEventArgs> OnGameAction;

        private readonly MongoDbClient _dbClient;
        private readonly HttpClient _httpClient;
        private readonly AppSettings _settings;

        private Timer _timer;

        private IMongoCollection<Guild> _guildCollection;
        private IMongoCollection<Game> _gameCollection;

        private CancellationToken _stoppingToken;

        public WatcherService(AppSettings settings, MongoDbClient dbClient, HttpClient httpClient)
        {
            _settings = settings;
            _dbClient = dbClient;
            _httpClient = httpClient;

            _guildCollection = _dbClient.GetCollection<Guild>(_settings.Mongo.Collections.Guilds);
            _gameCollection = _dbClient.GetCollection<Game>(_settings.Mongo.Collections.Games);            
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _stoppingToken = cancellationToken;
            _timer = new Timer(DoWatchCycle);
            _timer.Change(0, Timeout.Infinite);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stoppingToken = cancellationToken;
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void DoWatchCycle(object state)
        {
            DoWatchCycleAsync().GetAwaiter().GetResult();
        }

        private async Task DoWatchCycleAsync()
        {
            var groupIds = await GetGroupIdsFromGuilds();
            if (groupIds.Count > 0)
                await GetGamesForGroups(groupIds);

            // Waiting 5 seconds between cycles
            await Task.Delay(5000);
            _timer?.Change(0, Timeout.Infinite);
        }

        /// <summary>
        /// Obtain a list of the100.io groups from the guild database
        /// </summary>
        /// <returns></returns>
        private async Task<List<int>> GetGroupIdsFromGuilds()
        {
            var groupIds = new List<int>();
            var filter = Builders<Guild>.Filter.Empty;
            var results = await _guildCollection.FindAsync(filter);
            await results.ForEachAsync(guild =>
            {
                foreach (var groupId in guild.The100Groups)
                {
                    if (!groupIds.Contains(groupId))
                        groupIds.Add(groupId);
                }
            });
            return groupIds;
        }


        private async Task GetGamesForGroups(List<int> groupIds)
        {
            for(int i = 0; i<groupIds.Count; i++)
            {
                var games = await GetGamesFromThe100(groupIds[i]);
                games.ForEach(async game => await CheckForChanges(game));
            }
        }


        private async Task CheckForChanges(Game game)
        {
            var filter = Builders<Game>.Filter.Eq(x => x.Id, game.Id);
            var dbGame = (await _gameCollection.FindAsync(filter)).FirstOrDefault();
            
            // A new game that does not exist yet
            if(dbGame == null)
            {
                // Add the game to the db
                await _gameCollection.InsertOneAsync(game);
                OnGameAction?.Invoke(this, new GameEventArgs("New game scheduled.", game));
                return;
            }

            await CheckGameName(dbGame, game);
            await CheckGameActivity(dbGame, game);
            await CheckGameNote(dbGame, game);
            await CheckGameStartTime(dbGame, game);
            await CheckPlayersJoined(dbGame, game);
            await CheckPlayersLeft(dbGame, game);
        }
        
        private async Task CheckGameStartTime(Game existingGame, Game game)
        {
            if (!existingGame.StartTime.Equals(game.StartTime))
            {
                var filter = Builders<Game>.Filter.Eq(x => x.Id, existingGame.Id);
                var update = Builders<Game>.Update.Set(x => x.StartTime, game.StartTime);
                await _gameCollection.UpdateOneAsync(filter, update);
                OnGameAction?.Invoke(this, new GameEventArgs("The start time has changed.", game));
            }
        }

        private async Task CheckGameActivity(Game existingGame, Game game)
        {
            if (!existingGame.GameActivity.Equals(game.GameActivity))
            {
                var filter = Builders<Game>.Filter.Eq(x => x.Id, existingGame.Id);
                var update = Builders<Game>.Update.Set(x => x.GameActivity, game.GameActivity);
                await _gameCollection.UpdateOneAsync(filter, update);
                OnGameAction?.Invoke(this, new GameEventArgs("The game activity has changed.", game));
            }
        }

        private async Task CheckGameNote(Game existingGame, Game game)
        {
            if(!existingGame.Notes.Equals(game.Notes))
            {
                var filter = Builders<Game>.Filter.Eq(x => x.Id, existingGame.Id);
                var update = Builders<Game>.Update.Set(x => x.Notes, game.Notes);
                await _gameCollection.UpdateOneAsync(filter, update);
                OnGameAction?.Invoke(this, new GameEventArgs("The game note has changed.", game));
            }
        }

        private async Task CheckGameName(Game existingGame, Game game)
        {
            if (!existingGame.GameName.Equals(game.GameName))
            {
                var filter = Builders<Game>.Filter.Eq(x => x.Id, existingGame.Id);
                var update = Builders<Game>.Update.Set(x => x.GameName, game.GameName);
                await _gameCollection.UpdateOneAsync(filter, update);
                OnGameAction?.Invoke(this, new GameEventArgs("The game name has changed.", game));
            }
        }

        private async Task CheckPlayersJoined(Game existingGame, Game game)
        {
            for(int i = 0; i < game.Players.Count; i++)
            {
                var player = game.Players[i];
                if (existingGame.Players.Find(p => p.Id == player.Id) == null)
                {
                    var filter = Builders<Game>.Filter.Eq(x => x.Id, existingGame.Id);
                    var update = Builders<Game>.Update.AddToSet(x => x.Players, player);
                    await _gameCollection.UpdateOneAsync(filter, update);
                    OnPlayerAction?.Invoke(this, new PlayerEventArgs("A player has joined the game.", player, game));
                }
            }
        }

        private async Task CheckPlayersLeft(Game existingGame, Game game)
        {
            for (int i = 0; i < existingGame.Players.Count; i++)
            {
                var player = existingGame.Players[i];
                if (game.Players.Find(p => p.Id == player.Id) == null)
                {
                    var filter = Builders<Game>.Filter.Eq(x => x.Id, existingGame.Id);                    
                    var update = Builders<Game>.Update.Pull(x => x.Players, player);
                    await _gameCollection.UpdateOneAsync(filter, update);
                    OnPlayerAction?.Invoke(this, new PlayerEventArgs("A player has left the game.", player, game));
                }
            }
        }


        private async Task<List<Game>> GetGamesFromThe100(int groupId)
        {
            // The100 API uses pages for their results.
            // The pages start at 1 not 0.
            var page = 1;
            var games = new List<Game>();
            while (true)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.the100.io/api/v2/groups/{groupId}/gaming_sessions?page={page}");
                request.Headers.Add("Authorization", $"Token token=\"{_settings.The100.ApiToken}\"");
                var response = await _httpClient.SendAsync(request, _stoppingToken);
                if (!response.IsSuccessStatusCode)
                    break;

                var results = JsonSerializer.Deserialize<List<Game>>(await response.Content.ReadAsStringAsync());                
                
                if (results.Count == 0)
                    break;
                
                results.ForEach(game => { games.Add(game); });
                page++;                
            }
            return games;
        }
    }
}
