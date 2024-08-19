using Microsoft.Extensions.DependencyInjection;

namespace FlashcardViewer
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;

            MainPage = new AppShell();

            // Load the theme during app initialization
            ThemeManager.LoadSavedTheme();
        }
    }
}
