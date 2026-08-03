using System.IO;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HeroesCardEditor.ViewModels;

/// <summary>
/// Built-in raw text file editor window.
/// </summary>
internal partial class RawEditorViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial IStorageFile File { get; set; }

    public TextDocument FileText { get; set; }

    public bool Edited { get; private set; }

    public RawEditorViewModel(IStorageFile file)
    {
        using Stream fileStream = file.OpenReadAsync().GetAwaiter().GetResult();
        using StreamReader reader = new StreamReader(fileStream);
        string text = reader.ReadToEnd();

        File = file;
        FileText = new TextDocument(text);
    }

    /// <summary>
    /// Saves any recorded changes within the currently selected file.
    /// </summary>
    public void SaveFile()
    {
        using Stream fileStream = File.OpenWriteAsync().GetAwaiter().GetResult();
        using StreamWriter writer = new StreamWriter(fileStream);
        writer.WriteLine(FileText.Text);
        Edited = true;
    }
}
