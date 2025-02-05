using Domain.Abstractions;
using Domain.Handlers;
using Domain.Handlers.CmdHandler;
using Domain.Handlers.FileHandler;
using Domain.Handlers.InlineCallBack;
using Domain.Options;
using Domain.Services;
using Infrastructure.Clients;
using Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace Api
{
    public static class DependenciesExtensions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DbConnection");
            var envOpt = configuration.GetSection(EnvironmentOptions.SectionMame).Get<EnvironmentOptions>();

            EnvironmentOptionsHelper.Init(envOpt);

            services.Configure<EnvironmentOptions>(configuration.GetRequiredSection(EnvironmentOptions.SectionMame));

            services.AddDbContextFactory<TgMediaFileSaverDbContext>(options => options.UseNpgsql(connectionString));

            services.AddScoped<IMyDbContextPoolFactory, MyDbContextPoolFactory>();
            services.AddScoped<IUsersStateService, UsersStateService>();
            services.AddScoped<IManagmentStateService, ManagmenStateService>();
            services.AddScoped<ITelegramClient, TelegramClient>();
            services.AddScoped<IBotService, BotService>();
            services.AddScoped<IDataStore, DataStoreProxy>((service) =>
            {
                var factory = services.BuildServiceProvider().GetRequiredService<IMyDbContextPoolFactory>();
                var dataStore = new DataStore(factory);
                return new DataStoreProxy(dataStore);
            });
            services.AddScoped<ISearchService, SearchService>();

            services.AddScoped<IVideoStickersBotClient, VideoStickersBotClient>((service) =>
            {
                return new VideoStickersBotClient(envOpt.VideoStickerBotUrl);
            });

            services.AddScoped<IVideoResize, VideoResizeClient>((service) =>
            {
                return new VideoResizeClient(envOpt.VideoResizeUrl);
            });

            services.AddScoped<ITelegramBotClient, TelegramBotClient>((service) =>
            {
                return new TelegramBotClient(envOpt.BotSecret);
            });

            services.AddDependenciesTelegramHandlers();

            return services;
        }

        private static IServiceCollection AddDependenciesTelegramHandlers(this IServiceCollection services)
        {
            services.AddScoped<ITelegramMessageHandler, NewVideoFileHandler>();
            services.AddScoped<ITelegramMessageHandler, NewVideoNoteHandler>();
            services.AddScoped<ITelegramMessageHandler, DescriptionFromNewFileHandler>();
            services.AddScoped<ITelegramMessageHandler, ExistVideoNoteHandler>();
            services.AddScoped<ITelegramMessageHandler, InlineQueryHandler>();
            services.AddScoped<ITelegramMessageHandler, CallBackDescriptionEditHandler>();
            services.AddScoped<ITelegramMessageHandler, CallBackRemoveHandler>();
            services.AddScoped<ITelegramMessageHandler, ConfirmRemoveHandler>();
            services.AddScoped<ITelegramMessageHandler, UpdateDescriptionHandler>();
            services.AddScoped<ITelegramMessageHandler, InlineResultHandler>();
            services.AddScoped<ITelegramMessageHandler, CmdFilesHandler>();
            services.AddScoped<ITelegramMessageHandler, CmdStartHandler>();
            services.AddScoped<ITelegramMessageHandler, CmdHelpHanlder>();

            return services;
        }
    }
}