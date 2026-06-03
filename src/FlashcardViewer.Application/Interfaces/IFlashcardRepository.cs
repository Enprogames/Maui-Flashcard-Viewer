using System.Collections.Generic;
using System.Threading.Tasks;
using FlashcardViewer.Domain;

namespace FlashcardViewer.Application.Interfaces
{
    public interface IFlashcardRepository
    {
        Task<IEnumerable<FlashcardSet>> GetFlashcardSetsAsync();
        Task<FlashcardSet> GetFlashcardSetAsync(int id);
        Task AddFlashcardSetAsync(FlashcardSet set);
        Task UpdateFlashcardSetAsync(FlashcardSet set);
        Task DeleteFlashcardSetAsync(int id);
        Task<IEnumerable<Flashcard>> GetFlashcardsForSetAsync(int setId);
        Task AddFlashcardAsync(Flashcard flashcard);
        Task UpdateFlashcardAsync(Flashcard flashcard);
        Task DeleteFlashcardAsync(int id);
    }
}
