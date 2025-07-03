using BepinControl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

namespace ControlValley
{
    public enum BuffType
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
    public class Buff
    {
        public BuffType type;
        float old;

        public Buff(BuffType t) { 
            type = t;
            switch (type)
            {
                case BuffType.OHKO:
                case BuffType.INVUL:
                    {
                    var playerRef = StartOfRound.Instance.localPlayerController;
                    old = playerRef.health;
                    break;
                }
            }
        }

        public void addBuff(int duration)
        {
            switch (type)
            {
                case BuffType.DRUNK:
                    {
                        var playerRef = StartOfRound.Instance.localPlayerController;
                        float calculatedDrunkness = Math.Min(duration * 10f, 1200f);
                        playerRef.drunkness = calculatedDrunkness;
                        playerRef.drunknessSpeed = 1f;
                        playerRef.drunknessInertia = 20f;
                        break;
                    }
                case BuffType.LOW_PITCH:
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_0.65</size>");
                        });
                        break;
                    }
                case BuffType.HIGH_PITCH:
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_1.5</size>");
                        });
                        break;
                    }
                case BuffType.NIGHT_VISION:
                    {
                        var playerRef = StartOfRound.Instance.localPlayerController;
                        LethalCompanyControl.nightVision = true;
                        break;
                    }
                case BuffType.FREEZE:
                    {
                        var playerRef = StartOfRound.Instance.localPlayerController;
                        playerRef.movementSpeed = 0;
                        break;
                    }
                case BuffType.HYPER_MOVE:
                {
                    var playerRef = StartOfRound.Instance.localPlayerController;
                    playerRef.movementSpeed = 18.0f;
                    break;
                }
                case BuffType.FAST_MOVE:
                { 
                    var playerRef = StartOfRound.Instance.localPlayerController;
                    playerRef.movementSpeed = 9.0f;
                    break;
                }
                case BuffType.SLOW_MOVE:
                {
                    var playerRef = StartOfRound.Instance.localPlayerController;
                    playerRef.movementSpeed = 1.25f;
                    break;
                }

                case BuffType.ULTRA_JUMP:
                {
                    var playerRef = StartOfRound.Instance.localPlayerController;
                    playerRef.jumpForce = 50.0f;
                    break;
                }
                case BuffType.HIGH_JUMP:
                {
                    var playerRef = StartOfRound.Instance.localPlayerController;
                    playerRef.jumpForce = 35.0f;
                    break;
                }
                case BuffType.LOW_JUMP:
                {
                    var playerRef = StartOfRound.Instance.localPlayerController;
                    playerRef.jumpForce = 5.0f;
                    break;
                }
            }
        }

        public void removeBuff()
        {
            switch (type)
            {
                case BuffType.DRUNK:
                    {
                        var playerRef = StartOfRound.Instance.localPlayerController;
                        playerRef.drunkness = 0f;
                        playerRef.drunknessSpeed = 0f;
                        playerRef.drunknessInertia = 0f;
                        break;
                    }
                case BuffType.HIGH_PITCH:
                case BuffType.LOW_PITCH:
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_1.0</size>");
                        });
                        break;
                    }
                case BuffType.NIGHT_VISION:
                    {
                        var playerRef = StartOfRound.Instance.localPlayerController;
                        LethalCompanyControl.nightVision = false;
                        break;
                    }
                case BuffType.HYPER_MOVE:
                case BuffType.SLOW_MOVE:
                case BuffType.FAST_MOVE:
                case BuffType.FREEZE:
                    {
                    var playerRef = StartOfRound.Instance.localPlayerController;
                    playerRef.movementSpeed = 4.6f;
                    break;
                }
                case BuffType.ULTRA_JUMP:
                case BuffType.HIGH_JUMP:
                case BuffType.LOW_JUMP:
                {
                    var playerRef = StartOfRound.Instance.localPlayerController;
                    playerRef.jumpForce = 13.0f;
                    break;
                }
                case BuffType.INVUL:
                case BuffType.OHKO:
                {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                case BuffType.HIGH_PITCH:
                    if(frames%16==0)
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_1.5</size>");
                    break;
                case BuffType.LOW_PITCH:
                    if (frames % 16 == 0)
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_0.65</size>");
                    break;
                case BuffType.OHKO:
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
                case BuffType.INVUL:
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
                case BuffType.INFSTAM:
                    playerRef.sprintMeter = 1.0f;
                    playerRef.isExhausted = false;
                    break;
                case BuffType.NOSTAM:
                    playerRef.sprintMeter = 0.0f;
                    playerRef.isExhausted = true;
                    break;

            }
        }
    }
    public class BuffThread
    {
        public static List<BuffThread> threads = new List<BuffThread>();

        public readonly Buff buff;
        public int duration;
        public int remain;
        public int id;
        public bool paused;

        public static bool isRunning(BuffType t)
        {
            foreach (var thread in threads)
            {
                if (thread.buff.type == t) return true;
            }
            return false;
        }


        public static void tick()
        {
            foreach (var thread in threads)
            {
                if (!thread.paused)
                    thread.buff.tick();
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
                        Interlocked.Add(ref thread.duration, duration+5);
                        if (!thread.paused)
                        {
                            int time = Volatile.Read(ref thread.remain);
                            new TimedResponse(thread.id, time, CrowdResponse.Status.STATUS_PAUSE).Send(ControlClient.Socket);
                            thread.paused = true;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LethalCompanyControl.mls.LogInfo(e.ToString());
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
                        int time = Volatile.Read(ref thread.remain);
                        time -= duration;
                        if (time < 0) time = 0;
                        Volatile.Write(ref thread.remain, time);
                    }
                }
            }
            catch (Exception e)
            {
                LethalCompanyControl.mls.LogInfo(e.ToString());
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
                            int time = Volatile.Read(ref thread.remain);
                            new TimedResponse(thread.id, time, CrowdResponse.Status.STATUS_RESUME).Send(ControlClient.Socket);
                            thread.paused = false;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LethalCompanyControl.mls.LogInfo(e.ToString());
            }
        }    
        
        public BuffThread(int id, BuffType buff, int duration)
        {
            this.buff = new Buff(buff);
            this.duration = duration;
            this.remain = duration;
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
                LethalCompanyControl.mls.LogInfo(e.ToString());
            }
        }

        public void Run()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            buff.addBuff(duration);

            try
            {
                int time = Volatile.Read(ref duration); ;
                while (time > 0)
                {
                    Interlocked.Add(ref duration, -time);
                    Thread.Sleep(time);

                    time = Volatile.Read(ref duration);
                }
                buff.removeBuff();
                lock (threads)
                {
                    threads.Remove(this);
                }
                new TimedResponse(id, 0, CrowdResponse.Status.STATUS_STOP).Send(ControlClient.Socket);
            }
            catch (Exception e)
            {
                LethalCompanyControl.mls.LogInfo(e.ToString());
            }
        }
    }
}
