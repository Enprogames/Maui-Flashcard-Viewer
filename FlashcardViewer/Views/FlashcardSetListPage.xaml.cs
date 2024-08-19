using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using FlashcardViewer.Models;
using FlashcardViewer.Services;
using FlashcardViewer.Views;
using FlashcardViewer.ViewModels;

using CommunityToolkit.Mvvm.Input;

namespace FlashcardViewer.Views
{
    public partial class FlashcardSetListPage : ContentPage
    {
        public FlashcardSetListPage(FlashcardSetListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            SessionConfigPopup.IsVisible = false;
        }

        private async void OnAddSetButtonClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("New Flashcard Set", "Enter the name of the flashcard set:");
            if (!string.IsNullOrEmpty(result))
            {
                await ((FlashcardSetListViewModel)BindingContext).AddFlashcardSetCommand.ExecuteAsync(result);
            }
        }

        private void OnStartSessionButtonClicked(object sender, EventArgs e)
        {
            List<SelectableFlashcardSet> selectedSets
                = ((FlashcardSetListViewModel) BindingContext).FlashcardSets
                                                              .Where(set => set.IsSelected)
                                                              .ToList();
            if (selectedSets.Any())
            {
                List<int> setIds = selectedSets.Select(set => set.Id).ToList();

                string sessionTitle;
                if (setIds.Count > 1)
                {
                    sessionTitle = "Multiple Sets";
                }
                else
                {
                    sessionTitle = selectedSets[0].Title;
                }

                SessionConfigViewModel sessionConfig = new SessionConfigViewModel(
                    sessionTitle,
                    selectedSets.Select(set => set.Id).ToList()
                );

                var confirmButtonCommand = new AsyncRelayCommand(async () =>
                {
                    await Shell.Current.GoToAsync(nameof(FlashcardSessionPage), new Dictionary<string, object>
                    {
                        { "SessionConfig", sessionConfig }
                    });
                });

                SessionConfigManagementViewModel fcSessionConfigVM
                    = new (
                        sessionTitle,
                        "Start Session",
                        confirmButtonCommand,
                        sessionConfig
                    );

                // Set the ViewModel for the Popup
                SessionConfigPopup.BindingContext = fcSessionConfigVM;

                SessionConfigPopup.IsVisible = true;
            }
        }
    }
}
