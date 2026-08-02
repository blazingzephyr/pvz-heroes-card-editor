
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using HeroesCardEditor.Models;
using HeroesCardEditor.ViewModels;

namespace HeroesCardEditor;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = new EditorView();
            desktop.MainWindow = window;
            _ = InitializeAsync(window);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async Task InitializeAsync(EditorView window)
    {
        var (prefsFile, prefs) = await OpenOrCreate<Preferences>(window.StorageProvider, "preferences.json");
        if (prefsFile == null || prefs == null) return;

        var (recentFile, recent) = await OpenOrCreate<Collection<RecentFile>>(window.StorageProvider, "recent.json");
        if (recentFile == null || recent == null) return;

        window.DataContext = new EditorViewModel(prefsFile, recentFile, prefs, recent);
    }

    /// <summary>
    /// Opens (or creates) a file in the executable's directory, then reads it and returns the contents.
    /// </summary>
    private static async Task<(IStorageFile?, T?)> OpenOrCreate<T>(IStorageProvider provider, string fileName)
    {
        string folderPath = Environment.CurrentDirectory;
        IStorageFile? file = await provider.TryGetFileFromPathAsync($"{folderPath}/{fileName}");
        T? result;

        if (file == null)
        {
            IStorageFolder? folder = await provider.TryGetFolderFromPathAsync(folderPath);
            if (folder == null) return default;

            file = await folder.CreateFileAsync(fileName);
            if (file == null) return default;

            Stream writeStream = await file.OpenWriteAsync();
            result = Activator.CreateInstance<T>();

            JsonSerializer.Serialize(writeStream, result);
            writeStream.Close();
        }
        else
        {
            Stream readStream = await file.OpenReadAsync();
            result = JsonSerializer.Deserialize<T>(readStream);
            readStream.Close();
        }

        return (file, result);
    }
}
