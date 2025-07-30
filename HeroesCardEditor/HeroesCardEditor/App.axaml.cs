
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using HeroesCardEditor.Models;
using HeroesCardEditor.ViewModels;

namespace HeroesCardEditor;

public partial class App : Application
{
    public Func<IStorageProvider, string, Type, Task<(IStorageFile?, object?)>>? OpenOrCreate { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        IStorageProvider storageProvider;
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                EditorWindow window = new EditorWindow();
                desktop.MainWindow = window;
                storageProvider = window.StorageProvider;
                break;

            case ISingleViewApplicationLifetime view:
                EditorView editor = new EditorView();
                view.MainView = editor;
                storageProvider = TopLevel.GetTopLevel(editor)!.StorageProvider;
                break;

            default:
                throw new NotImplementedException();
        }

        (var prefsFile, var prefsObj) = await OpenOrCreate!(storageProvider, "preferences.json", typeof(Preferences));
        if (prefsObj is not Preferences prefs) return;
        Console.WriteLine($"{prefs.WriteIndented} {prefs.WriteIndented} {prefs.AutoSave}");
        // if (prefsFile == null) return;

        (var recentFile, var recentObj) = await OpenOrCreate!(storageProvider, "recent.json", typeof(Collection<RecentFile>));
        if (recentObj is not Collection<RecentFile> recent) return;
        // if (recentFile == null) return;

        EditorViewModel editorContext = new EditorViewModel(prefsFile, recentFile, prefs, recent, ApplicationLifetime, storageProvider);
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow!.DataContext = editorContext;
                break;

            case ISingleViewApplicationLifetime view:
                view.MainView!.DataContext = editorContext;
                break;

            default:
                throw new NotImplementedException();
        }
    }
}
