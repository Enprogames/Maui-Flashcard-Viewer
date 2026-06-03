using System.Threading.Tasks;
using TUnit.Assertions;
using TUnit.Core;
using FlashcardViewer.Domain;

namespace FlashcardViewer.UnitTests
{
    public class DomainModelTests
    {
        [Test]
        public async Task FlashcardSet_Should_Instantiate_With_Correct_Values()
        {
            // Arrange & Act
            var set = new FlashcardSet
            {
                Id = 1,
                Title = "Test Title",
                Description = "Test Description"
            };

            // Assert
            await Assert.That(set.Id).IsEqualTo(1);
            await Assert.That(set.Title).IsEqualTo("Test Title");
            await Assert.That(set.Description).IsEqualTo("Test Description");
        }

        [Test]
        public async Task Flashcard_Should_Instantiate_With_Correct_Values()
        {
            // Arrange & Act
            var card = new Flashcard
            {
                Id = 42,
                SetId = 2,
                Question = "What is .NET?",
                Answer = "A developer platform.",
                QuestionImage = "q.png",
                AnswerImage = "a.png"
            };

            // Assert
            await Assert.That(card.Id).IsEqualTo(42);
            await Assert.That(card.SetId).IsEqualTo(2);
            await Assert.That(card.Question).IsEqualTo("What is .NET?");
            await Assert.That(card.Answer).IsEqualTo("A developer platform.");
            await Assert.That(card.QuestionImage).IsEqualTo("q.png");
            await Assert.That(card.AnswerImage).IsEqualTo("a.png");
        }

        [Test]
        public async Task Flashcard_Should_Raise_PropertyChanged_Event_On_Mutation()
        {
            // Arrange
            var card = new Flashcard { Question = "Original Question" };
            var propertyChangedRaised = false;
            
            card.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Flashcard.Question))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            card.Question = "New Question";

            // Assert
            await Assert.That(propertyChangedRaised).IsTrue();
        }
    }
}
