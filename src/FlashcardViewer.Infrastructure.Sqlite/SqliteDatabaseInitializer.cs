using System.Threading.Tasks;
using SQLite;
using FlashcardViewer.Application.Interfaces;
using FlashcardViewer.Domain;

namespace FlashcardViewer.Infrastructure.Sqlite;

public class SqliteDatabaseInitializer : IDatabaseInitializer
{
    private readonly SQLiteAsyncConnection _database;

    public SqliteDatabaseInitializer(SQLiteAsyncConnection database)
    {
        _database = database;
    }

    public async Task InitializeAsync()
    {
        await _database.CreateTableAsync<FlashcardSet>();
        await _database.CreateTableAsync<Flashcard>();
    }
}
