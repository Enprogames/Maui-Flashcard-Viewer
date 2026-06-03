using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using FlashcardViewer.Application.Interfaces;
using FlashcardViewer.Domain;

namespace FlashcardViewer.Infrastructure.Sqlite;

public class SqliteFlashcardRepository : IFlashcardRepository
{
    private readonly SQLiteAsyncConnection _database;

    public SqliteFlashcardRepository(SQLiteAsyncConnection database)
    {
        _database = database;
    }

    public async Task<IEnumerable<FlashcardSet>> GetFlashcardSetsAsync()
    {
        var sets = await _database.Table<FlashcardSet>().ToListAsync();
        return sets.AsEnumerable();
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
        var list = await _database.Table<Flashcard>().Where(f => f.SetId == setId).ToListAsync();
        return list.AsEnumerable();
    }
}
