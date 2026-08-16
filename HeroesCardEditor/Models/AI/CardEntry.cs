
namespace HeroesCardEditor.Models.AI;

class CardEntry
{
    public int Faction { get; set; } = 2;
    public uint CardGuid { get; set; }
    public string? Guid { get; set; } = null;
    public int NumCopies { get; set; }
    public string? Filter { get; set; } = string.Empty;
}
