using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FlashcardViewer.Views;
using FlashcardViewer.Services;
using FlashcardViewer.Models;

namespace FlashcardViewer.ViewModels
{
    [QueryProperty(nameof(SetId), "setId")]
    public partial class FlashcardListViewModel : ObservableObject
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
        private string title;

        [ObservableProperty]
        private ObservableCollection<Flashcard> flashcards = new ObservableCollection<Flashcard>();

        [ObservableProperty]
        private string newQuestion;

        [ObservableProperty]
        private string newAnswer;

        public FlashcardListViewModel(IFlashcardDataStore dataStore)
        {
            _dataStore = dataStore;
            flashcards.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (Flashcard item in e.NewItems)
                    {
                        item.PropertyChanged += FlashcardPropertyChanged;
                    }
                }
            };
        }

        private async void LoadFlashcards(int setId)
        {
            if (_dataStore != null)
            {
                var flashcardSet = await _dataStore.GetFlashcardSetAsync(setId);
                Title = flashcardSet.Title;
                var newFlashcards = (await _dataStore.GetFlashcardsForSetAsync(setId)).ToList();
                Flashcards.Clear();
                foreach (Flashcard flashcard in newFlashcards)
                {
                    Flashcards.Add(flashcard);
                }
                AddEmptyFlashcardEntry();
            }
            else
            {
                Flashcards.Clear();
            }
        }

        private void AddEmptyFlashcardEntry()
        {
            Flashcards.Add(new Flashcard { Question = string.Empty, Answer = string.Empty });
        }

        private async void FlashcardPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Flashcard flashcard && !string.IsNullOrWhiteSpace(flashcard.Question) && !string.IsNullOrWhiteSpace(flashcard.Answer))
            {
                if (Flashcards.Last() == flashcard)
                {
                    var newFlashcard = new Flashcard
                    {
                        SetId = SetId,
                        Question = flashcard.Question,
                        Answer = flashcard.Answer
                    };

                    await _dataStore.AddFlashcardAsync(newFlashcard);

                    // Replace the entry with the saved flashcard (keeps IDs consistent)
                    Flashcards.Remove(flashcard);
                    Flashcards.Insert(Flashcards.Count, newFlashcard);
                    AddEmptyFlashcardEntry();
                }
                else
                {
                    // Update existing flashcard
                    await _dataStore.UpdateFlashcardAsync(flashcard);
                }
            }
        }

        [RelayCommand]
        async Task StartSession()
        {
            await Shell.Current.GoToAsync($"{nameof(FlashcardSessionPage)}?setId={SetId}");
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
