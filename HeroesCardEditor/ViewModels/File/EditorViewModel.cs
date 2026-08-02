
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using HeroesCardEditor.Models;
using PvZCards.Engine;
using ReactiveUI;

namespace HeroesCardEditor.ViewModels;

/// <summary>
/// Main editor window.
/// </summary>
internal partial class EditorViewModel : ObservableRecipient, ITabContainer
{
    /// <summary>
    /// The editor that's currently opened.
    /// Needed for the filter, sorting, file-saving and etc...
    /// </summary>
    [ObservableProperty]
    public partial FileEditor? SelectedFile { get; set; }

    /// <summary>
    /// Currently opened files.
    /// </summary>
    public ReadOnlyObservableCollection<FileEditor> OpenedFiles => _editors;

    /// <summary>
    /// Recently-opened files.
    /// </summary>
    public ReadOnlyObservableCollection<RecentFile> Recents => _recents;

    private JsonSerializerOptions _options;
    private readonly Preferences _prefs;
    private readonly FilePickerOpenOptions _fileOpenOptions;
    private readonly FilePickerSaveOptions _fileSaveOptions;
    private readonly SourceCache<FileEditor, Uri> _editorCache;
    private readonly SourceCache<RecentFile, Uri> _recentCache;
    private readonly ReadOnlyObservableCollection<FileEditor> _editors;
    private readonly ReadOnlyObservableCollection<RecentFile> _recents;

    public EditorViewModel(IStorageFile prefsFile, IStorageFile recentFile, Preferences prefs, Collection<RecentFile> recents)
    {
        var json = new FilePickerFileType("JSON")
        {
            Patterns = ["*.json"],
            AppleUniformTypeIdentifiers = ["public.json"],
            MimeTypes = ["json/*"]
        };

        _fileOpenOptions = new FilePickerOpenOptions
        {
            Title = "Open cards.json File",
            AllowMultiple = true,
            FileTypeFilter = [json]
        };

        _fileSaveOptions = new FilePickerSaveOptions
        {
            Title = "Save As",
            FileTypeChoices = [new FilePickerFileType("JSON") { Patterns = ["*.json"] }]
        };

        /// These are set later, this is just done so that there are no warnings...
        _options = JsonSerializerOptions.Default;
        _recents = ReadOnlyObservableCollection<RecentFile>.Empty;

        _editorCache = new SourceCache<FileEditor, Uri>(k => k.File.Path);
        _editorCache
            .Connect()
            .AutoRefresh()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _editors)
            .WhenPropertyChanged(p => p.Entries, false)
            .Subscribe(
                async r =>
                {
                    if (_options is null || _prefs is null || !_prefs.AutoSave)
                    {
                        r.Sender.HasUnsavedChanged = true;
                    }
                    else
                    {
                        await r.Sender.Save(options: _options);
                    }
                }
            );

        _recentCache = new SourceCache<RecentFile, Uri>(k => k.Path);
        _recentCache.AddOrUpdate(recents);
        _recentCache
            .Connect()
            .AutoRefresh()
            .ObserveOn(RxApp.MainThreadScheduler)
            .SortAndBind(out _recents, Comparer<RecentFile>.Create((c1, c2) => c2.LastOpenedAt.CompareTo(c1.LastOpenedAt)))
            .WhenPropertyChanged(p => p.LastOpenedAt, true)
            .Subscribe(
                async r =>
                {
                    Stream stream = await recentFile.OpenWriteAsync();
                    JsonSerializer.Serialize(stream, _recentCache.Items);
                    stream.Close();
                }
            );

        _prefs = prefs;
        _prefs
            .WhenAnyPropertyChanged()
            .Subscribe(
                async p =>
                {
                    if (p is null) return;
                    SetOptions(p);

                    Stream stream = await prefsFile.OpenWriteAsync();
                    JsonSerializer.Serialize(stream, p);
                    stream.Close();
                }
            );

        SetOptions(_prefs);
    }

    /// <summary>
    /// Opens a file using file dialog.
    /// </summary>
    public async Task OpenFileDialog(Window window)
    {
        var files = await window.StorageProvider.OpenFilePickerAsync(_fileOpenOptions);
        foreach (IStorageFile? file in files)
        {
            await OpenFile(window, file);
        }
    }

    /// <summary>
    /// Opens a recent file.
    /// </summary>
    public async Task OpenRecentFile(IReadOnlyList<object> o)
    {
        if (o.Count == 2 && o[0] is Window window && o[1] is RecentFile recent)
        {
            var file = await window.StorageProvider.OpenFileBookmarkAsync(recent.Bookmark);
            await OpenFile(window, file);
        }
    }

    /// <summary>
    /// Opens a dialog prompt if the editor that's being closed has changes to it.
    /// </summary>
    public void CloseTab(IReadOnlyList<object> o)
    {
        if (o.Count == 2 && o[0] is Window window && o[1] is FileEditor editor)
        {
            if (editor.HasUnsavedChanged)
            {
                var v = new ClosingPromptViewModel(this, editor);
                var closeDialog = new CloseDialog { DataContext = v };
                closeDialog.ShowDialog(window);
            }
            else
            {
                Close(editor);
            }
        }
    }

    /// <summary>
    /// Saves any changes made within an editor, and then closes it.
    /// </summary>
    public async void SaveAndClose(FileEditor editor)
    {
        await editor.Save(options: _options);
        Close(editor);
    }

    /// <summary>
    /// Closes an editor.
    /// </summary>
    public void Close(FileEditor editor)
    {
        _editorCache.Remove(editor);
        SelectedFile = null;
    }

    /// <summary>
    /// Saves any recorded changes within the currently selected file.
    /// </summary>
    public async Task SaveFile(Window window)
    {
        if (SelectedFile is null) return;
        await SelectedFile.Save(_options);

        ShowPopup(window, $"Saved {SelectedFile.CardCount} cards in {SelectedFile.File.Name}.");
    }

    /// <summary>
    /// Saves the currently selected file to a new destination.
    /// </summary>
    public async Task SaveFileAs(Window window)
    {
        if (SelectedFile is null) return;
        var file = await window.StorageProvider.SaveFilePickerAsync(_fileSaveOptions);
        await SelectedFile.Save(_options, file);

        if (file is not null)
        {
            await SaveRecent(file);
            ShowPopup(window, $"Saved {SelectedFile.CardCount} cards to {SelectedFile.File.Name}.");
        }
    }

    /// <summary>
    /// Opens the search dialog.
    /// </summary>
    public void OpenSearchDialog(Window window)
    {
        if (SelectedFile is null) return;
        var search = new FileEditorSearch { Editor = SelectedFile };
        var searchWindow = new SearchDialog { DataContext = search };
        searchWindow.ShowDialog(window);
    }

    /// <summary>
    /// Opens the filter dialog.
    /// </summary>
    public void OpenFilterDialog(Window window)
    {
        if (SelectedFile is null) return;
        var filterWindow = new FilterWindow { DataContext = SelectedFile.Filter };
        filterWindow.ShowDialog(window);
    }

    /// <summary>
    /// Opens the sorting dialog.
    /// </summary>
    public void OpenSortingDialog(Window window)
    {
        if (SelectedFile is null) return;
        var sortingWindow = new SortWindow { DataContext = SelectedFile.Sorting };
        sortingWindow.ShowDialog(window);
    }

    /// <summary>
    /// Options the application preferences options window.
    /// </summary>
    public void OpenPreferences(Window window)
    {
        var preferencesWindow = new PreferencesWindow { DataContext = _prefs };
        preferencesWindow.ShowDialog(window);
    }

    /// <summary>
    /// Opens a file.
    /// If possible, bookmarks it and saves it to recent history.
    /// </summary>
    private async Task<bool> OpenFile(Window window, IStorageFile? file)
    {
        if (file is null) return false;
        if (OpenedFiles.Any(p => p.File.Path == file.Path)) return false;

        StorageItemProperties props = await file.GetBasicPropertiesAsync();
        if (!props.Size.HasValue) return false;

        Stream fileStream = await file.OpenReadAsync();
        Dictionary<string, CardDescriptor>? dict = null;
        try
        {
            dict = JsonSerializer.Deserialize<Dictionary<string, CardDescriptor>>(fileStream, _options);
        }
        catch (Exception e)
        {
            ShowPopup(window, $"Could not parse {file.Name}. {e.Message}\n{e.StackTrace}");
        }

        if (dict is null)
        {
            fileStream.Close();
            return false;
        }

        FileEditor editor = new FileEditor(file, dict.Values);
        _editorCache.AddOrUpdate(editor);

        await SaveRecent(file);
        ShowPopup(window, $"Found {editor.CardCount} cards in {editor.File.Name}.");
        return true;
    }

    /// <summary>
    /// Saves a file to recent history.
    /// </summary>
    private async Task SaveRecent(IStorageFile file)
    {
        if (file.CanBookmark)
        {
            var bookmark = await file.SaveBookmarkAsync();
            if (bookmark is not null)
            {
                RecentFile recent = new RecentFile(bookmark, file.Name, file.Path, DateTime.Now);
                _recentCache.AddOrUpdate(recent);
            }
        }
    }

    /// <summary>
    /// Sets JSON options.
    /// Due to specific structure, JsonSerializerOptions cannot be a field directly in Preferences.
    /// </summary>
    private void SetOptions(Preferences prefs)
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = prefs.WriteIndented
        };

        _options.Converters.Add(new JsonStringEnumConverter());
    }

    /// <summary>
    /// Displays the popup at the top of the window.
    /// </summary>
    private static void ShowPopup(Window window, string message)
    {
        if (window.ContextFlyout is Flyout flyout)
        {
            flyout.Content = message;
            flyout.ShowAt(window);
        }
    }
}
