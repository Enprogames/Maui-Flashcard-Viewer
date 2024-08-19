using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlashcardViewer.ViewModels
{
    public partial class SessionConfigViewModel : ObservableObject
    {
        [ObservableProperty]
        public string title;

        [ObservableProperty]
        public ObservableCollection<int> setIds = new ObservableCollection<int>();

        [ObservableProperty]
        public bool displayTermFirst;

        [ObservableProperty]
        public bool isAutoplayEnabled;

        [ObservableProperty]
        public bool isShuffleEnabled;

        [ObservableProperty]
        public bool isReadAloudEnabled;

        [ObservableProperty]
        public bool isPopupVisible;

        public SessionConfigViewModel(string title, List<int> _setIds)
        {

            SetIds = new ObservableCollection<int>(_setIds);
            Title = _setIds.Count > 1 ? "Multiple Sets" : "Single Set";
        }
    }
}
