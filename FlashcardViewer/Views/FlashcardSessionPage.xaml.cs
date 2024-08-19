using FlashcardViewer.ViewModels;
using FlashcardViewer.Models;
using CommunityToolkit.Mvvm.Input;

namespace FlashcardViewer.Views;

public partial class FlashcardSessionPage : ContentPage
{
	public FlashcardSessionPage(FlashcardSessionViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;

        SessionConfigPopup.IsVisible = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await ((FlashcardSessionViewModel)BindingContext).LoadSetsAsync();
    }

    private void OnConfigureSessionClicked(object sender, EventArgs e)
	{
        SessionConfigViewModel sessionConfig = 
            ((FlashcardSessionViewModel)BindingContext).SessionConfig;

        string sessionTitle = sessionConfig.Title;

        SessionConfigManagementViewModel fcSessionConfigVM
            = new(
                "Configure Session",
                "Start Session",
                null,
                sessionConfig
            );

        // Set the ViewModel for the Popup
        SessionConfigPopup.BindingContext = fcSessionConfigVM;

        SessionConfigPopup.IsVisible = true;
    }
}
