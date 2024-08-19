using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

using FlashcardViewer.Views;
using FlashcardViewer.Models;
using FlashcardViewer.Services;
using CommunityToolkit.Mvvm.Input;

namespace FlashcardViewer.ViewModels
{
    public partial class FlashcardSetListViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<SelectableFlashcardSet> flashcardSets = new ObservableCollection<SelectableFlashcardSet>();

        [ObservableProperty]
        private string storageModeText;

        private readonly IDataStoreService _dataStoreService;
        private IFlashcardDataStore _dataStore;

        [ObservableProperty]
        private bool isAnySetSelected;

        private bool _isUpdatingSelectAll;

        private bool selectAllSets;
        public bool SelectAllSets
        {
            get => selectAllSets;
            set
            {
                if (SetProperty(ref selectAllSets, value))
                {
                    if (!_isUpdatingSelectAll)
                    {
                        ToggleSelectAllSets(value);
                    }
                }
            }
        }

        public IAsyncRelayCommand<string> AddFlashcardSetCommand { get; private set; }
        public IAsyncRelayCommand DeleteSelectedSetsCommand { get; private set; }

        public FlashcardSetListViewModel(IDataStoreService dataStoreService)
        {
            _dataStoreService = dataStoreService;
            _dataStore = _dataStoreService.GetDataStore();

            StorageModeText = _dataStore is CloudFlashcardDataStore ? "Data stored in the cloud" : "Data stored offline";

            AddFlashcardSetCommand = new AsyncRelayCommand<string>(
                async (p) => AddFlashcardSetAsync(p),
                canExecute => true
            );
            DeleteSelectedSetsCommand = new AsyncRelayCommand(
                async () => await DeleteSelectedSets()
            );

            // Initialize Flashcard Sets
            Task.Run(InitializeAsync);
        }

        public async Task InitializeAsync()
        {
            await _dataStore.InitializeAsync();
            var flashcardSets = (await _dataStore.GetFlashcardSetsAsync()).ToList();
            foreach (var set in flashcardSets)
            {
                var selectableSet = new SelectableFlashcardSet(set);
                selectableSet.PropertyChanged += SelectableSet_PropertyChanged;
                FlashcardSets.Add(selectableSet);
            }
            UpdateSelectionState();
        }

        private void SelectableSet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectableFlashcardSet.IsSelected))
            {
                UpdateSelectionState();
            }
        }

        private async void AddFlashcardSetAsync(string setName)
        {
            if (!string.IsNullOrEmpty(setName))
            {
                FlashcardSet newSet = new FlashcardSet { Title = setName };
                await _dataStore.AddFlashcardSetAsync(newSet);

                var selectableSet = new SelectableFlashcardSet(newSet);
                selectableSet.PropertyChanged += SelectableSet_PropertyChanged;
                FlashcardSets.Add(selectableSet);
            }
        }

        private void UpdateSelectionState()
        {
            IsAnySetSelected = FlashcardSets.Any(set => set.IsSelected);
            bool allSelected = FlashcardSets.All(set => set.IsSelected);

            _isUpdatingSelectAll = true;
            SelectAllSets = allSelected;
            _isUpdatingSelectAll = false;
        }

        private void ToggleSelectAllSets(bool value)
        {
            foreach (var set in FlashcardSets)
            {
                // Disable the PropertyChanged event handler while setting the property
                set.PropertyChanged -= SelectableSet_PropertyChanged;
                set.IsSelected = value;
                set.PropertyChanged += SelectableSet_PropertyChanged;
            }
            UpdateSelectionState();
        }

        private async Task DeleteSelectedSets()
        {
            var selectedSets = FlashcardSets.Where(set => set.IsSelected).ToList();
            if (App.Current != null && App.Current.MainPage != null && selectedSets.Any())
            {
                var result = await App.Current.MainPage.DisplayAlert(
                    "Confirm Deletion",
                    $"Are you sure? All of the following flashcard set(s) will be deleted:\n{string.Join(",\n", selectedSets.Select(set => set.Title))}",
                    "Yes", "No");

                if (result)
                {
                    foreach (var set in selectedSets)
                    {
                        await _dataStore.DeleteFlashcardSetAsync(set.Id);
                        FlashcardSets.Remove(set);
                    }
                    UpdateSelectionState();
                }
            }
        }

        [RelayCommand]
        async Task NavigateToFlashcardList(FlashcardSet set)
        {
            if (set != null)
            {
                await Shell.Current.GoToAsync($"{nameof(FlashcardListPage)}?setId={set.Id}");
            }
        }
    }

    public partial class SelectableFlashcardSet : FlashcardSet
    {
        [ObservableProperty]
        public bool isSelected;

        public SelectableFlashcardSet(FlashcardSet set)
        {
            Id = set.Id;
            Title = set.Title;
            Description = set.Description;
        }
    }
}
