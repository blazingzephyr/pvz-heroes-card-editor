
using CommunityToolkit.Mvvm.ComponentModel;

namespace HeroesCardEditor.Models;

/// <summary>
/// General application preferences.
/// </summary>
public partial class Preferences : ObservableRecipient
{
    /// <summary>
    /// Determines whether to auto-save files on every change.
    /// </summary>
    [ObservableProperty]
    public partial bool AutoSave { get; set; }

    /// <summary>
    /// Determines whether the saved JSONs should be indented or not.
    /// </summary>
    [ObservableProperty]
    public partial bool WriteIndented { get; set; }
}
