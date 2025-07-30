
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HeroesCardEditor.ViewModels;

/// <summary>
/// Search option for the editor (See EditorViewModel.cs).
/// </summary>
internal partial class FileEditorSearch : FileEditorFilter
{
    /// <summary>
    /// Currently opened editor (provided from the opened menu).
    /// </summary>
    [ObservableProperty]
    public partial FileEditor Editor { get; set; }

    private int _index;
    private readonly Collection<int> _descriptors = [];

    /// <summary>
    /// Selects the first ObservableCardDescriptor in FileEditor.
    /// Doesn't select anything if not found.
    /// </summary>
    public void Search(Window window)
    {
        if (_descriptors.Count != 0)
        {
            _index++;
            if (_index == _descriptors.Count)
            {
                _index = 0;
            }
        }
        else if (window.ContextFlyout is Flyout flyout)
        {
            flyout.Content = "Not found.";
            flyout.ShowAt(window);
        }

        /// Setting Editor.SelectedIndex does nothing, because it serves another purpose if I remember correctly.
        if (_index > -1)
        {
            Editor.SelectedTab = Editor.Entries.ElementAtOrDefault(_descriptors[_index]);
        }
    }

    /// <summary>
    /// Internal filter that saves the eligible entries' indices.
    /// Originally I saved the entries themselves, but it isn't worth it.
    /// </summary>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        _descriptors.Clear();
        _index = -1;

        for (int i = 0; i < Editor.Entries.Count; i++)
        {
            if (BuildFilter(Editor.Entries[i]))
            {
                _descriptors.Add(i);
            }
        }

        base.OnPropertyChanged(e);
    }
}
