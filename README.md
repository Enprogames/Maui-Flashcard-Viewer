# .NET Maui Flashcard Viewer

An application for creating and viewing flashcards, created using .NET Maui. Flashcards are stored in an sqlite database, or in the cloud (facilitated with aws lambda and dynamodb) if the user is logged in. Flashcards have questions and answers, but can also be presented to the user in both directions. They exist within sets so that they can be grouped by topic. The user can have a flashcard learning session which they customize to their needs. For instance, they might want the cards to be read aloud, or have them scroll automatically. They can also easily group several flashcard sets into a session. It is also possible to edit flashcards during a session. When they view their flashcard sets, it is easy and simple to edit the sets, add new cards, delete cards, or delete the entire set (after being shown a confirmation box). Flashcards can also have images on either side, or both sides. This application is primarily designed for the android smartphone, but also works well on desktop.

The local and cloud storage and not intended to be kept in-sync. They are meant to be mutually exclusive. If the user is logged in, they will use the cloud storage. If they are not logged in, they will use the local storage. The user can also switch between the two storage methods at any time. If they switch, the data that was previously displayed will no longer be shown in the application, as the other storage that they have switched to will instead be shown. But switching within the application won't delete any data stored in the respective database.

This application makes heavy use of ViewModels to ensure the frontend and backend code are separated. Shell navigation is also used for this application.

## Models
```c#
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardViewer.Models
{
    public class FlashcardSet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class Flashcard
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int SetId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string QuestionImage { get; set; } // Path to the image
        public string AnswerImage { get; set; } // Path to the image
    }
}
```


## View Models

ViewModels handle the logic and data-binding for the views, ensuring separation of concerns.
1. FlashcardSetListViewModel

    - Properties: ObservableCollection<FlashcardSet> FlashcardSets, FlashcardSet SelectedSet
    - Commands: LoadSetsCommand, AddSetCommand, DeleteSetCommand, EditSetCommand

2. FlashcardViewModel

    - Properties: ObservableCollection<Flashcard> Flashcards, Flashcard SelectedCard, bool IsEditMode
    - Commands: LoadFlashcardsCommand, AddCardCommand, DeleteCardCommand, EditCardCommand, SaveCardCommand

3. SessionViewModel

    - Properties: ObservableCollection<FlashcardSet> SelectedSets, bool IsAutoScroll, bool IsReadAloud
    - Commands: StartSessionCommand, StopSessionCommand, NextCardCommand, PreviousCardCommand

## Services

1. Data Store Services

    - SQLiteFlashcardDataStore: Implements IFlashcardDataStore for local storage using SQLite.
    - CloudFlashcardDataStore: Implements IFlashcardDataStore for cloud storage using AWS Lambda and DynamoDB.

2. User Authentication Service

    - IUserService: Handles user authentication and session management.

Both data storage methods (sqlite and cloud) use the same data model. Both methods implement them same interface:
```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlashcardViewer.Models;

namespace FlashcardViewer.Services
{
    public interface IFlashcardDataStore
    {
        Task InitializeAsync();
        Task<IEnumerable<FlashcardSet>> GetFlashcardSetsAsync();
        Task<FlashcardSet> GetFlashcardSetAsync(int id);
        Task AddFlashcardSetAsync(FlashcardSet set);
        Task UpdateFlashcardSetAsync(FlashcardSet set);
        Task DeleteFlashcardSetAsync(int id);
        Task<IEnumerable<Flashcard>> GetFlashcardsForSetAsync(int setId);
        Task AddFlashcardAsync(Flashcard flashcard);
        Task UpdateFlashcardAsync(Flashcard flashcard);
        Task DeleteFlashcardAsync(int id);
        Task<string> SaveImageAsync(Stream imageStream, string fileName);
        Task DeleteImageAsync(string imagePath);
    }
}
```

## Views

1. DashboardPage

    - Components: Displays available flashcard sets, with options to add, edit, or delete sets.

2. FlashcardSetPage

    - Components: Displays flashcards in a selected set, with options to add, edit, or delete cards.

3. FlashcardSessionPage

    - Components: Displays flashcards for study sessions, with options for automatic scrolling and reading aloud.

4. LoginPage

    - Components: Handles user authentication for cloud access.
