
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HeroesCardEditor.Models;

/// <summary>
/// Recently opened file.
/// </summary>
public partial class RecentFile(string bookmark, string fileName, Uri path, DateTime lastOpenedAt) : ObservableRecipient
{
    /// <summary>
    /// Avalonia Storage bookmark, representing the file access point.
    /// </summary>
    [ObservableProperty]
    public partial string Bookmark { get; set; } = bookmark;

    /// <summary>
    /// File name (for pretty printing)
    /// </summary>
    [ObservableProperty]
    public partial string FileName { get; set; } = fileName;

    /// <summary>
    /// File path (in Uri format).
    /// </summary>
    [ObservableProperty]
    public partial Uri Path { get; set; } = path;

    /// <summary>
    /// The last time this file was opened.
    /// </summary>
    [ObservableProperty]
    public partial DateTime LastOpenedAt { get; set; } = lastOpenedAt;
}
