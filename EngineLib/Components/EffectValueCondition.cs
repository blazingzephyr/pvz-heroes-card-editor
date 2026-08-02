
namespace PvZCards.Engine.Components;

[JsonConverter(typeof(ComponentConverter<EffectValueCondition>))]
[GeneratesDataTemplate]
public class EffectValueCondition : Component
{
    [JsonProperty(Path = ["EffectValue"])]
    public EffectValueType EffectValue { get; set; }

    [JsonProperty(Path = ["ComparisonOperator"])]
    [DataTemplateField(Name = "Operator", Tooltip = "Which comparison operation is used")]
    public ComparisonOperator Operator { get; set; }

    [JsonProperty(Path = ["ValueAmount"])]
    [DataTemplateField(Name = "Value", Tooltip = "Comparison Value")]
    public int Value { get; set; } = 1;
}
