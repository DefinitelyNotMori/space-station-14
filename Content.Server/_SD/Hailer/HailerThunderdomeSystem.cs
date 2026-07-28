using Content.Server.Chat.Systems;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Chat;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Shared._SD.Hailer;

namespace Content.Server._SD.Hailer;

public sealed partial class HailerThunderdomeSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private readonly Dictionary<EntityUid, TimeSpan> _delays = new();
    private readonly TimeSpan _fixed_delay = TimeSpan.FromSeconds(2);

    private readonly string[] _sounds = new[]
    {
        "/Audio/_SD/Hailer/Umresh.ogg",
        "/Audio/_SD/Hailer/DaiteEmyStul.ogg",
        "/Audio/_SD/Hailer/Jackson.ogg",
        "/Audio/_SD/Hailer/SirGround.ogg",
        "/Audio/_SD/Hailer/Vstavai.ogg",
        "/Audio/_SD/Hailer/Tyanetsa.ogg",
    };

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ActionsComponent, ThunderdomeHailerActionEvent>(OnHail);
        SubscribeLocalEvent<HailerThunderdomeComponent, GotEquippedEvent>(OnGotEquipped);
        SubscribeLocalEvent<HailerThunderdomeComponent, GotUnequippedEvent>(OnGotUnequipped);
    }

    private void OnGotEquipped(EntityUid uid, HailerThunderdomeComponent component, GotEquippedEvent args)
    {
        if (args.SlotFlags == SlotFlags.MASK)
        {
            _actionsSystem.AddAction(args.Equipee,
                ref component.HailActionEntity,
                component.HailerAction,
                args.Equipee);
        }
    }

    private void OnGotUnequipped(EntityUid uid, HailerThunderdomeComponent component, GotUnequippedEvent args)
    {
        if (args.SlotFlags == SlotFlags.MASK)
        {
            _actionsSystem.RemoveAction(args.Equipee, component.HailActionEntity);
        }
    }

    private void OnHail(EntityUid uid, ActionsComponent component, ThunderdomeHailerActionEvent args)
    {
        if (args.Handled)
            return;

        if (_delays.TryGetValue(uid, out var until) && _timing.CurTime < until)
            return;

        var rInt = _random.Next(0, _sounds.Length);
        _audio.PlayPvs(_sounds[rInt], uid);
        _delays[uid] = _timing.CurTime.Add(_fixed_delay);

        _chat.TrySendInGameICMessage(uid,
            Loc.GetString($"STUL-{rInt}"),
            InGameICChatType.Speak,
            ChatTransmitRange.GhostRangeLimit,
            nameOverride: Name(uid),
            checkRadioPrefix: false);

        args.Handled = true;
    }
}


