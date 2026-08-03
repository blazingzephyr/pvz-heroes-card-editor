
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using HeroesCardEditor.Models;
using PvZCards.Engine;
using PvZCards.Engine.Components;
using ReactiveUI;

namespace HeroesCardEditor.ViewModels;

/// <summary>
/// Represents an editable PvZ Heroes JSON file, serving as a collection of cards and few utility properties.
/// </summary>
internal partial class FileEditor : ObservableRecipient, ITabContainer
{
    /// <summary>
    /// The opened Avalonia file.
    /// Usually this remains the same throughout the entire time the editor tab is opened,
    /// but it can change if the file is saved to another location via Save As.
    /// </summary>
    [ObservableProperty]
    public partial IStorageFile File { get; private set; }

    /// <summary>
    /// DynamicData dynamic filter wrapper.
    /// </summary>
    [ObservableProperty]
    public partial FileEditorFilter Filter { get; private set; }

    /// <summary>
    /// DynamicData dynamic sorting wrapper. Records any changes on its own and sorts the entries accordingly.
    /// </summary>
    [ObservableProperty]
    public partial FileEditorSorting Sorting { get; private set; }

    /// <summary>
    /// The source of opened tabs on top of the editor.
    /// </summary>
    [ObservableProperty]
    public partial ObservableCollection<ObservableCardDescriptor> OpenedEntries { get; set; }

    /// <summary>
    /// The selected entry references (in the leftmost list).
    /// </summary>
    [ObservableProperty]
    public partial ObservableCardDescriptor? SelectedEntry { get; set; }

    /// <summary>
    /// The selected tab (in the opened entries tab strip).
    /// This was made so that the selection in the list and the tab always points to the same card.
    /// </summary>
    [ObservableProperty]
    public partial ObservableCardDescriptor? SelectedTab { get; set; }

    /// <summary>
    /// The selected tab (but in index form).
    /// This is used only once in the entire application.
    /// When a tab is deleted the index automatically points to the tab that preceded it. (See line 192)
    /// </summary>
    [ObservableProperty]
    public partial int SelectedIndex { get; set; }

    /// <summary>
    /// Whether this file has any unsaved changes.
    /// This does NOT take in the account that changes may revert other changes.
    /// </summary>
    [ObservableProperty]
    public partial bool HasUnsavedChanged { get; set; }

    /// <summary>
    /// All cards in this file.
    /// </summary>
    public ReadOnlyObservableCollection<ObservableCardDescriptor> Entries => _entries;
    
    /// <summary>
    /// Used only when a file is being opened or saved, so that we don't expose cache.
    /// </summary>
    public int CardCount => _cache.Count;

    private readonly SourceCache<CardDescriptor, uint> _cache;
    private readonly ReadOnlyObservableCollection<ObservableCardDescriptor> _entries;
    private readonly JsonSerializerOptions _options;
    
    public FileEditor(IStorageFile file, IEnumerable<CardDescriptor> descriptors, JsonSerializerOptions options)
    {
        File = file;
        Filter = new FileEditorFilter();
        Sorting = new FileEditorSorting();
        OpenedEntries = [];

        var observable = descriptors.ToObservable();
        var filter = CreateObservable<FileEditorFilter, Func<ObservableCardDescriptor, bool>>(Filter, i => i!.BuildFilter);
        var comparer = CreateObservable(Sorting, i => i!.BuildComparer());

        _options = options;
        _cache = new SourceCache<CardDescriptor, uint>(p => p.Components.OfType<Card>().First().Value);
        _cache.PopulateFrom(observable);
        _cache
            .Connect()
            .Transform(c => new ObservableCardDescriptor(c))
            .AutoRefresh()
            .Filter(filter)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SortAndBind(out _entries, comparer)
            .DisposeMany()
            .WhenAnyPropertyChanged(
                nameof(ObservableCardDescriptor.Note),
                nameof(ObservableCardDescriptor.Guid),
                nameof(ObservableCardDescriptor.PrefabName),
                nameof(ObservableCardDescriptor.Color),
                nameof(ObservableCardDescriptor.Rarity),
                nameof(ObservableCardDescriptor.Set),
                nameof(ObservableCardDescriptor.SetAndRarityKey),
                nameof(ObservableCardDescriptor.CraftingBuy),
                nameof(ObservableCardDescriptor.CraftingSell),
                nameof(ObservableCardDescriptor.MaxHealth),
                nameof(ObservableCardDescriptor.CurrentDamage),
                nameof(ObservableCardDescriptor.Attack),
                nameof(ObservableCardDescriptor.SunCost),
                nameof(ObservableCardDescriptor.Type),
                nameof(ObservableCardDescriptor.Faction),
                nameof(ObservableCardDescriptor.IgnoreDeckLimit),
                nameof(ObservableCardDescriptor.Usable),
                nameof(ObservableCardDescriptor.IsPower),
                nameof(ObservableCardDescriptor.IsPrimaryPower),
                nameof(ObservableCardDescriptor.IsAquatic),
                nameof(ObservableCardDescriptor.IsTeamup),
                nameof(ObservableCardDescriptor.CreateInFront),
                nameof(ObservableCardDescriptor.Strikethrough),
                nameof(ObservableCardDescriptor.Truestrike),
                nameof(ObservableCardDescriptor.Armor),
                nameof(ObservableCardDescriptor.Frenzy),
                nameof(ObservableCardDescriptor.Deadly),
                nameof(ObservableCardDescriptor.Unhealable),
                nameof(ObservableCardDescriptor.MultiplyDamage),
                nameof(ObservableCardDescriptor.Untrickable),
                nameof(ObservableCardDescriptor.AttackOverride),
                nameof(ObservableCardDescriptor.Surprise),
                nameof(ObservableCardDescriptor.PlaysFaceDown),
                nameof(ObservableCardDescriptor.Multishot),
                nameof(ObservableCardDescriptor.AttacksInAllLanes),
                nameof(ObservableCardDescriptor.AttacksOnlyInAdjacentLanes),
                nameof(ObservableCardDescriptor.SplashDamage),
                nameof(ObservableCardDescriptor.Tags),
                nameof(ObservableCardDescriptor.GrantedAbilities),
                nameof(ObservableCardDescriptor.SpecialAbilities),
                nameof(ObservableCardDescriptor.Subtypes),
                nameof(ObservableCardDescriptor.EED),
                nameof(ObservableCardDescriptor.EED.Entities))            
            .Subscribe(p =>
            {
                p?.HasUnsavedChanged = true;
                OnPropertyChanged(nameof(Entries));
            });
    }

    /// <summary>
    /// Saves cards to the file this editor points to (or a new location, if provided).
    /// Also takes options (I changed it from optional to required because otherwise it fails to write the file correctly).
    /// </summary>
    public async Task Save(JsonSerializerOptions options, IStorageFile? destination = default)
    {
        if (destination is object)
        {
            File = destination;
        }

        var dict = _cache.Items.ToDictionary(k => k.Components.OfType<Card>().First().Value.ToString());
        Stream writeStream = await File.OpenWriteAsync();
        JsonSerializer.Serialize(writeStream, dict, options);

        writeStream.Close();
        HasUnsavedChanged = false;
    }

    /// <summary>
    /// Opens a new editor tab and automatically selects.
    /// </summary>
    partial void OnSelectedEntryChanged(ObservableCardDescriptor? oldValue, ObservableCardDescriptor? newValue)
    {
        if (newValue is not null)
        {
            if (!OpenedEntries.Contains(newValue))
            {
                OpenedEntries.Add(newValue);
            }

            if (SelectedTab is null || SelectedEntry is null || SelectedTab != SelectedEntry)
            {
                SelectedTab = SelectedEntry;
            }
        }
    }

    /// <summary>
    /// When (presumably) scrolling, selects the card that is being edited in the list.
    /// </summary>
    partial void OnSelectedTabChanged(ObservableCardDescriptor? oldValue, ObservableCardDescriptor? newValue)
    {
        if (SelectedTab is null || SelectedEntry is null || SelectedTab != SelectedEntry)
        {
            SelectedEntry = SelectedTab;
        }
    }

    /// <summary>
    /// Closes an editor tab.
    /// This does NOT delete the card itself!
    /// </summary>
    public void CloseTab(IReadOnlyList<object> o)
    {
        if (o.Count == 2 && o[1] is ObservableCardDescriptor descriptor)
        {
            OpenedEntries.Remove(descriptor);
        }
    }

    /// <summary>
    /// Creates a new card and fills it with default values.
    /// This also adds all essential components to the card.
    /// </summary>
    public void CreateEmptyCard()
    {
        CardDescriptor card = new CardDescriptor
        {
            Components = [
                new Unusable { },
                new Card { Value = Enumerable.Max(_cache.Keys) + 1 },
                new SunCost { Value = 0 },
                new Rarity { Value = RarityType.Common }
            ],

            Usable = false
        };

        _cache.AddOrUpdate(card);
        OnPropertyChanged(nameof(Entries));

        uint guid = card.Components.OfType<Card>().First().Value;
        SelectedTab = _entries.FirstOrDefault(o => o.Guid == guid);
    }

    /// <summary>
    /// Self-explainatory.
    /// Some indexing logic might be broken here, feel free to fix it if you know how to!
    /// </summary>
    public void DeleteSelectedCard()
    {
        if (SelectedTab is null) return;

        var index = SelectedIndex;
        _cache.RemoveKey(SelectedTab.Guid);
        OpenedEntries.Remove(SelectedTab);
        OnPropertyChanged(nameof(Entries));

        index--;
        if (OpenedEntries.Count > 0)
        {
            SelectedIndex = index < 0 ? 0 : index;
        }
    }

    /// <summary>
    /// Creates a WhenAnyPropertyChanged observable with an initial setup notification.
    /// Used for the dynamic filter and sorting.
    /// </summary>
    private static IObservable<TFunc> CreateObservable<TSource, TFunc>(TSource observable, Func<TSource?, TFunc> func)
        where TSource : ObservableRecipient
    {
        var initialSetup = Observable.Create<TSource?>(
            o =>
            {
                o.OnNext(observable);
                return Task.CompletedTask;
            }
        );

        return observable
            .WhenAnyPropertyChanged()
            .Merge(initialSetup)
            .Throttle(TimeSpan.FromMilliseconds(250))
            .Select(func);
    }

    public async void EditCode(Window window)
    {
        var rawEditorMv = new RawEditorViewModel(File);
        var rawEditor = new RawEditorView() { DataContext = rawEditorMv };
        await rawEditor.ShowDialog(window);

        if (!rawEditorMv.Edited) return;
        
        var cards = await EditorViewModel.DeserializeFile(window, File, _options);
        if (cards is null)
        {
            _cache.Clear();
            return;
        }

        var observable = cards.ToObservable();
        var filter = CreateObservable<FileEditorFilter, Func<ObservableCardDescriptor, bool>>(Filter, i => i!.BuildFilter);
        var comparer = CreateObservable(Sorting, i => i!.BuildComparer());

        _cache.PopulateFrom(observable);
    }
}
