// MauiProgram.cs

using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

using FlashcardViewer.Services;
using FlashcardViewer.Views;
using FlashcardViewer.ViewModels;

namespace FlashcardViewer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiCommunityToolkit();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // Register services
            builder.Services.AddSingleton<IFlashcardDataStore, LocalFlashcardDataStore>(); // For local storage
            //builder.Services.AddSingleton<IFlashcardDataStore, CloudFlashcardDataStore>(); // Register CloudFlashcardDataStore
            builder.Services.AddSingleton<IDataStoreService, DataStoreService>();

            // Register pages
            builder.Services.AddTransient<FlashcardSetListPage>();
            builder.Services.AddTransient<FlashcardSetListViewModel>();

            builder.Services.AddTransient<FlashcardListPage>();
            builder.Services.AddTransient<FlashcardListViewModel>();
            
            builder.Services.AddTransient<FlashcardSessionPage>();
            builder.Services.AddTransient<FlashcardSessionViewModel>();
            builder.Services.AddTransient<SessionConfigViewModel>();

            return builder.Build();
        }
    }
}
