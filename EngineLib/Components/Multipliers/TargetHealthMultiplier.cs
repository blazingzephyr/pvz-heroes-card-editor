
namespace PvZCards.Engine.Components;

[JsonConverter(typeof(ComponentConverter<TargetHealthMultiplier>))]
[GeneratesDataTemplate]
public class TargetHealthMultiplier : MultiplierComponent
{

}
