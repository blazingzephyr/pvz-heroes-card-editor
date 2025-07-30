
using System.Collections.Generic;

namespace HeroesCardEditor.ViewModels;

/// <summary>
/// A utility interface that provides an access point for buttons within tab items.
/// Created because with compiled bindings you can't access methods on DataContext without casting it.
/// That creates problems specifically with the ControlTemplate (See App.axaml Line 81).
/// </summary>
internal interface ITabContainer
{
    /// <summary>
    /// Closes a tab in a view model that contains tab elements.
    /// </summary>
    public void CloseTab(IReadOnlyList<object> entry);
}
