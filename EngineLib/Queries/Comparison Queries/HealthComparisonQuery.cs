
namespace PvZCards.Engine.Queries;

[JsonConverter(typeof(ComponentConverter<HealthComparisonQuery>))]
public partial class HealthComparisonQuery : ComparisonQuery
{
    [ObservableProperty]
    [JsonProperty(Path = ["HealthValue"])]
    public override partial int Value { get; set; }
}
