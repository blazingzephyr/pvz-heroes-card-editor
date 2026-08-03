
using System;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using PvZCards.Engine;
using PvZCards.Engine.Components;
using PvZCards.Engine.Effects;
using PvZCards.Engine.Queries;

namespace HeroesCardEditor.Models;

/// <summary>
/// Provides a set of static PvZ Heroes card properties.
/// </summary>
internal static class CardProperties
{
    /// <summary>
    /// Rarity options of a card.
    /// </summary>
    public static readonly RarityType?[] RaritiesWithNone =
    [
        null,
        RarityType.Common,
        RarityType.Uncommon,
        RarityType.Rare,
        RarityType.SuperRare,
        RarityType.Event,
        RarityType.Legendary
    ];

    /// <summary>
    /// Rarity options of a card.
    /// </summary>
    public static readonly RarityType[] Rarities =
    [
        RarityType.Common,
        RarityType.Uncommon,
        RarityType.Rare,
        RarityType.SuperRare,
        RarityType.Event,
        RarityType.Legendary
    ];

    /// <summary>
    /// Color options of a card.
    /// </summary>
    public static readonly PvZCards.Engine.Color[] Colors =
    [
        PvZCards.Engine.Color.None,
        PvZCards.Engine.Color.Kabloom,
        PvZCards.Engine.Color.MegaGro,
        PvZCards.Engine.Color.Guardian,
        PvZCards.Engine.Color.Smarty,
        PvZCards.Engine.Color.Solar,
        PvZCards.Engine.Color.Brainy,
        PvZCards.Engine.Color.Hearty,
        PvZCards.Engine.Color.Sneaky,
        PvZCards.Engine.Color.Hungry,
        PvZCards.Engine.Color.Madcap,

        PvZCards.Engine.Color.MegaGro | PvZCards.Engine.Color.Smarty,
        PvZCards.Engine.Color.Kabloom | PvZCards.Engine.Color.Solar,
        PvZCards.Engine.Color.Guardian | PvZCards.Engine.Color.Solar,
        PvZCards.Engine.Color.MegaGro | PvZCards.Engine.Color.Solar,
        PvZCards.Engine.Color.Kabloom | PvZCards.Engine.Color.Guardian,
        PvZCards.Engine.Color.Guardian | PvZCards.Engine.Color.Smarty,
        PvZCards.Engine.Color.MegaGro | PvZCards.Engine.Color.Guardian,
        PvZCards.Engine.Color.Kabloom | PvZCards.Engine.Color.Smarty,
        PvZCards.Engine.Color.Smarty | PvZCards.Engine.Color.Solar,
        PvZCards.Engine.Color.Kabloom | PvZCards.Engine.Color.MegaGro,
        
        PvZCards.Engine.Color.Brainy | PvZCards.Engine.Color.Sneaky,
        PvZCards.Engine.Color.Hearty | PvZCards.Engine.Color.Hungry,
        PvZCards.Engine.Color.Sneaky | PvZCards.Engine.Color.Madcap,
        PvZCards.Engine.Color.Brainy | PvZCards.Engine.Color.Hearty,
        PvZCards.Engine.Color.Hearty | PvZCards.Engine.Color.Madcap,
        PvZCards.Engine.Color.Sneaky | PvZCards.Engine.Color.Hungry,
        PvZCards.Engine.Color.Brainy | PvZCards.Engine.Color.Madcap,
        PvZCards.Engine.Color.Brainy | PvZCards.Engine.Color.Hungry,
        PvZCards.Engine.Color.Hearty | PvZCards.Engine.Color.Madcap,
        PvZCards.Engine.Color.Hearty | PvZCards.Engine.Color.Sneaky
    ];

    public static readonly string?[] Sets =
    [
        null,
        "",
        "Blank",
        "Board",
        "cheats",
        "Cheats",
        "Event",
        "Gold",
        "Hero",
        "Set2",
        "Set3",
        "Set4",
        "Silver",
        "Superpower",
        "Token"
    ];

    public static readonly string?[] SetAndRarityKeys =
    [
        null,
        "",
        "Bloom_Common",
        "Bloom_Legendary",
        "Bloom_Rare",
        "Bloom_SuperRare",
        "Colossal_Legendary",
        "Colossal_Rare",
        "Colossal_SuperRare",
        "Colossal_Uncommon",
        "Dawn_Common",
        "Galaxy_Common",
        "Galaxy_Legendary",
        "Galaxy_Rare",
        "Galaxy_SuperRare",
        "Premium_Event",
        "Superpower_SuperRare",
        "Token",
        "Triassic_Legendary",
        "Triassic_Rare",
        "Triassic_SuperRare",
        "Triassic_Uncommon"
    ];

    public static readonly CardType?[] CardTypesWithNone =
    [
        null,
        CardType.Fighter,
        CardType.Trick,
        CardType.Environment,
        CardType.BoardAbility
    ];

    public static readonly CardType[] CardTypes =
    [
        CardType.Fighter,
        CardType.Trick,
        CardType.Environment,
        CardType.BoardAbility
    ];

    public static readonly Faction?[] FactionsWithNone =
    [
        null,
        Faction.All,
        Faction.Plants,
        Faction.Zombies
    ];

    public static readonly Faction[] Factions =
    [
        Faction.All,
        Faction.Plants,
        Faction.Zombies
    ];

    public static readonly SpecialAbility[] SpecialAbilities =
    [
        SpecialAbility.Ambush,
        SpecialAbility.Repeater,
        SpecialAbility.Overshoot,
        SpecialAbility.Unique,
        ////////////
        SpecialAbility.Armor,
        SpecialAbility.AttackOverride,
        SpecialAbility.Deadly,
        SpecialAbility.Frenzy,
        SpecialAbility.Strikethrough,
        SpecialAbility.Truestrike,
        SpecialAbility.Untrickable
    ];

    public enum Tribe
    {
        Peashooter = 0,
        Berry = 1,
        Bean = 2,
        Flower = 3,
        Mushroom = 4,
        Nut = 5,
        Sports = 6,
        Science = 7,
        Dancing = 8,
        Imp = 9,
        Pet = 10,
        Gargantuar = 11,
        Pirate = 12,
        Pinecone = 13,
        Mustache = 15,
        Party = 16,
        Gourmet = 18,
        History = 19,
        Barrel = 20,
        Seed = 21,
        Animal = 22,
        Cactus = 23,
        Corn = 24,
        Dragon = 25,
        Flytrap = 26,
        Fruit = 27,
        Leafy = 28,
        Moss = 29,
        Squash = 32,
        Root = 31,
        Tree = 33,
        Clock = 35,
        Professional = 37,
        Monster = 39,
        Mime = 41,
        ////
        Custom1 = 14,
        Custom2 = 17,
        Custom3 = 30
    }

    public static readonly Tribe[] Subtypes =
    [
        Tribe.Animal,
        Tribe.Barrel,
        Tribe.Bean,
        Tribe.Berry,
        Tribe.Cactus,
        Tribe.Clock,
        Tribe.Corn,
        Tribe.Custom1,
        Tribe.Custom2,
        Tribe.Custom3,
        Tribe.Dancing,
        Tribe.Dragon,
        Tribe.Flower,
        Tribe.Flytrap,
        Tribe.Fruit,
        Tribe.Gargantuar,
        Tribe.Gourmet,
        Tribe.History,
        Tribe.Imp,
        Tribe.Leafy,
        Tribe.Mime,
        Tribe.Monster,
        Tribe.Moss,
        Tribe.Mushroom,
        Tribe.Mustache,
        Tribe.Nut,
        Tribe.Party,
        Tribe.Peashooter,
        Tribe.Pet,
        Tribe.Pinecone,
        Tribe.Pirate,
        Tribe.Professional,
        Tribe.Root,
        Tribe.Science,
        Tribe.Seed,
        Tribe.Sports,
        Tribe.Squash,
        Tribe.Tree
    ];

    public static readonly Type[] Triggers =
    [
        typeof(BuffTrigger),
        typeof(CombatEndTrigger),
        typeof(CombatStartTrigger),
        typeof(Continuous),
        typeof(DamageTrigger),
        typeof(DestroyCardTrigger),
        typeof(DiscardFromPlayTrigger),
        typeof(DrawCardFromSubsetTrigger),
        typeof(DrawCardTrigger),
        typeof(EnterBoardTrigger),
        typeof(ExtraAttackTrigger),
        typeof(HealTrigger),
        typeof(LaneCombatEndTrigger),
        typeof(LaneCombatStartTrigger),
        typeof(MoveTrigger),
        typeof(PlayTrigger),
        typeof(ReturnToHandTrigger),
        typeof(RevealPhaseEndTrigger),
        typeof(RevealTrigger),
        typeof(SlowedTrigger),
        typeof(SurprisePhaseStartTrigger),
        typeof(TurnStartTrigger)
    ];

    public static readonly Type[] Effects =
    [
        typeof(AttackInLaneEffectDescriptor),
        typeof(BuffEffectDescriptor),
        typeof(ChargeBlockMeterEffectDescriptor),
        typeof(CopyCardEffectDescriptor),
        typeof(CopyStatsEffectDescriptor),
        typeof(CreateCardEffectDescriptor),
        typeof(CreateCardFromSubsetEffectDescriptor),
        typeof(CreateCardInDeckEffectDescriptor),
        typeof(DamageEffectDescriptor),
        typeof(DestroyCardEffectDescriptor),
        typeof(DrawCardEffectDescriptor),
        typeof(DrawCardFromSubsetEffectDescriptor),
        typeof(EffectDescriptor),
        typeof(ExtraAttackEffectDescriptor),
        typeof(GainSunEffectDescriptor),
        typeof(GrantAbilityEffectDescriptor),
        typeof(GrantTriggeredAbilityEffectDescriptor),
        typeof(HealEffectDescriptor),
        typeof(MixedUpGravediggerEffectDescriptor),
        typeof(ModifySunCostEffectDescriptor),
        typeof(MoveCardToLanesEffectDescriptor),
        typeof(ReturnToHandFromPlayEffectDescriptor),
        typeof(SetStatEffectDescriptor),
        typeof(SlowEffectDescriptor),
        typeof(TransformIntoCardFromSubsetEffectDescriptor),
        typeof(TransformWithCreationSource),
        typeof(TurnIntoGravestoneEffectDescriptor)
    ];

    public static readonly Type[] General =
    [
        typeof(ActiveTargets),
        typeof(DamageEffectRedirector),
        typeof(DamageEffectRedirectorDescriptor),
        typeof(EffectEntityGrouping),
        typeof(EffectValueDescriptor),
        typeof(HeraldEntities),
        typeof(PersistsAfterTransform)
    ];

    public static readonly Type[] Filters =
    [
        typeof(SelfEntityFilter),
        typeof(SelfLaneEntityFilter),
        typeof(TriggerSourceFilter),
        typeof(TriggerTargetFilter),
        typeof(PrimaryTargetFilter),
        typeof(SecondaryTargetFilter),
    ];

    public static Type[] Conditions =
    [
        typeof(EffectValueCondition),
        typeof(OncePerGameCondition),
        typeof(OncePerTurnCondition),
        typeof(PlayerInfoCondition),
        typeof(QueryEntityCondition)
    ];

    public static readonly Type[] Multipliers =
    [
        typeof(DrawnCardCostMultiplier),
        typeof(HeroHealthMultiplier),
        typeof(QueryMultiplier),
        typeof(SunGainedMultiplier),
        typeof(TargetAttackMultiplier),
        typeof(TargetAttackOrHealthMultiplier),
        typeof(TargetHealthMultiplier)
    ];

    public static readonly Query[] Queries =
    [
        new AdjacentLaneQuery(),
        new AlwaysMatchesQuery(),
        new AttackComparisonQuery(),
        new BehindSameLaneQuery(),
        new BlockMeterValueQuery(),
        new CardGuidQuery(),
        new CompositeAllQuery(),
        new CompositeAnyQuery(),
        new DamageTakenComparisonQuery(),
        new DrawnCardQuery(),
        new FighterQuery(),
        new HasComponentQuery(),
        new HealthComparisonQuery(),
        new InAdjacentLaneQuery(),
        new InEnvironmentQuery(),
        new InHandQuery(),
        new InLaneAdjacentToLaneQuery(),
        new InLaneQuery(),
        new InLaneSameAsLaneQuery(),
        new InOneTimeEffectZoneQuery(),
        new InSameLaneQuery(),
        new InUnopposedLaneQuery(),
        new IsActiveQuery(),
        new IsAliveQuery(),
        new KilledByQuery(),
        new LacksComponentQuery(),
        new LaneOfIndexQuery(),
        new LaneWithMatchingEnvironmentQuery(),
        new LaneWithMatchingFighterQuery(),
        new LastLaneOfSelfQuery(),
        new NotQuery(),
        new OnTerrainQuery(),
        new OpenLaneQuery(),
        new OriginalTargetCardGuidQuery(),
        new SameFactionQuery(),
        new SameLaneAsTargetQuery(),
        new SameLaneQuery(),
        new SelfQuery(),
        new SourceQuery(),
        new SpringboardedOnSelfQuery(),
        new SubsetQuery(),
        new SubtypeQuery(),
        new SunCostComparisonQuery(),
        new SunCostPlusNComparisonQuery(),
        new SunCounterComparisonQuery(),
        new TargetableInPlayFighterQuery(),
        new TargetCardGuidQuery(),
        new TargetQuery(),
        new TrickQuery(),
        new TurnCountQuery(),
        new WasInSameLaneAsSelfQuery(),
        new WillTriggerEffectsQuery(),
        new WillTriggerOnDeathEffectsQuery()
    ];

    public static FuncValueConverter<RarityType, IBrush> Foreground = new FuncValueConverter<RarityType, IBrush>(
        r =>
        {
            return new SolidColorBrush
            {
                Color = r switch
                {
                    RarityType.Uncommon => new Avalonia.Media.Color(255, 25, 76, 102),
                    RarityType.Rare => new Avalonia.Media.Color(255, 102, 99, 76),
                    RarityType.SuperRare => new Avalonia.Media.Color(255, 95, 115, 127),
                    RarityType.Event => new Avalonia.Media.Color(255, 0, 0, 0),
                    RarityType.Legendary => new Avalonia.Media.Color(255, 0, 0, 0),
                    _ => new Avalonia.Media.Color(255, 0, 0, 0)
                }
            };
        }
    );

    public static FuncValueConverter<RarityType, IBrush> ForegroundSelected = new FuncValueConverter<RarityType, IBrush>(
        r =>
        {
            return new SolidColorBrush
            {
                Color = r switch
                {
                    RarityType.Uncommon => new Avalonia.Media.Color(255, 19, 89, 127),
                    RarityType.Rare => new Avalonia.Media.Color(255, 130, 120, 30),
                    RarityType.SuperRare => new Avalonia.Media.Color(255, 119, 229, 255),
                    RarityType.Event => new Avalonia.Media.Color(255, 255, 255, 255),
                    RarityType.Legendary => new Avalonia.Media.Color(255, 255, 255, 255),
                    _ => new Avalonia.Media.Color(255, 0, 0, 0)
                }
            };
        }
    );

    public static FuncValueConverter<RarityType, IBrush> Background = new FuncValueConverter<RarityType, IBrush>(
    r =>
    {
        return new LinearGradientBrush
        {
            GradientStops = r switch
            {
                RarityType.Uncommon =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 255, 255), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 181, 217, 237), Offset = 1 }
                ],

                RarityType.Rare =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 239, 219, 167), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 249, 241, 184), Offset = 0.5 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 239, 219, 167), Offset = 1 }
                ],

                RarityType.SuperRare =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 203, 166, 237), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 190, 164, 252), Offset = 1 }
                ],

                RarityType.Event =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 190, 205), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 205, 200, 185), Offset = 1 }
                ],

                RarityType.Legendary =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 220, 172, 229), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 211, 158, 173), Offset = 0.16666666666 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 204, 175, 153), Offset = 0.33333333332 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 224, 215, 168), Offset = 0.5 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 216, 229, 172), Offset = 0.66666666664 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 175, 234, 222), Offset = 0.8333333333 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 183, 239, 244), Offset = 1 },
                ],

                _ =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 255, 255), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 255, 255), Offset = 1 }
                ]
            }
        };
    });

    public static FuncValueConverter<RarityType, IBrush> BackgroundSelected = new FuncValueConverter<RarityType, IBrush>(
    r =>
    {
        return new LinearGradientBrush
        {
            GradientStops = r switch
            {
                RarityType.Uncommon =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 255, 255), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 140, 201, 234), Offset = 1 }
                ],

                RarityType.Rare =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 180, 180, 25), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 250, 230, 80), Offset = 0.5 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 240, 180, 25), Offset = 1 }
                ],

                RarityType.SuperRare =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 171, 93, 238), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 145, 97, 254), Offset = 1 }
                ],

                RarityType.Event =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 250, 50, 70), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 250, 115, 70), Offset = 1 }
                ],

                RarityType.Legendary =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 210, 89, 230), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 212, 45, 92), Offset = 0.16666666666 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 206, 115, 41), Offset = 0.33333333332 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 225, 197, 47), Offset = 0.5 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 186, 232, 45), Offset = 0.66666666664 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 71, 237, 206), Offset = 0.8333333333 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 19, 224, 246), Offset = 1 },
                ],

                _ =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 255, 255), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 255, 255), Offset = 1 }
                ]
            }
        };
    });

    public static FuncValueConverter<RarityType, IBrush> Border = new FuncValueConverter<RarityType, IBrush>(
    r =>
    {
        return new LinearGradientBrush {

            GradientStops = r switch
            {
                RarityType.Uncommon =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 255, 255), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 255, 255), Offset = 1 }
                ],

                RarityType.Rare =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 249, 193), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 249, 237, 174), Offset = 1 }
                ],

                RarityType.SuperRare =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 194, 162, 215), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 186, 153, 204), Offset = 1 }
                ],

                RarityType.Event =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 190, 200), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 250, 205, 185), Offset = 1 }
                ],

                RarityType.Legendary =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 201, 154, 206), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 196, 147, 163), Offset = 0.16666666666 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 221, 198, 166), Offset = 0.33333333332 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 216, 206, 162), Offset = 0.5 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 188, 196, 147), Offset = 0.66666666664 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 129, 165, 124), Offset = 0.8333333333 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 133, 178, 174), Offset = 1 },
                ],

                _ =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 0, 0, 0), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 0, 0, 0), Offset = 1 }
                ]
            }
        };
    });

    public static FuncValueConverter<RarityType, IBrush> BorderSelected = new FuncValueConverter<RarityType, IBrush>(
    r =>
    {
        return new LinearGradientBrush
        {
            GradientStops = r switch
            {
                RarityType.Uncommon =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 196, 237, 250), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 188, 225, 247), Offset = 1 }
                ],

                RarityType.Rare =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 255, 245, 85), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 250, 220, 70), Offset = 1 }
                ],
                
                RarityType.SuperRare =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 103, 103, 217), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 175, 115, 206), Offset = 1 }
                ],

                RarityType.Event =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 250, 50, 70), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 250, 120, 150), Offset = 1 }
                ],

                RarityType.Legendary =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 198, 87, 209), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 197, 79, 117), Offset = 0.16666666666 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 224, 168, 89), Offset = 0.33333333332 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 218, 196, 92), Offset = 0.5 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 178, 198, 87), Offset = 0.66666666664 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 114, 167, 108), Offset = 0.8333333333 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 61, 108, 177), Offset = 1 },
                ],

                _ =>
                [
                    new GradientStop { Color = new Avalonia.Media.Color(255, 0, 0, 0), Offset = 0 },
                    new GradientStop { Color = new Avalonia.Media.Color(255, 0, 0, 0), Offset = 1 }
                ]
            }
        };
    });

}