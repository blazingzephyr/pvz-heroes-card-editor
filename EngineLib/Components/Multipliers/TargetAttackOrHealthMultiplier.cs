
namespace PvZCards.Engine.Components;

[JsonConverter(typeof(ComponentConverter<TargetAttackOrHealthMultiplier>))]
[GeneratesDataTemplate]
public class TargetAttackOrHealthMultiplier : MultiplierComponent
{

}
