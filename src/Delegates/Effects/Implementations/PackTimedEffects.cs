#nullable disable
using ConnectorLib.JSON;

namespace CrowdControl.Delegates.Effects.Implementations;

// Timed effects. The precheck (GameActions.X) validates the game state; the buff itself lives in
// TimedEffects.cs (Buff.addBuff / removeBuff / tick), driven by the scheduler through TimedGameEffect.
// Durations are the pack definition's defaults (30s where it has none, as before); conflicts mirror the
// "already running" checks the old delegates made.

[Effect(["fast"], 30f, ["fast", "fastjetpack", "freeze", "hyper", "slow"])]
public class FastMoveEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.FastMove(req);
    protected override BuffType Type(EffectRequest req) => BuffType.FAST_MOVE;
}

[Effect(["slow"], 30f, ["fast", "freeze", "hyper", "slow"])]
public class SlowMoveEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.SlowMove(req);
    protected override BuffType Type(EffectRequest req) => BuffType.SLOW_MOVE;
}

[Effect(["hyper"], 15f, ["fast", "freeze", "hyper", "slow"])]
public class HyperMoveEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.HyperMove(req);
    protected override BuffType Type(EffectRequest req) => BuffType.HYPER_MOVE;
}

[Effect(["freeze"], 5f, ["fast", "freeze", "hyper", "slow"])]
public class FreezeEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.Freeze(req);
    protected override BuffType Type(EffectRequest req) => BuffType.FREEZE;
}

[Effect(["drunk"], 10f, ["drunk"])]
public class DrunkEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.Drunk(req);
    protected override BuffType Type(EffectRequest req) => BuffType.DRUNK;
}

[Effect(["fastjetpack"], 30f, ["fast", "fastjetpack", "freeze", "hyper", "slow"])]
public class FastJetpackEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.FastJetpack(req);
    protected override BuffType Type(EffectRequest req) => BuffType.FASTJETPACK;
}

[Effect(["jumpultra"], 15f, ["jumphigh", "jumplow", "jumpultra"])]
public class UltraJumpEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.UltraJump(req);
    protected override BuffType Type(EffectRequest req) => BuffType.ULTRA_JUMP;
}

[Effect(["jumphigh"], 30f, ["jumphigh", "jumplow", "jumpultra"])]
public class HighJumpEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.HighJump(req);
    protected override BuffType Type(EffectRequest req) => BuffType.HIGH_JUMP;
}

[Effect(["jumplow"], 30f, ["jumphigh", "jumplow", "jumpultra"])]
public class LowJumpEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.LowJump(req);
    protected override BuffType Type(EffectRequest req) => BuffType.LOW_JUMP;
}

[Effect(["ohko"], 30f, ["invul", "ohko"])]
public class OHKOEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.OHKO(req);
    protected override BuffType Type(EffectRequest req) => BuffType.OHKO;
}

[Effect(["invul"], 30f, ["invul", "ohko"])]
public class InvulEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.Invul(req);
    protected override BuffType Type(EffectRequest req) => BuffType.INVUL;
}

[Effect(["infstam"], 30f, ["infstam", "nostam"])]
public class InfiniteStaminaEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.InfiniteStamina(req);
    protected override BuffType Type(EffectRequest req) => BuffType.INFSTAM;
}

[Effect(["nostam"], 30f, ["infstam", "nostam"])]
public class NoStaminaEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.NoStamina(req);
    protected override BuffType Type(EffectRequest req) => BuffType.NOSTAM;
}

[Effect(["nightvision"], 30f, ["nightvision"])]
public class NightVisionEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.NightVision(req);
    protected override BuffType Type(EffectRequest req) => BuffType.NIGHT_VISION;
}

[Effect(["highpitch"], 30f, ["highpitch", "lowpitch"])]
public class HighPitchEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.HighPitch(req);
    protected override BuffType Type(EffectRequest req) => BuffType.HIGH_PITCH;
}

[Effect(["lowpitch"], 30f, ["highpitch", "lowpitch"])]
public class LowPitchEffect(CrowdControlMod mod, NetworkClient client) : TimedGameEffect(mod, client)
{
    protected override EffectResponse Precheck(EffectRequest req) => GameActions.LowPitch(req);
    protected override BuffType Type(EffectRequest req) => BuffType.LOW_PITCH;
}
