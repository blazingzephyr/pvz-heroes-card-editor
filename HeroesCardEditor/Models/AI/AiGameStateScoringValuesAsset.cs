
namespace HeroesCardEditor.Models.AI;

#pragma warning disable IDE1006
class AiGameStateScoringValuesAsset
{
    public MonoBehaviorFileRef m_GameObject { get; set; } = new MonoBehaviorFileRef();
    public byte m_Enabled { get; set; }
    public MonoBehaviorFileRef m_Script { get; set; } = new MonoBehaviorFileRef();
    public string m_Name { get; set; } = string.Empty;
    public float FighterOnBoardBaseValue { get; set; }
    public float FighterOnBoardMultiplier { get; set; }
    public float CardInHandBaseValue { get; set; }
    public float CardInHandMultiplier { get; set; }
    public float EnvironmentOnBoardValue { get; set; }
    public float EnvironmentOnBoardMultiplier { get; set; }
    public float OpponentCardInHandBaseValue { get; set; }
    public ArrayContainer<float> FighterHealthValues { get; set; } = new ArrayContainer<float>();
    public ArrayContainer<float> FighterAttackValues { get; set; } = new ArrayContainer<float>();
    public ArrayContainer<float> CardSunValues { get; set; } = new ArrayContainer<float>();
    public ArrayContainer<float> HeroLifeValues { get; set; } = new ArrayContainer<float>();
    public ArrayContainer<AiGameStateScoringValueCardOverrides> CardValueOverrides { get; set; } = new ArrayContainer<AiGameStateScoringValueCardOverrides>();
    public float HighgroundLaneMultiplier { get; set; }
    public float HighgroundLaneIndexPenalty { get; set; }
    public float GrassLaneMultiplier { get; set; }
    public float GrassLaneIndexPenalty { get; set; }
    public float WaterLaneMultiplier { get; set; }
    public float WaterLaneIndexPenalty { get; set; }
    public float TeamupAttackBonusForBeingBehind { get; set; }
    public float TeamupHealthBonusForBeingInFront { get; set; }
    public float DummyGravestoneAttackHealthStat { get; set; }
    public float MulliganThresholdForKeepingHand { get; set; }
    public ArrayContainer<AiMulliganScoringTagValueOverrides> MulliganTagValueOverrides { get; set; } = new ArrayContainer<AiMulliganScoringTagValueOverrides>();
}
#pragma warning restore IDE1006
