//this project is a retrofit, it should NOT be used as part of any example - kat
using ConnectorLib.JSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace BepinControl;

public enum TimedType
{
    HYPER_MOVE,
    FAST_MOVE,
    SLOW_MOVE,
    FREEZE,
    ULTRA_JUMP,
    HIGH_JUMP,
    LOW_JUMP,
    OHKO,
    INVUL,
    NOSTAM,
    INFSTAM,
    NIGHT_VISION,
    HIGH_PITCH,
    LOW_PITCH,
    DRUNK,
}

public class Timed
{
    public TimedType type;
    float old;

    public Timed(TimedType t) { 
        type = t;
        switch (type)
        {
            case TimedType.OHKO:
            case TimedType.INVUL:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                old = playerRef.health;
                break;
            }
        }
    }

    public void addEffect(long duration)
    {
        switch (type)
        {
            case TimedType.DRUNK:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                float calculatedDrunkness = Math.Min(duration * 10f, 1200f);
                playerRef.drunkness = calculatedDrunkness;
                playerRef.drunknessSpeed = 1f;
                playerRef.drunknessInertia = 20f;
                break;
            }
            case TimedType.LOW_PITCH:
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_0.65</size>");
                });
                break;
            }
            case TimedType.HIGH_PITCH:
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_1.5</size>");
                });
                break;
            }
            case TimedType.NIGHT_VISION:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                Mod.nightVision = true;
                break;
            }
            case TimedType.FREEZE:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                playerRef.movementSpeed = 0;
                break;
            }
            case TimedType.HYPER_MOVE:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                playerRef.movementSpeed = 18.0f;
                break;
            }
            case TimedType.FAST_MOVE:
            { 
                var playerRef = StartOfRound.Instance.localPlayerController;
                playerRef.movementSpeed = 9.0f;
                break;
            }
            case TimedType.SLOW_MOVE:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                playerRef.movementSpeed = 1.25f;
                break;
            }

            case TimedType.ULTRA_JUMP:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                playerRef.jumpForce = 50.0f;
                break;
            }
            case TimedType.HIGH_JUMP:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                playerRef.jumpForce = 35.0f;
                break;
            }
            case TimedType.LOW_JUMP:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                playerRef.jumpForce = 5.0f;
                break;
            }
        }
    }

    public void removeEffect()
    {
        switch (type)
        {
            case TimedType.DRUNK:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                playerRef.drunkness = 0f;
                playerRef.drunknessSpeed = 0f;
                playerRef.drunknessInertia = 0f;
                break;
            }
            case TimedType.HIGH_PITCH:
            case TimedType.LOW_PITCH:
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_1.0</size>");
                });
                break;
            }
            case TimedType.NIGHT_VISION:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                Mod.nightVision = false;
                break;
            }
            case TimedType.HYPER_MOVE:
            case TimedType.SLOW_MOVE:
            case TimedType.FAST_MOVE:
            case TimedType.FREEZE:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                playerRef.movementSpeed = 4.6f;
                break;
            }
            case TimedType.ULTRA_JUMP:
            case TimedType.HIGH_JUMP:
            case TimedType.LOW_JUMP:
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                playerRef.jumpForce = 13.0f;
                break;
            }
            case TimedType.INVUL:
            case TimedType.OHKO:
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    var playerRef = StartOfRound.Instance.localPlayerController;
                    playerRef.health = (int)old;

                    if (playerRef.health >= 20)
                    {
                        playerRef.MakeCriticallyInjured(false);
                    }
                    if (playerRef.health < 10)
                    {
                        playerRef.MakeCriticallyInjured(true);
                    }

                    HUDManager.Instance.UpdateHealthUI(playerRef.health, true);
                });

                break;
            }
        }
    }
    static int frames = 0;

    public void tick()
    {
        frames++;
        var playerRef = StartOfRound.Instance.localPlayerController;

        switch (type)
        {
            case TimedType.HIGH_PITCH:
                if(frames%16==0)
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_1.5</size>");
                break;
            case TimedType.LOW_PITCH:
                if (frames % 16 == 0)
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_0.65</size>");
                break;
            case TimedType.OHKO:
                if(playerRef.health > 1)playerRef.health = 1;

                if (playerRef.health >= 20)
                {
                    playerRef.MakeCriticallyInjured(false);
                }
                if (playerRef.health < 10)
                {
                    playerRef.MakeCriticallyInjured(true);
                }

                HUDManager.Instance.UpdateHealthUI(playerRef.health, true);

                break;
            case TimedType.INVUL:
                if(playerRef.health > 0)playerRef.health = 100;

                if (playerRef.health >= 20)
                {
                    playerRef.MakeCriticallyInjured(false);
                }
                if (playerRef.health < 10)
                {
                    playerRef.MakeCriticallyInjured(true);
                }

                HUDManager.Instance.UpdateHealthUI(playerRef.health, true);

                break;
            case TimedType.INFSTAM:
                playerRef.sprintMeter = 1.0f;
                playerRef.isExhausted = false;
                break;
            case TimedType.NOSTAM:
                playerRef.sprintMeter = 0.0f;
                playerRef.isExhausted = true;
                break;

        }
    }
}

public class TimedThread
{
    public static List<TimedThread> threads = new();

    public readonly Timed effect;
    public long duration;
    public long remain;
    public uint id;
    public bool paused;

    public static bool isRunning(TimedType t)
    {
        foreach (var thread in threads)
        {
            if (thread.effect.type == t) return true;
        }
        return false;
    }


    public static void tick()
    {
        foreach (var thread in threads)
        {
            if (!thread.paused)
                thread.effect.tick();
        }
    }
    public static void addTime(int duration)
    {
        try
        {
            lock (threads)
            {
                foreach (var thread in threads)
                {
                    Interlocked.Add(ref thread.duration, duration + 5);
                    if (!thread.paused)
                    {
                        long time = Volatile.Read(ref thread.remain);
                        ControlClient.Instance.Send(new EffectResponse(thread.id, EffectStatus.Paused, time));
                        thread.paused = true;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Mod.mls.LogInfo(e.ToString());
        }
    }

    public static void tickTime(int duration)
    {
        try
        {
            lock (threads)
            {
                foreach (var thread in threads)
                {
                    long time = Volatile.Read(ref thread.remain);
                    time -= duration;
                    if (time < 0) time = 0;
                    Volatile.Write(ref thread.remain, time);
                }
            }
        }
        catch (Exception e)
        {
            Mod.mls.LogInfo(e.ToString());
        }
    }

    public static void unPause()
    {
        try
        {
            lock (threads)
            {
                foreach (var thread in threads)
                {
                    if (thread.paused)
                    {
                        long time = Volatile.Read(ref thread.remain);
                        ControlClient.Instance.Send(new EffectResponse(thread.id, EffectStatus.Resumed, time));
                        thread.paused = false;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Mod.mls.LogInfo(e.ToString());
        }
    }

    public TimedThread(uint id, TimedType type, long duration)
    {
        effect = new Timed(type);
        this.duration = duration;
        remain = duration;
        this.id = id;
        paused = false;

        try
        {
            lock (threads)
            {
                threads.Add(this);
            }
        }
        catch (Exception e)
        {
            Mod.mls.LogInfo(e.ToString());
        }
    }

    public void Run()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        effect.addEffect(duration);

        try
        {
            long time = Volatile.Read(ref duration);
            while (time > 0)
            {
                Interlocked.Add(ref duration, -time);
                Thread.Sleep((int)time);

                time = Volatile.Read(ref duration);
            }
            effect.removeEffect();
            lock (threads)
            {
                threads.Remove(this);
            }
            ControlClient.Instance.Send(new EffectResponse(id, EffectStatus.Finished, 0));
        }
        catch (Exception e)
        {
            Mod.mls.LogInfo(e.ToString());
        }
    }
}