
namespace PvZCards.Engine.Components;

[JsonConverter(typeof(ComponentConverter<TargetAttackMultiplier>))]
[GeneratesDataTemplate]
public class TargetAttackMultiplier : MultiplierComponent
{

}
