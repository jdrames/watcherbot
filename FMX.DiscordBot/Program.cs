using FMX.Shared.Settings;
using FMX.The100Watcher;
using FMX.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace FMX.DiscordBot
{
    class Program
    {
        // Convert to async 
        static void Main(string[] args)
        {
            // Validate that the settings.json file exists
            // This is required for the app
            if (!File.Exists("settings.json"))
            {
                var json = JsonSerializer.Serialize(new AppSettings(), new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("settings.json", json, Encoding.UTF8);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The settings file was not found. A new one has been generated. Edit it and restart this app.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            // Ensures date comparison works correctly between mongodb and the100.io api datetime values.
            BsonSerializer.RegisterSerializer(DateTimeSerializer.LocalInstance);


            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((HostContext, services) =>
                {                    
                    var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText("settings.json"));                    
                    services.AddSingleton<AppSettings>(settings);
                    services.AddSingleton<HttpClient>();
                    services.AddSingleton<MongoDbClient>(new MongoDbClient(settings.Mongo.ConnectionString, settings.Mongo.DatabaseName));
                    services.AddSingleton<WatcherService>();
                    services.AddHostedService<DiscordBotService>();
                });
    }
}
