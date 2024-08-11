using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using FlashcardViewer.Models;
using FlashcardViewer.Services;
using FlashcardViewer.Views;
using FlashcardViewer.ViewModels;

namespace FlashcardViewer.Views;

public partial class FlashcardListPage : ContentPage
{
    public FlashcardListPage(FlashcardListViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
    }
}
