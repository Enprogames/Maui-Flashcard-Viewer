using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using TUnit.Assertions;
using TUnit.Core;
using Microsoft.JSInterop;
using System.Collections.Generic;
using FlashcardViewer.Domain;
using FlashcardViewer.Infrastructure.Sqlite;
using FlashcardViewer.Infrastructure.Web;

namespace FlashcardViewer.UnitTests;

public class MockJSRuntime : IJSRuntime
{
    public Dictionary<string, string> Storage { get; } = [];

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        if (identifier == "localStorage.getItem" && args != null && args.Length > 0)
        {
            var key = args[0] as string;
            if (key != null && Storage.TryGetValue(key, out var val))
            {
                return ValueTask.FromResult((TValue)(object)val);
            }
            return ValueTask.FromResult(default(TValue)!);
        }
        if (identifier == "localStorage.setItem" && args != null && args.Length > 1)
        {
            var key = args[0] as string;
            var val = args[1] as string;
            if (key != null && val != null)
            {
                Storage[key] = val;
            }
            return ValueTask.FromResult(default(TValue)!);
        }
        return ValueTask.FromResult(default(TValue)!);
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        return InvokeAsync<TValue>(identifier, args);
    }
}

public class PersistenceTests
{
    private string _tempDbPath = string.Empty;

    [Before(Test)]
    public void SetUp()
    {
        _tempDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.db");
    }

    [After(Test)]
    public void TearDown()
    {
        if (File.Exists(_tempDbPath))
        {
            try
            {
                File.Delete(_tempDbPath);
            }
            catch
            {
                // Ignored
            }
        }
    }

    [Test]
    public async Task Sqlite_Repository_Should_Perform_CRUD_Operations()
    {
        // Arrange
        var connection = new SQLiteAsyncConnection(_tempDbPath);
        var initializer = new SqliteDatabaseInitializer(connection);
        await initializer.InitializeAsync();

        var repo = new SqliteFlashcardRepository(connection);

        var newSet = new FlashcardSet
        {
            Title = "Chemistry 101",
            Description = "Intro to chemistry"
        };

        // Act & Assert: Add Set
        await repo.AddFlashcardSetAsync(newSet);
        var sets = (await repo.GetFlashcardSetsAsync()).ToList();
        await Assert.That(sets.Count).IsEqualTo(1);
        await Assert.That(sets[0].Title).IsEqualTo("Chemistry 101");

        // Act & Assert: Add Flashcard
        var newCard = new Flashcard
        {
            SetId = newSet.Id,
            Question = "What is the atomic number of Hydrogen?",
            Answer = "1"
        };
        await repo.AddFlashcardAsync(newCard);

        var cards = (await repo.GetFlashcardsForSetAsync(newSet.Id)).ToList();
        await Assert.That(cards.Count).IsEqualTo(1);
        await Assert.That(cards[0].Question).IsEqualTo("What is the atomic number of Hydrogen?");

        // Cleanup connection
        await connection.CloseAsync();
    }

    [Test]
    public async Task Web_Repository_Should_Perform_CRUD_Operations()
    {
        // Arrange
        var mockJs = new MockJSRuntime();
        var repo = new WebFlashcardRepository(mockJs);

        var newSet = new FlashcardSet
        {
            Title = "World History",
            Description = "Major historical events"
        };

        // Act & Assert: Add Set
        await repo.AddFlashcardSetAsync(newSet);
        var sets = (await repo.GetFlashcardSetsAsync()).ToList();
        await Assert.That(sets.Count).IsEqualTo(1);
        await Assert.That(sets[0].Title).IsEqualTo("World History");

        // Act & Assert: Add Flashcard
        var newCard = new Flashcard
        {
            SetId = sets[0].Id,
            Question = "When did WW2 end?",
            Answer = "1945"
        };
        await repo.AddFlashcardAsync(newCard);

        var cards = (await repo.GetFlashcardsForSetAsync(sets[0].Id)).ToList();
        await Assert.That(cards.Count).IsEqualTo(1);
        await Assert.That(cards[0].Answer).IsEqualTo("1945");
    }
}
