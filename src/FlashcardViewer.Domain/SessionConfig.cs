using System.Collections.Generic;

namespace FlashcardViewer.Domain;

public class SessionConfig
{
    public string Title { get; set; } = string.Empty;
    public List<int> SetIds { get; set; } = [];
    public bool DisplayTermFirst { get; set; } = true;
    public bool IsAutoplayEnabled { get; set; }
    public bool IsShuffleEnabled { get; set; }
    public bool IsReadAloudEnabled { get; set; }
}
