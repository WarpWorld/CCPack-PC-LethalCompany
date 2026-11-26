#nullable disable
using ConnectorLib.JSON;

namespace CrowdControl.Delegates.Effects;

/// <summary>
/// Base class for this pack's timed effects. The scheduler owns the countdown, pause/resume and
/// conflict handling; this class bridges it to the apply / revert / tick logic in <see cref="Buff"/>
/// (TimedEffects.cs), which predates the scheduler.
/// </summary>
public abstract class TimedGameEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    /// <summary>
    /// The old BuffThread runner ticked buffs once every 16 game frames, and some buffs (the pitch
    /// effects) re-send a chat RPC every 16 of those ticks. The scheduler ticks every frame, so keep
    /// the old cadence rather than flooding the server's chat.
    /// </summary>
    private const int FRAMES_PER_BUFF_TICK = 16;

    private readonly Dictionary<uint, (Buff buff, int frames)> m_active = new();

    /// <summary>The default length, used when the request does not carry a duration.</summary>
    private const long DEFAULT_DURATION_MS = 30_000;

    /// <summary>Validates the game state for this request (the old delegate). Anything but Success refuses it.</summary>
    protected abstract EffectResponse Precheck(EffectRequest request);

    /// <summary>The buff this request applies.</summary>
    protected abstract BuffType Type(EffectRequest request);

    public override EffectResponse Start(EffectRequest request)
    {
        EffectResponse response = Precheck(request);
        if (response.status != EffectStatus.Success) return response;

        Buff buff = new(Type(request));
        long duration = (request.duration is > 0) ? request.duration.Value : DEFAULT_DURATION_MS;
        buff.addBuff((int)Math.Min(duration, int.MaxValue));
        ActiveBuffs.Add(buff);
        m_active[request.ID] = (buff, 0);
        return response;
    }

    public override EffectResponse Tick(EffectRequest request)
    {
        if (!m_active.TryGetValue(request.ID, out var entry)) return null;

        int frames = entry.frames + 1;
        if (frames >= FRAMES_PER_BUFF_TICK)
        {
            frames = 0;
            entry.buff.tick();
        }
        m_active[request.ID] = (entry.buff, frames);
        return null;
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        if (m_active.TryGetValue(request.ID, out var entry))
        {
            m_active.Remove(request.ID);
            ActiveBuffs.Remove(entry.buff);
            entry.buff.removeBuff();
        }
        return EffectResponse.Finished(request.ID);
    }
}
