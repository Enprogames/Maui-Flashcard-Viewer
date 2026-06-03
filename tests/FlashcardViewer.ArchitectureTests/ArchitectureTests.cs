using System.Threading.Tasks;
using TUnit.Assertions;
using TUnit.Core;
using NetArchTest.Rules;

namespace FlashcardViewer.ArchitectureTests
{
    public class ArchitectureTests
    {
        [Test]
        public async Task Domain_Should_Not_Have_Dependency_On_Other_Projects()
        {
            var result = Types.InAssembly(typeof(FlashcardViewer.Domain.FlashcardSet).Assembly)
                .ShouldNot()
                .HaveDependencyOn("FlashcardViewer.Application")
                .GetResult();

            await Assert.That(result.IsSuccessful).IsTrue();
        }

        [Test]
        public async Task Application_Should_Not_Have_Dependency_On_Infrastructure_Or_UI()
        {
            var result = Types.InAssembly(typeof(FlashcardViewer.Application.Interfaces.IFlashcardRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn("FlashcardViewer.Maui")
                .And()
                .HaveDependencyOn("FlashcardViewer.Wasm")
                .GetResult();

            await Assert.That(result.IsSuccessful).IsTrue();
        }
    }
}
