
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using HeroesCardEditor.ViewModels;
using PvZCards.Engine;
using PvZCards.Engine.Components;
using PvZCards.Engine.Queries;
using static HeroesCardEditor.Models.CardProperties;
using Component = PvZCards.Engine.Components.Component;
using Environment = PvZCards.Engine.Components.Environment;

namespace HeroesCardEditor.Models;

/// <summary>
/// Observable wrapper for CardDescriptor for the better editor access.
/// </summary>
public partial class ObservableCardDescriptor : ObservableRecipient, ITabContainer
{
    [ObservableProperty]
    public partial bool HasUnsavedChanged { get; set; }

    /// <summary>
    /// Additional notes that can be added to descriptors.
    /// </summary>
    public string? Note
    {
        get => _desc.Comment;
        set => SetProperty(_desc.Comment, value, b => _desc.Comment = b, nameof(Note));
    }

    /// <summary>
    /// Card's Guid (in PvZCards.Engine.Components.Card)
    /// </summary>
    public uint Guid
    {
        get => _card.Value;
        set => SetProperty(_card.Value, value, b => _card.Value = b, nameof(Guid));
    }

    /// <summary>
    /// Card's Prefab Name
    /// </summary>
    public string? PrefabName
    {
        get => _desc.PrefabName;
        set => SetProperty(_desc.PrefabName, value, b => _desc.PrefabName = b, nameof(PrefabName));
    }

    /// <summary>
    /// Card's class
    /// </summary>
    public Color Color
    {
        get => _desc.Color;
        set => SetProperty(_desc.Color, value, b => _desc.Color = b, nameof(Color));
    }

    /// <summary>
    /// Card's rarity.
    /// </summary>
    public RarityType Rarity
    {
        get => _rarity.Value;
        set
        {
            _desc.Rarity = (uint)value;
            SetProperty(_rarity.Value, value, b => _rarity.Value = b, nameof(Rarity));
        }
    }

    public string? Set
    {
        get => _desc.Set;
        set => SetProperty(_desc.Set, value, b => _desc.Set = b, nameof(Set));
    }

    public string? SetAndRarityKey
    {
        get => _desc.SetAndRarityKey;
        set => SetProperty(_desc.SetAndRarityKey, value, b => _desc.SetAndRarityKey = b, nameof(SetAndRarityKey));
    }

    public int? CraftingBuy
    {
        get => _desc.CraftingBuy;
        set => SetProperty(_desc.CraftingBuy, value, b => _desc.CraftingBuy = b, nameof(CraftingBuy));
    }

    public int? CraftingSell
    {
        get => _desc.CraftingSell;
        set => SetProperty(_desc.CraftingSell, value, b => _desc.CraftingSell = b, nameof(CraftingSell));
    }

    public uint MaxHealth
    {
        get => _desc.DisplayHealth;
        set
        {
            if (value == 0)
            {
                _desc.Components.RemoveMany(from c in _desc.Components where c is Health select c);
            }
            else
            {
                int i = 0;
                for (; i < _desc.Components.Count; i++)
                {
                    if (_desc.Components[i] is Health health)
                    {
                        health.MaxHealth = value;
                        break;
                    }
                }

                if (i == _desc.Components.Count)
                {
                    Health health = new Health { MaxHealth = value };
                    _desc.Components.Insert(1, health);
                }
            }

            SetProperty(_desc.DisplayHealth, value, b => _desc.DisplayHealth = b, nameof(MaxHealth));
        }
    }

    public uint? CurrentDamage
    {
        get => _desc.Components.OfType<Health>().FirstOrDefault()?.CurrentDamage;
        set
        {
            uint? old = null;
            for (int i = 0; i < _desc.Components.Count; i++)
            {
                if (_desc.Components[i] is Health health)
                {
                    old = health.CurrentDamage;
                    health.CurrentDamage = value;
                    break;
                }
            }

            Broadcast(old, value, nameof(CurrentDamage));
            OnPropertyChanged(nameof(CurrentDamage));
        }
    }

    public uint Attack
    {
        get => _desc.DisplayAttack;
        set
        {
            if (value == 0)
            {
                _desc.Components.RemoveMany(from c in _desc.Components where c is Attack select c);
            }
            else
            {
                int i = 0;
                for (; i < _desc.Components.Count; i++)
                {
                    if (_desc.Components[i] is Attack attack)
                    {
                        attack.Value = value;
                        break;
                    }
                }

                if (i == _desc.Components.Count)
                {
                    Attack attack = new Attack { Value = value };
                    _desc.Components.Insert(2, attack);
                }
            }

            SetProperty(_desc.DisplayAttack, value, b => _desc.DisplayAttack = b, nameof(Attack));
        }
    }

    public uint SunCost
    {
        get => _desc.DisplaySunCost;
        set
        {
            _desc.Components.OfType<SunCost>().First().Value = value;
            SetProperty(_desc.DisplaySunCost, value, b => _desc.DisplaySunCost = b, nameof(SunCost));
        }
    }

    public CardType Type
    {
        get
        {
            return _desc.BaseID switch
            {
                _ when _desc.Components.Any(p => p is BoardAbility)                     => CardType.BoardAbility,
                BaseIdType.Base or BaseIdType.BaseZombie                                => CardType.Fighter,
                BaseIdType.BasePlantOneTimeEffect or BaseIdType.BaseZombieOneTimeEffect => CardType.Trick,
                BaseIdType.BasePlantEnvironment or BaseIdType.BaseZombieEnvironment     => CardType.Environment,
                _                                                                       => throw new Exception()
            };
        }
        set
        {
            SetBase(value, Faction);
            Broadcast(Type, value, nameof(Type));
            OnPropertyChanged(nameof(Type));
        }
    }

    public Faction Faction
    {
        get => _desc.Faction;
        set
        {
            var old = _desc.Faction;
            Component? oldComponent = old switch
            {
                Faction.Plants => _desc.Components.OfType<Plants>().First(),
                Faction.Zombies => _desc.Components.OfType<Zombies>().First(),
                _ => null
            };

            Component? newComponent = value switch
            {
                Faction.Plants => new Plants(),
                Faction.Zombies => new Zombies(),
                _ => null
            };

            switch ((old, value))
            {
                case (Faction.All, Faction.All): break;
                case (_, Faction.All): _desc.Components.Remove(oldComponent!); break;
                case (Faction.All, _): _desc.Components.Insert(3, newComponent!); break;
                default: _desc.Components[_desc.Components.IndexOf(oldComponent!)] = newComponent!; break;
            }

            SetBase(Type, value);
            SetProperty(old, value, b => _desc.Faction = b, nameof(Faction));
        }
    }

    public bool IgnoreDeckLimit
    {
        get => _desc.IgnoreDeckLimit;
        set => SetProperty(_desc.IgnoreDeckLimit, value, b => _desc.IgnoreDeckLimit = b, nameof(IgnoreDeckLimit));
    }

    public bool Usable
    {
        get => _desc.Usable;
        set
        {
            if (!value)
            {
                _desc.Components.Insert(0, new Unusable { });
            }
            else
            {
                _desc.Components.Remove(_desc.Components.First(p => p is Unusable));
            }

            SetProperty(_desc.Usable, value, b => _desc.Usable = b, nameof(Usable));
        }
    }

    public bool IsPower
    {
        get => _desc.IsPower;
        set
        {
            if (value)
            {
                _desc.Components.Insert(3, new Superpower { });
            }
            else
            {
                _desc.Components.Remove(_desc.Components.First(p => p is Superpower));
            }

            SetProperty(_desc.IsPower, value, b => _desc.IsPower = b, nameof(IsPower));
        }
    }

    public bool IsPrimaryPower
    {
        get => _desc.IsPrimaryPower;
        set
        {
            if (value)
            {
                _desc.Components.Insert(4, new PrimarySuperpower { });
            }
            else
            {
                _desc.Components.Remove(_desc.Components.First(p => p is PrimarySuperpower));
            }

            SetProperty(_desc.IsPrimaryPower, value, b => _desc.IsPrimaryPower = b, nameof(IsPrimaryPower));
        }
    }

    public bool IsAquatic
    {
        get => _aquatic is not null;
        set
        {
            _desc.IsAquatic = value;
            SetComponent(ref _aquatic, IsAquatic, value, nameof(IsAquatic));
        }
    }

    public bool IsTeamup
    {
        get => _desc.IsTeamup;
        set
        {
            if (value)
            {
                _desc.Components.Add(new Teamup());
            }
            else
            {
                _desc.Components.Remove(_desc.Components.First(p => p is Teamup));
            }

            SetProperty(_desc.IsTeamup, value, p => _desc.IsTeamup = p, nameof(IsTeamup));
        }
    }

    public bool CreateInFront
    {
        get => _createInFront is not null;
        set => SetComponent(ref _createInFront, CreateInFront, value, nameof(CreateInFront));
    }

    public bool Strikethrough
    {
        get => _strikethrough is not null;
        set => SetComponent(ref _strikethrough, Strikethrough, value, nameof(Strikethrough));
    }

    public bool Truestrike
    {
        get => _truestrike is not null;
        set => SetComponent(ref _truestrike, Truestrike, value, nameof(Truestrike));
    }

    public uint Armor
    {
        get => _armor is null ? 0 : _armor.Value;
        set => SetComponent(ref _armor, Armor, value > 0, nameof(Armor), () =>
        {
            if (_armor is not null)
            {
                _armor.Value = value;
            }
        });
    }

    public bool Frenzy
    {
        get => _frenzy is not null;
        set => SetComponent(ref _frenzy, Frenzy, value, nameof(Frenzy));
    }

    public bool Deadly
    {
        get => _deadly is not null;
        set => SetComponent(ref _deadly, Deadly, value, nameof(Deadly));
    }

    public bool Unhealable
    {
        get => _unhealable is not null;
        set => SetComponent(ref _unhealable, Unhealable, value, nameof(Unhealable));
    }

    public int Untrickable
    {
        get => _untrickable is null ? -1 : _untrickable.Counters[0].Value;
        set => SetComponent(ref _untrickable, Untrickable, value > -1, nameof(Untrickable), () =>
        {
            if (_untrickable is not null)
            {
                _untrickable.Counters[0].Value = value;
            }
        });
    }

    public int AttackOverride
    {
        get => _attackOverride is null ? -1 : _attackOverride.Counters[0].Value;
        set => SetComponent(ref _attackOverride, AttackOverride, value > -1, nameof(AttackOverride), () =>
        {
            if (_attackOverride is not null)
            {
                _attackOverride.Counters[0].Value = value;
            }
        });
    }

    public bool Surprise
    {
        get => _surprise is not null;
        set => SetComponent(ref _surprise, Surprise, value, nameof(Surprise));
    }

    public bool PlaysFaceDown
    {
        get => _playsFaceDown is not null;
        set => SetComponent(ref _playsFaceDown, PlaysFaceDown, value, nameof(PlaysFaceDown));
    }

    public bool Multishot
    {
        get => _multishot is not null;
        set => SetComponent(ref _multishot, Multishot, value, nameof(Multishot));
    }

    public bool AttacksInAllLanes
    {
        get => _attacksInAllLanes is not null;
        set => SetComponent(ref _attacksInAllLanes, AttacksInAllLanes, value, nameof(AttacksInAllLanes));
    }

    public bool AttacksOnlyInAdjacentLanes
    {
        get => _attacksOnlyInAdjacentLanes is not null;
        set => SetComponent(ref _attacksOnlyInAdjacentLanes, AttacksOnlyInAdjacentLanes, value, nameof(AttacksOnlyInAdjacentLanes));
    }

    public uint SplashDamage
    {
        get => _splashDamage is null ? 0 : _splashDamage.Value;
        set => SetComponent(ref _splashDamage, SplashDamage, value > 0, nameof(SplashDamage), () =>
        {
            if (_splashDamage is not null)
            {
                _splashDamage.Value = value;
            }
        });
    }

    public bool Evolvable
    {
        get => _evolvable is not null;
        set => SetComponent(ref _evolvable, Evolvable, value, nameof(Evolvable));
    }

    public bool Springboard
    {
        get => _springboard is not null;
        set => SetComponent(ref _evolvable, Springboard, value, nameof(Springboard));
    }

    public Query? EvolutionRestriction
    {
        get => _evolutionRestriction?.Query;
        set
        {
            SetComponent(ref _evolutionRestriction, EvolutionRestriction, value is not null, nameof(EvolutionRestriction), () =>
            {
                if (_evolutionRestriction is not null && value is not null)
                {
                    _evolutionRestriction.Query = value;
                }
            });
        }
    }

    public string Tags
    {
        get => _tags;
        set
        {
            var val = value.Split(';');
            var old = _tags;
            _tags = value;
            _desc.Tags = [.. val];

            SetComponent(ref _tagsComponent, old, val.Length > 0, nameof(Tags), () =>
            {
                if (_tagsComponent is not null)
                {
                    _tagsComponent.CardTags = [.. val];
                }
            });
        }
    }

    public ObservableCollection<GrantedTriggeredAbility> GrantedAbilities { get; set; } = [];
    public ObservableCollection<SpecialAbility> SpecialAbilities { get; set; }
    public ObservableCollection<Tribe> Subtypes { get; set; }

    [ObservableProperty]
    public partial EffectEntity? SelectedEntity { get; set; }
    [ObservableProperty]
    public partial Component? SelectedComponent { get; set; }
    public EffectEntitiesDescriptor? EED { get; set; }

    public CardDescriptor Descriptor => _desc;

    private readonly CardDescriptor _desc;
    private readonly Card _card = new Card();
    private readonly Rarity _rarity = new Rarity();
    private CreateInFront? _createInFront;
    private Aquatic? _aquatic;
    private Strikethrough? _strikethrough;
    private Truestrike? _truestrike;
    private Armor? _armor;
    private Frenzy? _frenzy;
    private Deadly? _deadly;
    private Unhealable? _unhealable;
    private Untrickable? _untrickable;
    private AttackOverride? _attackOverride;
    private Surprise? _surprise;
    private PlaysFaceDown? _playsFaceDown;
    private Multishot? _multishot;
    private AttacksInAllLanes? _attacksInAllLanes;
    private AttacksOnlyInAdjacentLanes? _attacksOnlyInAdjacentLanes;
    private SplashDamage? _splashDamage;
    private Evolvable? _evolvable;
    private Springboard? _springboard;
    private EvolutionRestriction? _evolutionRestriction;
    private Tags? _tagsComponent;
    private string _tags;

    public ObservableCardDescriptor(CardDescriptor inner)
    {
        _desc = inner;
        _tags = string.Join(';', inner.Tags);

        SpecialAbilities = [.._desc.SpecialAbilities];
        SpecialAbilities.CollectionChanged += (s, e) =>
        {
            _desc.SpecialAbilities = SpecialAbilities;
            if (SpecialAbilities.Count == 0)
            {
                if (_desc.Components.FirstOrDefault(p => p is ShowTriggeredIcon) is ShowTriggeredIcon sti)
                    _desc.Components.Remove(sti);
            }
            else
            {
                if (_desc.Components.FirstOrDefault(p => p is ShowTriggeredIcon) is not ShowTriggeredIcon sti)
                {
                    sti = new ShowTriggeredIcon();
                    _desc.Components.Add(sti);
                }
                else
                {
                    sti.Values.Clear();
                }

                foreach (SpecialAbility ability in SpecialAbilities)
                {
                    if ((uint)ability > 6)
                    {
                        sti.Values.Add((uint)ability);
                    }
                }
            }
            OnPropertyChanged(nameof(SpecialAbilities));
        };

        Subtypes = [.._desc.Subtypes.Select(p => {
            if (Enum.TryParse(p, out Tribe result)) return result;
            return Tribe.Custom1;
        })];

        Subtypes.CollectionChanged += (s, e) =>
        {
            _desc.Subtypes = [..Subtypes.Select(p => p.ToString())];
            _desc.SubtypeAffinities = [.. Subtypes.Select(p => p.ToString())];

            if (Subtypes.Count == 0)
            {
                if (_desc.Components.FirstOrDefault(p => p is Subtypes) is Subtypes sub)
                    _desc.Components.Remove(sub);
            }
            else
            {
                if (_desc.Components.FirstOrDefault(p => p is Subtypes) is not Subtypes sub)
                {
                    sub = new Subtypes();
                    _desc.Components.Add(sub);
                }

                sub.Values = [..Subtypes.Select(p => (uint)p)];
            }
            OnPropertyChanged(nameof(Subtypes));
        };

        foreach (Component component in inner.Components)
        {
            switch (component)
            {
                case Card card: _card = card; break;
                case Rarity rarity: _rarity = rarity; break;
                case CreateInFront inFront: _createInFront = inFront; break;
                case Aquatic aquatic: _aquatic = aquatic; break;
                case Strikethrough strikethrough: _strikethrough = strikethrough; break;
                case Truestrike truestrike: _truestrike = truestrike; break;
                case Armor armor: _armor = armor; break;
                case Frenzy frenzy: _frenzy = frenzy; break;
                case Deadly deadly: _deadly = deadly; break;
                case Unhealable unhealable: _unhealable = unhealable; break;
                case Untrickable untrickable: _untrickable = untrickable; break;
                case AttackOverride attackOverride: _attackOverride = attackOverride; break;
                case PlaysFaceDown playsFaceDown: _playsFaceDown = playsFaceDown; break;
                case Multishot multishot: _multishot = multishot; break;
                case AttacksInAllLanes attacksInAllLanes: _attacksInAllLanes = attacksInAllLanes; break;
                case AttacksOnlyInAdjacentLanes attacksOnlyInAdjacentLanes: _attacksOnlyInAdjacentLanes = attacksOnlyInAdjacentLanes; break;
                case SplashDamage splashDamage: _splashDamage = splashDamage; break;
                case Evolvable evolvable: _evolvable = evolvable; break;
                case Springboard springboard: _springboard = springboard; break;
                case EvolutionRestriction evRestriction: _evolutionRestriction = evRestriction; break;
                case GrantedTriggeredAbilities gta: 
                    GrantedAbilities = gta.Abilities;
                    foreach (var a in GrantedAbilities)
                    {
                        a.WhenAnyPropertyChanged().Subscribe(p => OnPropertyChanged(nameof(GrantedAbilities)));
                    }
                    break;
                case Tags tags: _tagsComponent = tags; break;
                case EffectEntitiesDescriptor eed: 
                    EED = eed;
                    foreach (var entity in eed.Entities)
                    {
                        foreach (var c in entity.Components)
                        {
                            c.WhenAnyPropertyChanged().Subscribe(p => OnPropertyChanged(nameof(EED.Entities)));
                        }
                    }
                    break;
            }
        }
    }

    public void AddNewGrantedAbility()
    {
        if (GrantedAbilities is null)
        {
            var gta = new GrantedTriggeredAbilities();
            GrantedAbilities = gta.Abilities;
            _desc.Components.Add(gta);
        }

        GrantedTriggeredAbility triggeredAbility = new GrantedTriggeredAbility();
        triggeredAbility
            .WhenAnyPropertyChanged()
            .Subscribe(p => OnPropertyChanged(nameof(GrantedAbilities)));

        GrantedAbilities.Add(triggeredAbility);
        OnPropertyChanged(nameof(GrantedAbilities));
    }

    public void RemoveGrantedAbility(GrantedTriggeredAbility? gta)
    {
        if (gta is null) return;
        GrantedAbilities.Remove(gta);
        OnPropertyChanged(nameof(GrantedAbilities));
    }

    public void CreateNewEffectDescriptor()
    {
        if (EED is null)
        {
            EED = new EffectEntitiesDescriptor { Entities = [] };
            _desc.Components.Add(EED);
        }

        EED.Entities.Add(new EffectEntity { });
        OnPropertyChanged(nameof(EED));
    }

    public void CreateNewComponentOfType(IReadOnlyList<object> param)
    {
        if (param.Count == 2 &&
            param[0] is EffectEntity entity &&
            param[1] is Type componentType &&
            Activator.CreateInstance(componentType) is Component component)
        {
            component.WhenAnyPropertyChanged().Subscribe(p => OnPropertyChanged(nameof(EED.Entities)));
            entity.Components.Add(component);
            OnPropertyChanged(nameof(EED.Entities));
        }
    }

    private void SetComponent<T>(ref T? component, object oldValue, bool shouldExist, string propertyName, Action? afterAction = null)
        where T : Component, new()
    {
        if (shouldExist)
        {
            if (!_desc.Components.OfType<T>().Any())
            {
                component = new T();
                _desc.Components.Add(component);
            }
        }
        else if (component is not null)
        {
            _desc.Components.Remove(component);
            component = null;
        }

        afterAction?.Invoke();

        Broadcast(oldValue, shouldExist, propertyName);
        OnPropertyChanged(propertyName);
    }

    private void SetBase(CardType type, Faction faction)
    {
        _desc.BaseID = (type, faction) switch
        {
            (CardType.BoardAbility, _)              => BaseIdType.BasePlantOneTimeEffect,
            (CardType.Fighter, Faction.All)         => BaseIdType.Base,
            (CardType.Fighter, Faction.Plants)      => BaseIdType.Base,
            (CardType.Fighter, Faction.Zombies)     => BaseIdType.BaseZombie,
            (CardType.Trick, Faction.All)           => BaseIdType.BasePlantOneTimeEffect,
            (CardType.Trick, Faction.Plants)        => BaseIdType.BasePlantOneTimeEffect,
            (CardType.Trick, Faction.Zombies)       => BaseIdType.BaseZombieOneTimeEffect,
            (CardType.Environment, Faction.All)     => BaseIdType.BasePlantEnvironment,
            (CardType.Environment, Faction.Plants)  => BaseIdType.BasePlantEnvironment,
            (CardType.Environment, Faction.Zombies) => BaseIdType.BaseZombieEnvironment,
            _                                       => throw new Exception()
        };

        void Component<A, B>(Component? newComponent = null)
        {
            bool replaced = false;
            bool powerAdded = false;
            bool primaryPowerAdded = false;

            for (int i = 0; i < _desc.Components.Count; i++)
            {
                Component c = _desc.Components[i];
                if (c is A || c is B)
                {
                    if (newComponent is not null)
                    {
                        replaced = true;
                        _desc.Components[i] = newComponent;
                    }
                    else
                    {
                        _desc.Components.RemoveAt(i);
                        i--;
                    }
                }
                else if (c is Superpower)
                {
                    if (!_desc.IsPower)
                    {
                        _desc.Components.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        powerAdded = true;
                    }
                }
                else if (c is PrimarySuperpower)
                {
                    if (!_desc.IsPrimaryPower)
                    {
                        _desc.Components.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        primaryPowerAdded = true;
                    }
                }
            }

            if (!replaced && newComponent is not null)
            {
                _desc.Components.Insert(2, newComponent);
            }
            if (_desc.IsPower && !powerAdded)
            {
                _desc.Components.Insert(3, new Superpower());
            }
            if (_desc.IsPrimaryPower && !primaryPowerAdded)
            {
                _desc.Components.Insert(4, new PrimarySuperpower());
            }
        }

        switch (type)
        {
            case CardType.BoardAbility:
                Component<Burst, Environment>(new BoardAbility());
                break;

            case CardType.Fighter:
                Component<Burst, Environment>();
                break;

            case CardType.Trick:
                Component<BoardAbility, Environment>(new Burst());
                break;

            case CardType.Environment:
                Component<BoardAbility, Burst>(new Environment());
                break;
        }

        _desc.IsFighter = type == CardType.Fighter;
        _desc.IsEnv = type == CardType.Environment;
    }

    public void CloseTab(IReadOnlyList<object> entry)
    {
        if (entry[1] is EffectEntity effect && EED is not null)
        {
            EED.Entities.Remove(effect);
            OnPropertyChanged(nameof(EED));
        }
    }

    public void DeleteSelectedEffectComponent(Component o)
    {
        SelectedEntity?.Components.Remove(o);
        OnPropertyChanged(nameof(EED.Entities));
    }
}
