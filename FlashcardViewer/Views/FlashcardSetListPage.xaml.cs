using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using FlashcardViewer.Models;
using FlashcardViewer.Services;
using FlashcardViewer.Views;
using FlashcardViewer.ViewModels;

namespace FlashcardViewer.Views
{
    public partial class FlashcardSetListPage : ContentPage
    {
        FlashcardSetListViewModel _viewModel;

        public FlashcardSetListPage(FlashcardSetListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        private async void OnAddSetButtonClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("New Flashcard Set", "Enter the name of the flashcard set:");
            if (!string.IsNullOrEmpty(result))
            {
                await _viewModel.AddFlashcardSetCommand.ExecuteAsync(result);
            }
        }
    }
}
