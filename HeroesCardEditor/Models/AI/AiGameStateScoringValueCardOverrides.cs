
namespace HeroesCardEditor.Models.AI;

class AiGameStateScoringValueCardOverrides
{
    public CardEntry CardEntry { get; set; } = new CardEntry();
    public byte UseFighterOnBoardBaseValueOverride { get; set; } = 0;
    public float FighterOnBoardBaseValue { get; set; } = 0.0f;
    public byte UseFighterOnBoardMultiplierOverride { get; set; } = 0;
    public float FighterOnBoardMultiplier { get; set; } = 0.0f;
    public byte UseCardInHandBaseValueOverride { get; set; } = 0;
    public float CardInHandBaseValue { get; set; } = 0.0f;
    public byte UseCardInHandMultiplierOverride { get; set; } = 0;
    public float CardInHandMultiplier { get; set; } = 0.0f;
    public byte UseEnvironmentOnBoardValueOverride { get; set; } = 0;
    public float EnvironmentOnBoardValue { get; set; } = 0.0f;
    public byte UseEnvironmentOnBoardMultiplierOverride { get; set; } = 0;
    public float EnvironmentOnBoardMultiplier { get; set; } = 0.0f;
}
