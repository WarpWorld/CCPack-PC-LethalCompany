#nullable disable
using BepinControl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

namespace CrowdControl
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
        FASTJETPACK,
        SLOWJETPACK,
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
                case BuffType.FASTJETPACK:
                {
                    var playerRef = StartOfRound.Instance.localPlayerController;
                    foreach(JetpackItem Jetpack in UnityEngine.Object.FindObjectsByType<JetpackItem>(FindObjectsSortMode.None))
                    {
                        if (Jetpack != null && Jetpack.playerHeldBy == playerRef)
                        {
                            LethalCompanyControl.oldJetpackRefSpeed = Jetpack.jetpackAcceleration;
                            Jetpack.jetpackAcceleration = Jetpack.jetpackAcceleration * 3;
                        }
                    }
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
                case BuffType.FASTJETPACK:
                {
                    var playerRef = StartOfRound.Instance.localPlayerController;
                    foreach (JetpackItem Jetpack in UnityEngine.Object.FindObjectsByType<JetpackItem>(FindObjectsSortMode.None))
                    {
                        if (Jetpack != null && Jetpack.playerHeldBy == playerRef)
                        {
                            Jetpack.jetpackAcceleration = LethalCompanyControl.oldJetpackRefSpeed;
                        }
                    }
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
    /// <summary>
    /// The buffs currently applied, for the "is X already running" prechecks in GameActions.
    /// Replaces the old BuffThread list: the scheduler now owns the countdown, pause/resume and
    /// finish, and TimedGameEffect applies/reverts/ticks the <see cref="Buff"/> itself.
    /// </summary>
    public static class ActiveBuffs
    {
        private static readonly List<Buff> s_active = new List<Buff>();

        public static bool IsRunning(BuffType t)
        {
            lock (s_active) return s_active.Exists(b => b.type == t);
        }

        internal static void Add(Buff b) { lock (s_active) s_active.Add(b); }

        internal static void Remove(Buff b) { lock (s_active) s_active.Remove(b); }
    }
}
