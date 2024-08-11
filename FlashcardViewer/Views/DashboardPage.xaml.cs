using Microsoft.Maui.Controls;
using System.Threading.Tasks;

using FlashcardViewer.Models;
using FlashcardViewer.Services;
using FlashcardViewer.Views;
using FlashcardViewer.ViewModels;

namespace FlashcardViewer.Views
{
    public partial class DashboardPage : ContentPage
    {
        private readonly FlashcardSetListViewModel _viewModel;

        public DashboardPage(FlashcardSetListViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;

            var flashcardSetListView = new FlashcardSetListView(_viewModel);
            SetListViewContainer.Content = flashcardSetListView;
        }

        private async void OnAddSetButtonClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("New Flashcard Set", "Enter the name of the flashcard set:");
            if (!string.IsNullOrEmpty(result))
            {
                await _viewModel.AddFlashcardSetCommand.ExecuteAsync(result);
            }
        }

        private void OnStartSessionClicked(object sender, EventArgs e)
        {
            // TODO: Start session
        }
    }
}
