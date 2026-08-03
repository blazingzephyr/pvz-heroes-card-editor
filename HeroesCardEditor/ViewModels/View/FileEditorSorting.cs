
using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using HeroesCardEditor.Models;
using PvZCards.Engine;

namespace HeroesCardEditor.ViewModels;

/// <summary>
/// Dynamic sorting for ObservableCache (See FileEditor.cs Line 61).
/// </summary>
internal partial class FileEditorSorting : ObservableRecipient
{
    /// <summary>
    /// The priority card's guid takes in sort order.
    /// </summary>
    [ObservableProperty]
    public partial int GuidPriority { get; set; } = 1;

    /// <summary>
    /// Whether to sort by guid descending.
    /// </summary>
    [ObservableProperty]
    public partial bool GuidDescending { get; set; }

    [ObservableProperty]
    public partial int CardTypePriority { get; set; }

    [ObservableProperty]
    public partial bool CardTypeDescending { get; set; }

    [ObservableProperty]
    public partial int FactionPriority { get; set; }

    [ObservableProperty]
    public partial bool FactionDescending { get; set; }

    [ObservableProperty]
    public partial int ColorPriority { get; set; }

    [ObservableProperty]
    public partial bool ColorDescending { get; set; }

    [ObservableProperty]
    public partial int PrefabNamePriority { get; set; }

    [ObservableProperty]
    public partial bool PrefabNameDescending { get; set; }

    [ObservableProperty]
    public partial int RarityPriority { get; set; }

    [ObservableProperty]
    public partial bool RarityDescending { get; set; }

    [ObservableProperty]
    public partial int SunCostPriority { get; set; }

    [ObservableProperty]
    public partial bool SunCostDescending { get; set; }

    [ObservableProperty]
    public partial int AttackPriority { get; set; }

    [ObservableProperty]
    public partial bool AttackDescending { get; set; }

    [ObservableProperty]
    public partial int HealthPriority { get; set; }

    [ObservableProperty]
    public partial bool HealthDescending { get; set; }

    /// <summary>
    /// Builds the comparer.
    /// </summary>
    public Comparer<ObservableCardDescriptor> BuildComparer()
    {
        int DefaultComparer(IComparable? a, IComparable? b)
        {
            if (a == b) return 0;
            else if (a is null) return -1;
            else if (b is null) return 1;
            return a.CompareTo(b);
        }

        (int Priority,
         bool Descending,
         Func<ObservableCardDescriptor?, IComparable?> Property,
         Func<IComparable?, IComparable?, int>)[] props =
        [
            (GuidPriority, GuidDescending, p => p?.Guid, DefaultComparer),
            (CardTypePriority, CardTypeDescending, p => p?.Type, DefaultComparer),
            (FactionPriority, FactionDescending, p => p?.Faction, DefaultComparer),
            (ColorPriority, ColorDescending, p => p?.Color, DefaultComparer),
            (PrefabNamePriority, PrefabNameDescending, p => $"{p?.Name} {p?.Note} {p?.PrefabName}", StringComparer.CurrentCultureIgnoreCase.Compare),
            (RarityPriority, RarityDescending, p => p?.Rarity, DefaultComparer),
            (SunCostPriority, SunCostDescending, p => p?.SunCost, DefaultComparer),
            (AttackPriority, AttackDescending, p => p?.Attack, DefaultComparer),
            (HealthPriority, HealthDescending, p => p?.MaxHealth, DefaultComparer)
        ];

        var properties = from prop in props
                       orderby prop.Priority descending
                       where prop.Priority > 0
                       select prop;

        int Comparison(ObservableCardDescriptor a, ObservableCardDescriptor b)
        {
            foreach ((var priority, var desc, var property, var comparer) in properties)
            {
                if (priority == 0) continue;

                var x = property(a);
                var y = property(b);
                var res = desc ? comparer(y, x) : comparer(x, y);
                if (res != 0) return res;
            }

            return 0;
        }

        return Comparer<ObservableCardDescriptor>.Create(Comparison);
    }
}
