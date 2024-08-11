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
    public partial class FlashcardSetListView : ContentView
    {
        FlashcardSetListViewModel _viewModel;

        public FlashcardSetListView(FlashcardSetListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }
    }
}
