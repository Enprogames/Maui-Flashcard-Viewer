using Microsoft.Maui.Controls;
using FlashcardViewer.Views;

namespace FlashcardViewer
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(FlashcardListPage), typeof(FlashcardListPage));
            Routing.RegisterRoute(nameof(FlashcardSessionPage), typeof(FlashcardSessionPage));
        }
    }
}
