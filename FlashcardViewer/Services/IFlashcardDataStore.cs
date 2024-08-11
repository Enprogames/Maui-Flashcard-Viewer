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
