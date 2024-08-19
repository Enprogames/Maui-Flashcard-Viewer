using FlashcardViewer.Models;
using FlashcardViewer.ViewModels;

namespace FlashcardViewer.Views;

public partial class SessionConfigManagementPopup : ContentView
{
	public SessionConfigManagementPopup()
	{
		InitializeComponent();
	}

    private void OnStartSessionClicked(object sender, EventArgs e)
    {
		IsVisible = false;
        BindingContext = null;
    }

    private void OnCloseButtonClicked(object sender, EventArgs e)
    {
        IsVisible = false;
        BindingContext = null;
    }
}
