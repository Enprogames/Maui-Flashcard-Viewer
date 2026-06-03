using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using FlashcardViewer.Application.Interfaces;
using FlashcardViewer.Domain;

namespace FlashcardViewer.Infrastructure.Web;

public class WebFlashcardRepository : IFlashcardRepository
{
    private readonly IJSRuntime _jsRuntime;
    private const string SetsKey = "flashcard_sets";
    private const string CardsKey = "flashcards";

    public WebFlashcardRepository(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task<List<FlashcardSet>> LoadSetsAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", SetsKey);
            return string.IsNullOrEmpty(json) ? [] : JsonSerializer.Deserialize<List<FlashcardSet>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private async Task SaveSetsAsync(List<FlashcardSet> sets)
    {
        var json = JsonSerializer.Serialize(sets);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", SetsKey, json);
    }

    private async Task<List<Flashcard>> LoadCardsAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", CardsKey);
            return string.IsNullOrEmpty(json) ? [] : JsonSerializer.Deserialize<List<Flashcard>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private async Task SaveCardsAsync(List<Flashcard> cards)
    {
        var json = JsonSerializer.Serialize(cards);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", CardsKey, json);
    }

    public async Task<IEnumerable<FlashcardSet>> GetFlashcardSetsAsync()
    {
        return await LoadSetsAsync();
    }

    public async Task<FlashcardSet> GetFlashcardSetAsync(int id)
    {
        var sets = await LoadSetsAsync();
        return sets.FirstOrDefault(s => s.Id == id);
    }

    public async Task AddFlashcardSetAsync(FlashcardSet set)
    {
        var sets = await LoadSetsAsync();
        set.Id = sets.Count > 0 ? sets.Max(s => s.Id) + 1 : 1;
        sets.Add(set);
        await SaveSetsAsync(sets);
    }

    public async Task UpdateFlashcardSetAsync(FlashcardSet set)
    {
        var sets = await LoadSetsAsync();
        var index = sets.FindIndex(s => s.Id == set.Id);
        if (index != -1)
        {
            sets[index] = set;
            await SaveSetsAsync(sets);
        }
    }

    public async Task DeleteFlashcardSetAsync(int id)
    {
        var sets = await LoadSetsAsync();
        sets.RemoveAll(s => s.Id == id);
        await SaveSetsAsync(sets);

        // Cascade delete cards
        var cards = await LoadCardsAsync();
        cards.RemoveAll(c => c.SetId == id);
        await SaveCardsAsync(cards);
    }

    public async Task AddFlashcardAsync(Flashcard flashcard)
    {
        var cards = await LoadCardsAsync();
        flashcard.Id = cards.Count > 0 ? cards.Max(c => c.Id) + 1 : 1;
        cards.Add(flashcard);
        await SaveCardsAsync(cards);
    }

    public async Task UpdateFlashcardAsync(Flashcard flashcard)
    {
        var cards = await LoadCardsAsync();
        var index = cards.FindIndex(c => c.Id == flashcard.Id);
        if (index != -1)
        {
            cards[index] = flashcard;
            await SaveCardsAsync(cards);
        }
    }

    public async Task DeleteFlashcardAsync(int id)
    {
        var cards = await LoadCardsAsync();
        cards.RemoveAll(c => c.Id == id);
        await SaveCardsAsync(cards);
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsForSetAsync(int setId)
    {
        var cards = await LoadCardsAsync();
        return cards.Where(c => c.SetId == setId);
    }
}
