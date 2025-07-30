
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HeroesCardEditor.ViewModels;

/// <summary>
/// The prompt that shows when you are trying to close a file without saving.
/// </summary>
internal partial class ClosingPromptViewModel(EditorViewModel editor, FileEditor file) : ObservableRecipient
{
    /// <summary>
    /// The parent editor this was called from.
    /// </summary>
    [ObservableProperty]
    public partial EditorViewModel Editor { get; private set; } = editor;

    /// <summary>
    /// The file we are trying to close.
    /// </summary>
    [ObservableProperty]
    public partial FileEditor File { get; private set; } = file;

    /// <summary>
    /// Saves and closes the file, then closes this window.
    /// </summary>
    public void SaveAndClose(Window window)
    {
        Editor.SaveAndClose(File);
        window.Close();
    }

    /// <summary>
    /// Closes the file without saving, then closes this window.
    /// </summary>
    public void Close(Window window)
    {
        Editor.Close(File);
        window.Close();
    }
}
