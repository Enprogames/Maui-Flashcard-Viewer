using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlashcardViewer.ViewModels
{
    public partial class SessionConfigManagementViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isPopupVisible;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string confirmButtonText;

        [ObservableProperty]
        private SessionConfigViewModel sessionConfig;

        public IAsyncRelayCommand ?ConfirmButtonCommand { get; set; }

        public IAsyncRelayCommand ConfirmButtonCommandWrapper { get; set; }

        public SessionConfigManagementViewModel() { }

        public SessionConfigManagementViewModel(
            string popupTitle,
            string _confirmButtonText,
            IAsyncRelayCommand ?_confirmButtonCommand,
            SessionConfigViewModel _sessionConfig)
        {
            Title = popupTitle;
            ConfirmButtonText = _confirmButtonText;
            ConfirmButtonCommand = _confirmButtonCommand;

            if (ConfirmButtonCommand == null)
            {
                ConfirmButtonCommandWrapper = new AsyncRelayCommand(async () =>
                {
                    IsPopupVisible = false;
                }
                );
            }
            else
            {
                ConfirmButtonCommandWrapper = new AsyncRelayCommand(async () =>
                {
                    IsPopupVisible = false;
                    await ConfirmButtonCommand.ExecuteAsync(null);
                });
            }
            SessionConfig = _sessionConfig;
        }
    }
}
