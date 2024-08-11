using FlashcardViewer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

using FlashcardViewer.Models;
using CommunityToolkit.Mvvm.Input;

namespace FlashcardViewer.ViewModels
{
    [QueryProperty(nameof(SetId), "setId")]
    [QueryProperty(nameof(IsAutoplayEnabled), "isAutoplayEnabled")]
    [QueryProperty(nameof(DisplayTermFirst), "displayTermFirst")]
    [QueryProperty(nameof(IsShuffleEnabled), "isShuffleEnabled")]
    public partial class FlashcardSessionViewModel : ObservableObject
    {
        private int _setId;
        public int SetId
        {
            get => _setId;
            set
            {
                _setId = value;
                LoadFlashcards(value); // Method to load flashcards based on SetId
            }
        }

        private IFlashcardDataStore _dataStore;

        [ObservableProperty]
        private bool displayTermFirst;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private ObservableCollection<Flashcard> flashcards = new ObservableCollection<Flashcard>();

        [ObservableProperty]
        private Flashcard currentFlashcard;

        [ObservableProperty]
        private int currentIndex;

        [ObservableProperty]
        private string currentCardText;

        [ObservableProperty]
        private string cardCountDisplay;

        [ObservableProperty]
        private bool isNextButtonVisible;

        [ObservableProperty]
        private bool isPreviousButtonVisible;

        [ObservableProperty]
        private bool isAutoplayEnabled;

        [ObservableProperty]
        private bool isShuffleEnabled;

        public FlashcardSessionViewModel(IFlashcardDataStore dataStore)
        {
            _dataStore = dataStore;
            CurrentIndex = 0;
            UpdateNavigationVisibility();
        }

        private async void LoadFlashcards(int setId)
        {
            if (_dataStore != null)
            {
                var flashcardSet = await _dataStore.GetFlashcardSetAsync(setId);
                Title = flashcardSet.Title;
                var loadedFlashcards = (await _dataStore.GetFlashcardsForSetAsync(setId)).ToList();

                // Shuffle the cards if shuffle is enabled
                if (IsShuffleEnabled)
                {
                    loadedFlashcards = ShuffleFlashcards(loadedFlashcards);
                }

                Flashcards.Clear();
                foreach (Flashcard flashcard in loadedFlashcards)
                {
                    Flashcards.Add(flashcard);
                }
                CurrentIndex = 0;
                SetCurrentFlashcard(0); // Start with the first card
            }
        }

        private List<Flashcard> ShuffleFlashcards(List<Flashcard> cards)
        {
            var random = new Random();
            return cards.OrderBy(card => random.Next()).ToList();
        }

        private void SetCurrentFlashcard(int index)
        {
            if (Flashcards.Count > 0 && index >= 0 && index < Flashcards.Count)
            {
                CurrentFlashcard = Flashcards[index];
                CurrentCardText = displayTermFirst ? CurrentFlashcard.Question : CurrentFlashcard.Answer;
                CurrentIndex = index;
                CardCountDisplay = $"{CurrentIndex + 1} / {Flashcards.Count}";
                UpdateNavigationVisibility();
            }
        }

        [RelayCommand]
        private void FlipCard()
        {
            if (CurrentFlashcard != null)
            {
                CurrentCardText = CurrentCardText == CurrentFlashcard.Question ? CurrentFlashcard.Answer : CurrentFlashcard.Question;
            }
        }

        [RelayCommand]
        private void NextCard()
        {
            if (CurrentIndex < Flashcards.Count - 1)
            {
                SetCurrentFlashcard(CurrentIndex + 1);
            }
            else
            {
                GoBackCommand.Execute(null);
            }
        }

        [RelayCommand]
        private void PreviousCard()
        {
            if (CurrentIndex > 0)
            {
                SetCurrentFlashcard(CurrentIndex - 1);
            }
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        private void UpdateNavigationVisibility()
        {
            IsPreviousButtonVisible = CurrentIndex > 0;
            IsNextButtonVisible = CurrentIndex < Flashcards.Count - 1;
        }
    }
}
