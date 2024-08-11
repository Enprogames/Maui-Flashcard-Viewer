using FlashcardViewer.ViewModels;

namespace FlashcardViewer.Views;

public partial class FlashcardSessionPage : ContentPage
{
	public FlashcardSessionPage(FlashcardSessionViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}