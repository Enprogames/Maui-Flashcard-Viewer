using FlashcardViewer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

using FlashcardViewer.Views;
using FlashcardViewer.Models;
using CommunityToolkit.Mvvm.Input;

namespace FlashcardViewer.ViewModels
{
    [QueryProperty(nameof(SessionConfig), "SessionConfig")]
    public partial class FlashcardSessionViewModel : ObservableObject
    {
        private IFlashcardDataStore dataStore;

        [ObservableProperty]
        private SessionConfigViewModel sessionConfig;

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
        private string nextButtonIcon;

        [ObservableProperty]
        private bool isPreviousButtonVisible;

        public FlashcardSessionViewModel(IFlashcardDataStore _dataStore)
        {
            dataStore = _dataStore;
            CurrentIndex = 0;

            UpdateNavigationButtonVisibility();
        }
        
        public async Task LoadSetsAsync()
        {
            if (SessionConfig == null)
            {
                throw new Exception("SessionConfig is null. It must be passed in as a QueryProperty when navigating to this page.");
            }

            if (dataStore != null)
            {
                var allFlashcards = new List<Flashcard>();

                var setIds = SessionConfig.SetIds;

                if (setIds.Count() == 1)
                {
                    var flashcardSet = await dataStore.GetFlashcardSetAsync(setIds[0]);
                    Title = flashcardSet.Title;
                }
                else
                {
                    Title = "Multiple Sets";
                }
                foreach (var setId in setIds)
                {
                    var loadedFlashcards = (await dataStore.GetFlashcardsForSetAsync(setId)).ToList();
                    allFlashcards.AddRange(loadedFlashcards);
                }

                // Shuffle the cards if shuffle is enabled
                if (SessionConfig.IsShuffleEnabled)
                {
                    allFlashcards = ShuffleFlashcards(allFlashcards);
                }

                Flashcards.Clear();
                foreach (Flashcard flashcard in allFlashcards)
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
                CurrentCardText = SessionConfig.DisplayTermFirst ? CurrentFlashcard.Question : CurrentFlashcard.Answer;
                CurrentIndex = index;
                CardCountDisplay = $"{CurrentIndex + 1} / {Flashcards.Count}";
                UpdateNavigationButtonVisibility();
            }
        }

        [RelayCommand]
        async Task StartSession()
        {
            await Shell.Current.GoToAsync(nameof(FlashcardSessionPage));
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

        private void UpdateNavigationButtonVisibility()
        {
            IsPreviousButtonVisible = CurrentIndex > 0;
            NextButtonIcon = CurrentIndex < Flashcards.Count - 1 ? "&gt;" : "✓";
        }
    }
}
