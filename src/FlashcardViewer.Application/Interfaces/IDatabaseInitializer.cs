using System.Threading.Tasks;

namespace FlashcardViewer.Application.Interfaces
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }
}
