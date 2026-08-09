
namespace PvZCards.Engine.Components;

[JsonConverter(typeof(ComponentConverter<DrawnCardCostMultiplier>))]
[GeneratesDataTemplate]
public class DrawnCardCostMultiplier : MultiplierComponent
{

}
