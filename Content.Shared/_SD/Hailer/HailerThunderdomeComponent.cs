using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._SD.Hailer;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HailerThunderdomeComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId HailerAction = "ActionThunderdomeHailer";

    [DataField, AutoNetworkedField]
    public EntityUid? HailActionEntity;
}
