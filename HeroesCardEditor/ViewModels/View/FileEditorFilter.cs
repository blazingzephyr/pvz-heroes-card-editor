
using PvZCards.Engine;
using HeroesCardEditor.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;

namespace HeroesCardEditor.ViewModels;

/// <summary>
/// Dynamic filter for ObservableCache (See FileEditor.cs Line 60).
/// </summary>
internal partial class FileEditorFilter : ObservableRecipient
{
    /// <summary>
    /// Card's type the filter or search should use.
    /// </summary>
    [ObservableProperty]
    public partial CardType? ByCardType { get; set; } = null;

    /// <summary>
    /// Card's faction.
    /// </summary>
    [ObservableProperty]
    public partial Faction? ByFaction { get; set; } = null;

    /// <summary>
    /// Card's class value the filter or search should use.
    /// </summary>
    [ObservableProperty]
    public partial Color ByColor { get; set; } = Color.None;

    /// <summary>
    /// Card's prefab name the filter or search should use.
    /// </summary>
    [ObservableProperty]
    public partial string ByPrefabName { get; set; } = string.Empty;

    /// <summary>
    /// Card's rarity the filter or search should use.
    /// </summary>
    [ObservableProperty]
    public partial RarityType? ByRarity { get; set; } = null;

    /// <summary>
    /// Card's sun cost the filter or search should use.
    /// </summary>
    [ObservableProperty]
    public partial int BySunCost { get; set; } = -1;

    /// <summary>
    /// Card's attack value (if any) the filter or search should use.
    /// </summary>
    [ObservableProperty]
    public partial int ByAttack { get; set; } = -1;

    /// <summary>
    /// Card's health (if any) the filter or search should use.
    /// </summary>
    [ObservableProperty]
    public partial int ByHealth { get; set; } = -1;

    /// <summary>
    /// Card's tags that will be analyzed.
    /// </summary>
    [ObservableProperty]
    public partial string ByTags { get; set; } = string.Empty;

    /// <summary>
    /// Card predicate.
    /// Originally, I had a base class of FileEditorView for FileEditorFilter and FileEditorSearch.
    /// Then I realized that FileEditorSearch basically derives from FileEditorFilter, so I removed the base class.
    /// 
    /// Because of that limitation, I had to rewrite
    /// Filter from Func<ObservableCardDescriptor, bool>() to bool(ObservableCardDescriptor)
    /// (See FileEditor.cs Line 60 and FileEditorSearch Line 26).
    /// </summary>
    public bool BuildFilter(ObservableCardDescriptor card)
    {
        if (ByCardType != null && card.Type != ByCardType) return false;
        if (ByFaction != null && card.Faction != ByFaction) return false;
        if (ByColor != Color.None && card.Color != ByColor) return false;
        if (!string.IsNullOrEmpty(ByPrefabName))
        {
            if (!$"{card.Descriptor.Comment} {card.Name} {card.PrefabName}".Contains(ByPrefabName, StringComparison.CurrentCultureIgnoreCase))
                return false;
        }
        if (ByRarity != null && card.Rarity != ByRarity) return false;
        if (BySunCost != -1 && card.SunCost != BySunCost) return false;
        if (ByAttack != -1 && card.Attack != ByAttack) return false;
        if (ByHealth != -1 && card.MaxHealth != ByHealth) return false;

        var tags = card.Tags.Split(';');
        if (!String.IsNullOrEmpty(ByTags) && !ByTags.Split(';').All(c => tags.Contains(c))) return false;

        return true;
    }
}
