using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using FlashcardViewer.Models;
using FlashcardViewer.Services;
using FlashcardViewer.Views;
using FlashcardViewer.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace FlashcardViewer.Views;

public partial class FlashcardListPage : ContentPage
{
    public FlashcardListPage(FlashcardListViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

        SessionConfigPopup.IsVisible = false;
    }

    async void OnStartSessionClicked(object sender, EventArgs e)
    {
        int setId = ((FlashcardListViewModel)BindingContext).SetId;

        SessionConfigViewModel sessionConfig = new (
            ((FlashcardListViewModel)BindingContext).FlashcardSet.Title,
            new List<int>(setId)
        );

        IAsyncRelayCommand confirmButtonCommand
            = new AsyncRelayCommand(async () => {
                await Shell.Current.GoToAsync(nameof(FlashcardSessionPage), new Dictionary<string, object>
                    {
                        { "SessionConfig", sessionConfig }
                    });
            });

        SessionConfigManagementViewModel sessionConfigManagementViewModel
            = new SessionConfigManagementViewModel(
                "Configure Session",
                "Start",
                confirmButtonCommand,
                sessionConfig
            );

        SessionConfigPopup.IsVisible = true;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}
