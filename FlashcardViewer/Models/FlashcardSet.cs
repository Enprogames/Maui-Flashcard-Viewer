using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardViewer.Models
{
    public partial class FlashcardSet : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ObservableProperty]
        public string title;

        [ObservableProperty]
        public string description;
    }

    public partial class Flashcard : ObservableObject
    {
        private int id;
        private int setId;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public int SetId
        {
            get => setId;
            set => SetProperty(ref setId, value);
        }

        [ObservableProperty]
        public string question;

        [ObservableProperty]
        public string answer;

        [ObservableProperty]
        public string questionImage;

        [ObservableProperty]
        public string answerImage;
    }
}
