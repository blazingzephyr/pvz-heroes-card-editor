
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HeroesCardEditor.Models;

/// <summary>
/// Recently opened file.
/// </summary>
internal partial class RecentFile(string bookmark, string fileName, Uri path, string? locBookmark, string? LocFileName, Uri? locPath, DateTime lastOpenedAt) : ObservableRecipient
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
    /// Avalonia Storage bookmark, representing the localization file access point.
    /// </summary>
    [ObservableProperty]
    public partial string? LocBookmark { get; set; } = locBookmark;

    /// <summary>
    /// Localization file name (for pretty printing)
    /// </summary>
    [ObservableProperty]
    public partial string? LocFileName { get; set; } = LocFileName;

    /// <summary>
    /// Localization file path (in Uri format).
    /// </summary>
    [ObservableProperty]
    public partial Uri? LocPath { get; set; } = locPath;

    /// <summary>
    /// The last time this file was opened.
    /// </summary>
    [ObservableProperty]
    public partial DateTime LastOpenedAt { get; set; } = lastOpenedAt;
}
