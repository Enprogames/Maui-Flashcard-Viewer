using FlashcardViewer.Domain;

namespace FlashcardViewer.SharedUI.Services;

public class SessionStateService
{
    public SessionConfig? CurrentConfig { get; set; }
}
