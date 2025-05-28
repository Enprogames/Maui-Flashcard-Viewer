using FlashcardViewer.Models;
using SQLite;

namespace FlashcardViewer.Services
{
    class LocalFlashcardDataStore : IFlashcardDataStore
    {
        private SQLiteAsyncConnection _database;

        public async Task InitializeAsync()
        {
            if (_database != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "flashcards.db");
            _database = new SQLiteAsyncConnection(databasePath);

            await _database.CreateTableAsync<FlashcardSet>();
            await _database.CreateTableAsync<Flashcard>();
        }

        public async Task<IEnumerable<FlashcardSet>> GetFlashcardSetsAsync()
        {
            var flashcardSets = await _database.Table<FlashcardSet>().ToListAsync();
            return flashcardSets.AsEnumerable();
        }

        public Task<FlashcardSet> GetFlashcardSetAsync(int id)
        {
            return _database.Table<FlashcardSet>().FirstOrDefaultAsync(f => f.Id == id);
        }

        public Task AddFlashcardSetAsync(FlashcardSet set)
        {
            return _database.InsertAsync(set);
        }

        public Task UpdateFlashcardSetAsync(FlashcardSet set)
        {
            return _database.UpdateAsync(set);
        }

        public Task DeleteFlashcardSetAsync(int id)
        {
            return _database.DeleteAsync<FlashcardSet>(id);
        }

        public Task AddFlashcardAsync(Flashcard flashcard)
        {
            return _database.InsertAsync(flashcard);
        }
        public Task UpdateFlashcardAsync(Flashcard flashcard)
        {
            return _database.UpdateAsync(flashcard);
        }

        public Task DeleteFlashcardAsync(int id)
        {
            return _database.DeleteAsync<Flashcard>(id);
        }

        public async Task<IEnumerable<Flashcard>> GetFlashcardsForSetAsync(int setId)
        {
            var flashcards = await _database.Table<Flashcard>().Where(f => f.SetId == setId).ToListAsync();
            return flashcards.AsEnumerable();
        }

        public async Task<string> SaveImageAsync(Stream imageStream, string fileName)
        {
            var imagePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            using (var newStream = File.OpenWrite(imagePath))
            {
                await imageStream.CopyToAsync(newStream);
            }
            return imagePath;
        }

        public Task DeleteImageAsync(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
            return Task.CompletedTask;
        }
    }
}
