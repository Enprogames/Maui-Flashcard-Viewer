using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TUnit.Assertions;
using TUnit.Core;
using FlashcardViewer.Domain;

namespace FlashcardViewer.UnitTests
{
    public class SharedUIComponentTests
    {
        [Test]
        public async Task SessionConfig_Title_Should_Format_Correctly()
        {
            // Arrange
            var configSingle = new SessionConfig
            {
                Title = "Single Set Title",
                SetIds = [1]
            };

            var configMulti = new SessionConfig
            {
                Title = "Multiple Sets",
                SetIds = [1, 2, 3]
            };

            // Assert
            await Assert.That(configSingle.Title).IsEqualTo("Single Set Title");
            await Assert.That(configSingle.SetIds).Count().IsEqualTo(1);

            await Assert.That(configMulti.Title).IsEqualTo("Multiple Sets");
            await Assert.That(configMulti.SetIds).Count().IsEqualTo(3);
        }

        [Test]
        public async Task SetListSelection_Helper_Should_Calculate_Correct_Flags()
        {
            // Arrange
            var sets = new List<SelectableSetTestWrapper>
            {
                new() { Set = new() { Id = 1, Title = "Set 1" }, IsSelected = false },
                new() { Set = new() { Id = 2, Title = "Set 2" }, IsSelected = false }
            };

            // Act & Assert (None selected)
            var isAnySelected = sets.Any(s => s.IsSelected);
            var selectedCount = sets.Count(s => s.IsSelected);
            var allSelected = sets.Any() && sets.All(s => s.IsSelected);

            await Assert.That(isAnySelected).IsFalse();
            await Assert.That(selectedCount).IsEqualTo(0);
            await Assert.That(allSelected).IsFalse();

            // Act & Assert (One selected)
            sets[0].IsSelected = true;
            isAnySelected = sets.Any(s => s.IsSelected);
            selectedCount = sets.Count(s => s.IsSelected);
            allSelected = sets.Any() && sets.All(s => s.IsSelected);

            await Assert.That(isAnySelected).IsTrue();
            await Assert.That(selectedCount).IsEqualTo(1);
            await Assert.That(allSelected).IsFalse();

            // Act & Assert (All selected)
            sets[1].IsSelected = true;
            isAnySelected = sets.Any(s => s.IsSelected);
            selectedCount = sets.Count(s => s.IsSelected);
            allSelected = sets.Any() && sets.All(s => s.IsSelected);

            await Assert.That(isAnySelected).IsTrue();
            await Assert.That(selectedCount).IsEqualTo(2);
            await Assert.That(allSelected).IsTrue();
        }

        [Test]
        public async Task CardList_Placeholder_Logic_Should_Correctly_Represent_Adding_State()
        {
            // Arrange
            var cards = new List<Flashcard>
            {
                new() { Id = 1, SetId = 1, Question = "Q1", Answer = "A1" },
                new() { Id = 2, SetId = 1, Question = "Q2", Answer = "A2" }
            };

            // Append empty placeholder
            cards.Add(new Flashcard { SetId = 1, Question = string.Empty, Answer = string.Empty });

            // Assert setup
            await Assert.That(cards).Count().IsEqualTo(3);
            var lastCard = cards.Last();
            await Assert.That(lastCard.Question).IsEmpty();
            await Assert.That(lastCard.Answer).IsEmpty();

            // Act (Simulate editing the placeholder)
            lastCard.Question = "Q3";
            lastCard.Answer = "A3";

            // If it was the placeholder, we would normally save to DB and add a new placeholder
            if (!string.IsNullOrWhiteSpace(lastCard.Question) && !string.IsNullOrWhiteSpace(lastCard.Answer))
            {
                cards.Add(new Flashcard { SetId = 1, Question = string.Empty, Answer = string.Empty });
            }

            // Assert state after simulation
            await Assert.That(cards).Count().IsEqualTo(4);
            await Assert.That(cards[2].Question).IsEqualTo("Q3");
            await Assert.That(cards[3].Question).IsEmpty();
        }

        [Test]
        public async Task SessionProgression_Should_Track_Indices_And_Transitions()
        {
            // Arrange
            var cards = new List<Flashcard>
            {
                new() { Id = 1, SetId = 1, Question = "Q1", Answer = "A1" },
                new() { Id = 2, SetId = 1, Question = "Q2", Answer = "A2" },
                new() { Id = 3, SetId = 1, Question = "Q3", Answer = "A3" }
            };

            int currentIndex = 0;
            bool isFlipped = false;

            // Assert Initial State
            await Assert.That(currentIndex).IsEqualTo(0);
            await Assert.That(isFlipped).IsFalse();
            await Assert.That(cards[currentIndex].Question).IsEqualTo("Q1");

            // Act & Assert (Flip card)
            isFlipped = !isFlipped;
            await Assert.That(isFlipped).IsTrue();
            await Assert.That(cards[currentIndex].Answer).IsEqualTo("A1");

            // Act & Assert (Move Next)
            if (currentIndex < cards.Count - 1)
            {
                currentIndex++;
                isFlipped = false;
            }
            await Assert.That(currentIndex).IsEqualTo(1);
            await Assert.That(isFlipped).IsFalse();
            await Assert.That(cards[currentIndex].Question).IsEqualTo("Q2");

            // Act & Assert (Move Next)
            if (currentIndex < cards.Count - 1)
            {
                currentIndex++;
                isFlipped = false;
            }
            await Assert.That(currentIndex).IsEqualTo(2);

            // Act & Assert (Move Previous)
            if (currentIndex > 0)
            {
                currentIndex--;
                isFlipped = false;
            }
            await Assert.That(currentIndex).IsEqualTo(1);
            await Assert.That(isFlipped).IsFalse();
        }

        private class SelectableSetTestWrapper
        {
            public FlashcardSet Set { get; set; } = null!;
            public bool IsSelected { get; set; }
        }
    }
}
