using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlashcardViewer.Models;
using SQLite;

namespace FlashcardViewer.Services
{
    class CloudFlashcardDataStore : IFlashcardDataStore
    {
        public Task AddFlashcardAsync(Flashcard flashcard)
        {
            throw new NotImplementedException();
        }

        public Task AddFlashcardSetAsync(FlashcardSet set)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFlashcardAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFlashcardSetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteImageAsync(string imagePath)
        {
            throw new NotImplementedException();
        }

        public Task<FlashcardSet> GetFlashcardSetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FlashcardSet>> GetFlashcardSetsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Flashcard>> GetFlashcardsForSetAsync(int setId)
        {
            throw new NotImplementedException();
        }

        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> SaveImageAsync(Stream imageStream, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task UpdateFlashcardAsync(Flashcard flashcard)
        {
            throw new NotImplementedException();
        }

        public Task UpdateFlashcardSetAsync(FlashcardSet set)
        {
            throw new NotImplementedException();
        }
    }
}
