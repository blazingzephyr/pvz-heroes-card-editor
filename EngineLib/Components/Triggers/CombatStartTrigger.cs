
namespace PvZCards.Engine.Components;

[JsonConverter(typeof(ComponentConverter<CombatStartTrigger>))]
public class CombatStartTrigger : Trigger
{

}
