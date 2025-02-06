using Microsoft.Maui.Controls;
using FlashcardViewer.Views;

using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;

namespace FlashcardViewer
{
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;

            Routing.RegisterRoute(nameof(FlashcardListPage), typeof(FlashcardListPage));
            Routing.RegisterRoute(nameof(FlashcardSessionPage), typeof(FlashcardSessionPage));
        }

        [RelayCommand]
        void SwitchTheme(string themeName)
        {
            ThemeManager.ApplyTheme(themeName);
        }
    }
}
