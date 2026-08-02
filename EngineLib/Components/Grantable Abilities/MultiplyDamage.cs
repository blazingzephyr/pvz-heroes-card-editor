
namespace PvZCards.Engine.Components;

[JsonConverter(typeof(ComponentConverter<MultiplyDamage>))]
public class MultiplyDamage : GrantableAbility
{

}
