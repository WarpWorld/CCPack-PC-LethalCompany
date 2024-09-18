//this project is a retrofit, it should NOT be used as part of any example - kat
using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using ConnectorLib.JSON;
using Unity.Netcode;
using UnityEngine;

namespace BepinControl;

public delegate EffectResponse EffectDelegate(ControlClient client, EffectRequest req);

public class EffectDelegates
{
    public static uint msgid = 0;

    public static uint givedelay = 0;

    public enum BuyableItemList
    {
        walkie = 0,
        flashlight = 1,
        shovel = 2,
        lockpicker = 3,
        proflashlight = 4,
        stungrenade = 5,
        boombox = 6,
        inhaler = 7,
        stungun = 8,
        jetpack = 9,
        extensionladder = 10,
        radarbooster = 11,
        spraypaint = 12,
        weedkiller = 13
    }
    public enum buyableVehiclesList//Future Planning
    {
        Cruiser = 0
    }

    #region Health
    public static EffectResponse HealFull(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";


        try
        {
            NetworkManager networkManager = StartOfRound.Instance.NetworkManager;
            if (networkManager == null || !networkManager.IsListening)
            {
                status = EffectStatus.Retry;
            }
            else
            {
                Mod.test = true;

                //StartOfRound.Instance.ChangeLevel(6);
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }


        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse Kill(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;

            if (playerRef.health <= 0 || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    if (Mod.isHost)
                    {
                        playerRef.KillPlayer(playerRef.transform.up * 100.0f, true, CauseOfDeath.Inertia, 0);
                    }
                    else
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_kill_{(int)playerRef.playerClientId}</size>");
                    }
                });
            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse KillCrewmate(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        List<PlayerControllerB> list = new List<PlayerControllerB>();

        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
        {
            if (player != null && !player.isPlayerDead && player != playerRef && player.isActiveAndEnabled && player.isPlayerControlled)
                list.Add(player);
        }

        if (list.Count <= 0)
        {
            return new EffectResponse(req.ID, EffectStatus.Retry, "");
        }

        try
        {
            var player = list[UnityEngine.Random.Range(0, list.Count)];

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_kill_{(int)player.playerClientId}</size>");
                });
            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse Damage(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        if (TimedThread.isRunning(TimedType.OHKO)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.INVUL)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;

            if (playerRef.health <= 20 || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    int dmg = 25;
                    if (playerRef.health < 25) dmg = playerRef.health - 1;
                    playerRef.DamagePlayer(dmg);
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse DamageCrew(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        List<PlayerControllerB> list = new List<PlayerControllerB>();

        var playerRef = StartOfRound.Instance.localPlayerController;

        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
        {
            if (player != null && player.health >= 20 && !player.isPlayerDead && player != playerRef && player.isActiveAndEnabled && player.isPlayerControlled)
                list.Add(player);
        }

        if (list.Count <= 0)
        {
            return new EffectResponse(req.ID, EffectStatus.Retry, "");
        }

        try
        {
            var player = list[UnityEngine.Random.Range(0, list.Count)];

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !player.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_damage_{(int)player.playerClientId}</size>");

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse Heal(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        if (TimedThread.isRunning(TimedType.OHKO)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.INVUL)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        try
        {

            var playerRef = StartOfRound.Instance.localPlayerController;

            if (playerRef.health >= 100 || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {

                Mod.ActionQueue.Enqueue(() =>
                {
                    playerRef.health = Mathf.Clamp(playerRef.health + 25, 0, 100);

                    if (playerRef.health >= 20)
                    {
                        playerRef.MakeCriticallyInjured(false);
                    }

                    HUDManager.Instance.UpdateHealthUI(playerRef.health, true);
                });


            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }


        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse HealCrew(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        List<PlayerControllerB> list = new List<PlayerControllerB>();

        var playerRef = StartOfRound.Instance.localPlayerController;

        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
        {
            if (player != null && player.health < 100 && !player.isPlayerDead && player != playerRef && player.isActiveAndEnabled && player.isPlayerControlled)
                list.Add(player);
        }

        if (list.Count <= 0)
        {
            return new EffectResponse(req.ID, EffectStatus.Retry, "");
        }

        try
        {
            var player = list[UnityEngine.Random.Range(0, list.Count)];

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !player.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_heal_{(int)player.playerClientId}</size>");

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }


    public static EffectResponse OHKO(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.OHKO)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.INVUL)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.OHKO, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static EffectResponse Invul(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.OHKO)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.INVUL)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.INVUL, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    #endregion

    #region Stamina
    public static EffectResponse DrainStamins(ControlClient client, EffectRequest req)
    {
        var playerRef = StartOfRound.Instance.localPlayerController;
        if (playerRef.sprintMeter < 0.1f) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        playerRef.sprintMeter = 0;
        playerRef.isExhausted = true;


        return new EffectResponse(req.ID, EffectStatus.Success);
    }

    public static EffectResponse RestoreStamins(ControlClient client, EffectRequest req)
    {
        var playerRef = StartOfRound.Instance.localPlayerController;
        if (playerRef.sprintMeter > 0.9f) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        playerRef.sprintMeter = 1.0f;
        playerRef.isExhausted = false;


        return new EffectResponse(req.ID, EffectStatus.Success);
    }

    public static EffectResponse InfiniteStamina(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.INFSTAM)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.NOSTAM)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.INFSTAM, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static EffectResponse NoStamina(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.INFSTAM)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.NOSTAM)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.NOSTAM, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    #endregion

    #region Sounds
    public static EffectResponse Footstep(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        try
        {


            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    playerRef.PlayFootstepServer();
                    playerRef.movementAudio.PlayOneShot(StartOfRound.Instance.footstepSurfaces[0].clips[0], 4.0f);
                    WalkieTalkie.TransmitOneShotAudio(playerRef.movementAudio, StartOfRound.Instance.footstepSurfaces[0].clips[0], 4.0f);
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static void SpawnAndPlayScreech(SpawnableEnemyWithRarity source)
    {
        var playerRef = StartOfRound.Instance.localPlayerController;

        GameObject obj = UnityEngine.Object.Instantiate(source.enemyType.enemyPrefab, playerRef.transform.position + playerRef.transform.forward * 5.0f, Quaternion.Euler(Vector3.zero));
        //obj.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);

        obj.gameObject.GetComponentInChildren<EnemyAI>().stunNormalizedTimer = 6.0f;

        BaboonBirdAI bird = obj.gameObject.GetComponent<BaboonBirdAI>();

        playerRef.movementAudio.PlayOneShot(bird.cawScreamSFX[0], 4.0f);
        WalkieTalkie.TransmitOneShotAudio(playerRef.movementAudio, bird.cawScreamSFX[0], 4.0f);
        RoundManager.Instance.PlayAudibleNoise(playerRef.transform.position, 12f, 4.0f, 0, false, 911);

        UnityEngine.Object.Destroy(obj);

    }
    public static EffectResponse Screech(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        try
        {


            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    foreach (var level in StartOfRound.Instance.levels)
                    {
                        foreach (var outsideEnemy in level.OutsideEnemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("hawk"))
                            {
                                try
                                {
                                    SpawnAndPlayScreech(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                        foreach (var outsideEnemy in level.Enemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("hawk"))
                            {
                                try
                                {
                                    SpawnAndPlayScreech(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                        foreach (var outsideEnemy in level.DaytimeEnemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("hawk"))
                            {
                                try
                                {
                                    SpawnAndPlayScreech(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                    }

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static void SpawnAndPlayBreathing(SpawnableEnemyWithRarity source)
    {
        var playerRef = StartOfRound.Instance.localPlayerController;

        GameObject obj = UnityEngine.Object.Instantiate(source.enemyType.enemyPrefab, playerRef.transform.position + playerRef.transform.forward * 5.0f, Quaternion.Euler(Vector3.zero));
        //obj.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);

        obj.gameObject.GetComponentInChildren<EnemyAI>().stunNormalizedTimer = 6.0f;

        DressGirlAI bird = obj.gameObject.GetComponent<DressGirlAI>();

        playerRef.movementAudio.PlayOneShot(bird.breathingSFX, 4.0f);
        WalkieTalkie.TransmitOneShotAudio(playerRef.movementAudio, bird.breathingSFX, 4.0f);
        RoundManager.Instance.PlayAudibleNoise(playerRef.transform.position, 12f, 4.0f, 0, false, 911);

        UnityEngine.Object.Destroy(obj);
    }

    public static EffectResponse Breathing(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        try
        {


            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    foreach (var level in StartOfRound.Instance.levels)
                    {
                        foreach (var outsideEnemy in level.OutsideEnemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("girl"))
                            {
                                try
                                {
                                    SpawnAndPlayBreathing(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                        foreach (var outsideEnemy in level.Enemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("girl"))
                            {
                                try
                                {
                                    SpawnAndPlayBreathing(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                        foreach (var outsideEnemy in level.DaytimeEnemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("girl"))
                            {
                                try
                                {
                                    SpawnAndPlayBreathing(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                    }

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static void SpawnAndPlayBlob(SpawnableEnemyWithRarity source)
    {
        var playerRef = StartOfRound.Instance.localPlayerController;

        GameObject obj = UnityEngine.Object.Instantiate(source.enemyType.enemyPrefab, playerRef.transform.position + playerRef.transform.forward * 5.0f, Quaternion.Euler(Vector3.zero));
        //obj.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);

        obj.gameObject.GetComponentInChildren<EnemyAI>().stunNormalizedTimer = 6.0f;

        BlobAI bird = obj.gameObject.GetComponent<BlobAI>();

        playerRef.movementAudio.PlayOneShot(bird.jiggleSFX, 4.0f);
        WalkieTalkie.TransmitOneShotAudio(playerRef.movementAudio, bird.jiggleSFX, 4.0f);
        RoundManager.Instance.PlayAudibleNoise(playerRef.transform.position, 12f, 4.0f, 0, false, 911);

        UnityEngine.Object.Destroy(obj);
    }

    public static EffectResponse BlobSound(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        try
        {


            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    foreach (var level in StartOfRound.Instance.levels)
                    {
                        foreach (var outsideEnemy in level.OutsideEnemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("blob"))
                            {
                                try
                                {
                                    SpawnAndPlayBreathing(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                        foreach (var outsideEnemy in level.Enemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("blob"))
                            {
                                try
                                {
                                    SpawnAndPlayBreathing(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                        foreach (var outsideEnemy in level.DaytimeEnemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("blob"))
                            {
                                try
                                {
                                    SpawnAndPlayBreathing(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                    }

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse HighPitch(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.LOW_PITCH)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.HIGH_PITCH)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        Mod.ActionQueue.Enqueue(() =>
        {
            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_1.5</size>");
        });

        new Thread(new TimedThread(req.ID, TimedType.HIGH_PITCH, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static EffectResponse LowPitch(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.LOW_PITCH)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.HIGH_PITCH)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        Mod.ActionQueue.Enqueue(() =>
        {
            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_0.65</size>");
        });

        new Thread(new TimedThread(req.ID, TimedType.LOW_PITCH, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    #endregion

    #region Players
    public static EffectResponse HyperMove(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.FAST_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.SLOW_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.HYPER_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.FREEZE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.HYPER_MOVE, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static EffectResponse FastMove(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.FAST_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.SLOW_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.HYPER_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.FREEZE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");


        new Thread(new TimedThread(req.ID, TimedType.FAST_MOVE, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static EffectResponse Drunk(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 10000;

        if (TimedThread.isRunning(TimedType.DRUNK)) return new EffectResponse(req.ID, EffectStatus.Retry, "");


        new Thread(new TimedThread(req.ID, TimedType.DRUNK, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static EffectResponse SlowMove(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.FAST_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.SLOW_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.HYPER_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.FREEZE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.SLOW_MOVE, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static EffectResponse Freeze(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.FAST_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.SLOW_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.HYPER_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.FREEZE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");


        new Thread(new TimedThread(req.ID, TimedType.FREEZE, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static EffectResponse UltraJump(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.ULTRA_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.HIGH_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.LOW_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.ULTRA_JUMP, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static EffectResponse HighJump(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.ULTRA_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.HIGH_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.LOW_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.HIGH_JUMP, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static EffectResponse LowJump(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.ULTRA_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.HIGH_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.LOW_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.LOW_JUMP, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static EffectResponse Revive(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        try
        {

            {
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {

                        StartOfRound.Instance.ReviveDeadPlayers();
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_revive</size>");

                    });
                }
            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse TeleportToShip(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        try
        {

            if (GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom) status = EffectStatus.Retry;
            else
            {
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {

                        StartOfRound.Instance.ForcePlayerIntoShip();

                    });
                }
            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }
        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse TeleportCrewToShip(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;


        List<PlayerControllerB> list = new List<PlayerControllerB>();

        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
        {
            if (player != null && !player.isPlayerDead && player != playerRef && player.isActiveAndEnabled && player.isPlayerControlled)
                list.Add(player);
        }

        if (list.Count <= 0)
        {
            return new EffectResponse(req.ID, EffectStatus.Retry, "");
        }

        try
        {
            var player = list[UnityEngine.Random.Range(0, list.Count)];

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_ship_{(int)player.playerClientId}</size>");

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse TeleportToCrew(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        List<PlayerControllerB> list = new List<PlayerControllerB>();

        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
        {
            if (player != null && !player.isPlayerDead && player != playerRef && player.isActiveAndEnabled && player.isPlayerControlled)
                list.Add(player);
        }

        if (list.Count <= 0)
        {
            return new EffectResponse(req.ID, EffectStatus.Retry, "");
        }

        try
        {
            var player = list[UnityEngine.Random.Range(0, list.Count)];

            {
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        playerRef.TeleportPlayer(player.transform.position);

                    });
                }
            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse InverseTeleport(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        PlayerControllerB playerRef = StartOfRound.Instance.localPlayerController;

        try
        {
            if(StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled || playerRef.isInsideFactory)
            {
                status = EffectStatus.Retry;
            }
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    var randomSeed = new System.Random(StartOfRound.Instance.randomMapSeed);
                    Vector3 pos1 = RoundManager.Instance.insideAINodes[randomSeed.Next(0, RoundManager.Instance.insideAINodes.Length)].transform.position;
                    Vector3 pos2 = RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(pos1,randomSeed:randomSeed);

                    playerRef.TeleportPlayer(pos2);
                });
            }
        }
        catch(Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }
        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse TeleportCrewTo(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        List<PlayerControllerB> list = new List<PlayerControllerB>();

        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
        {
            if (player != null && !player.isPlayerDead && player != playerRef && player.isActiveAndEnabled && player.isPlayerControlled)
                list.Add(player);
        }

        if (list.Count <= 0)
        {
            return new EffectResponse(req.ID, EffectStatus.Retry, "");
        }

        try
        {
            var player = list[UnityEngine.Random.Range(0, list.Count)];

            {
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_tele_{(int)player.playerClientId}_{(int)playerRef.playerClientId}</size>");

                    });
                }
            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse SpawnBody(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        try
        {

            if (GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom) status = EffectStatus.Retry;
            else
            {
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {

                        //playerRef.SpawnDeadBody((int)playerRef.playerClientId, playerRef.transform.up * 2.0f + playerRef.transform.forward * 5.0f, 0, playerRef);

                        msgid++;
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_body_{(int)playerRef.playerClientId}_{msgid}</size>");

                    });
                }
            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse SpawnCrewBody(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        List<PlayerControllerB> list = new List<PlayerControllerB>();

        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
        {
            if (player != null && !player.isPlayerDead && !player.IsServer && player.isActiveAndEnabled && player.isPlayerControlled)
                list.Add(player);
        }

        if (list.Count <= 0)
        {
            return new EffectResponse(req.ID, EffectStatus.Retry, "Could not find Active player to Spawn Crew Body");
        }

        try
        {
            var player = list[UnityEngine.Random.Range(0, StartOfRound.Instance.connectedPlayersAmount)]; //test fixing Crew bodies?

            if (player.isInHangarShipRoom) status = EffectStatus.Retry;
            else
            {
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {

                        msgid++;
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_body_{(int)player.playerClientId}_{msgid}</size>");

                    });
                }
            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse Launch(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    int layerMask = ~LayerMask.GetMask(new string[]
                    {
                        "Room"
                    });
                    layerMask = ~LayerMask.GetMask(new string[]
                    {
                        "Colliders"
                    });

                    var pos = playerRef.transform.position - playerRef.transform.up * 5.0f;

                    var array = Physics.OverlapSphere(pos, 10f, layerMask);
                    for (int j = 0; j < array.Length; j++)
                    {
                        Rigidbody component2 = array[j].GetComponent<Rigidbody>();
                        if (component2 != null)
                        {
                            component2.AddExplosionForce(70f, pos, 10f);
                        }
                    }

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    #endregion

    #region Enemies

    public static EffectResponse Spawn(ControlClient client, EffectRequest req)
    {
        var playerRef = StartOfRound.Instance.localPlayerController;
        SpawnableEnemyWithRarity? enemyRef = null;
        if (playerRef.isPlayerDead) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is dead");

        string[] enteredText = req.code.Split('_');
        if (enteredText.Length == 2)
        {

        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }
        bool found = false;
        if (enteredText[1] == "mimic")
        {
            GameObject prefab = null;

            foreach (var level in StartOfRound.Instance.levels)
            {
                if (prefab == null)
                    foreach (var spawn in level.spawnableScrap)
                    {
                        if (spawn.spawnableItem.name.ToLower() == "tragedymask")
                        {
                            prefab = spawn.spawnableItem.spawnPrefab;
                            found = true;
                        }
                    }
            }

            if (prefab == null)
                return new EffectResponse(req.ID, EffectStatus.Retry);

        }
        if (enteredText[1] == "landmine")
        {
            if (playerRef.isInElevator) return new EffectResponse(req.ID, EffectStatus.Failure, "Player is inside ship");
            found = true;
        }
        if (!found)
            foreach (var Enemy in StartOfRound.Instance.currentLevel.Enemies)
            {


                if (Enemy.enemyType.enemyName.ToLower().Contains(enteredText[1]))
                {
                    try
                    {
                        if (!playerRef.isInsideFactory) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is outside");
                        if (enteredText[1] == "jester" && !playerRef.isInsideFactory || enteredText[1] == "butler" && !playerRef.isInsideFactory || enteredText[1] == "cracker" && !playerRef.isInsideFactory) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is Outside Building");
                    }
                    catch (Exception e)
                    {
                    }
                }
                found = true;
            }

        if (!found)
            foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.OutsideEnemies)
            {


                if (outsideEnemy.enemyType.enemyName.ToLower().Contains(enteredText[1]))
                {
                    found = true;
                    if (enteredText[1] == "giant" || enteredText[1] == "levi" || enteredText[1] == "radmech")
                    {
                        try
                        {
                            if (playerRef.isInsideFactory) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is outside");

                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }

        if (found == false) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is outside");

        if (Mod.isHost)
        {

            Mod.ActionQueue.Enqueue(() =>
            {

                if (enteredText[1] == "mimic")
                {
                    GameObject prefab = null;

                    foreach (var level in StartOfRound.Instance.levels)
                    {
                        if (prefab == null)
                            foreach (var spawn in level.spawnableScrap)
                            {
                                if (spawn.spawnableItem.name.ToLower() == "tragedymask") prefab = spawn.spawnableItem.spawnPrefab;
                            }
                    }

                    if (prefab == null)
                        return;

                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, playerRef.transform.position, Quaternion.identity, Mod.currentStart.propsContainer);
                    HauntedMaskItem component = gameObject.GetComponent<HauntedMaskItem>();

                    GameObject obj = UnityEngine.Object.Instantiate(component.mimicEnemy.enemyPrefab, playerRef.transform.position + playerRef.transform.forward * 5.0f, Quaternion.Euler(Vector3.zero));
                    obj.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);


                    MaskedPlayerEnemy enemy = obj.GetComponent<MaskedPlayerEnemy>();
                    enemy.mimickingPlayer = playerRef;
                    enemy.SetSuit(playerRef.currentSuitID);
                    enemy.SetEnemyOutside(!playerRef.isInsideFactory);
                    enemy.SetVisibilityOfMaskedEnemy();
                    enemy.SetMaskType(component.maskTypeId);

                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_mimic_{enemy.NetworkObject.NetworkObjectId}</size>");

                    obj.gameObject.GetComponentInChildren<EnemyAI>().stunNormalizedTimer = 6.0f;
                    UnityEngine.Object.Destroy(gameObject);
                    return;
                }
                if (enteredText[1] == "landmine")
                {
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_landmine_{(int)playerRef.playerClientId}</size>");

                    return;
                }
                foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.OutsideEnemies)
                {

                    if (outsideEnemy.enemyType.enemyName.ToLower().Contains(enteredText[1]))
                    {

                        try
                        {
                            Mod.SpawnEnemy(outsideEnemy, 1, false);

                        }
                        catch (Exception e)
                        {
                            Mod.mls.LogWarning("Monster is Not Present Outside on Moon, Aborting");

                        }
                        return;
                    }
                }
                foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.Enemies)
                {
                    if (outsideEnemy.enemyType.enemyName.ToLower().Contains(enteredText[1]))
                    {
                        try
                        {
                            Mod.SpawnEnemy(outsideEnemy, 1, true);

                        }
                        catch (Exception e)
                        {
                            Mod.mls.LogWarning("Monster is Not Present Inside on Moon, Aborting");
                        }
                        return;
                    }
                }
            });
        }
        else
        {
            try
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_cspawn_{enteredText[1]}_{(int)playerRef.playerClientId}</size>");
                });
            }
            catch (Exception e)
            {
                Mod.mls.LogError(e.ToString());
            }
        }


        return new EffectResponse(req.ID, EffectStatus.Success);
    }

    public static EffectResponse CrewSpawn(ControlClient client, EffectRequest req)
    {
        var playerRef = StartOfRound.Instance.localPlayerController;
        SpawnableEnemyWithRarity? enemyRef = null;

        string[] enteredText = req.code.Split('_');
        if (enteredText.Length == 2)
        {

        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }

        List<PlayerControllerB> list = new List<PlayerControllerB>();


        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
        {
            if (player != null && !player.isPlayerDead && player != playerRef && player.isActiveAndEnabled && player.isPlayerControlled)
                list.Add(player);
        }

        if (list.Count <= 0)
        {
            return new EffectResponse(req.ID, EffectStatus.Retry, "");
        }

        {
            PlayerControllerB player;
            player = list[UnityEngine.Random.Range(0, list.Count)];

            bool found = false;

            if (enteredText[1] == "landmine")
            {
                if (player.isInElevator) return new EffectResponse(req.ID, EffectStatus.Failure, "Player is inside ship");
                found = true;
            }

            if (!found)
                foreach (var Enemy in StartOfRound.Instance.currentLevel.Enemies)
                {


                    if (Enemy.enemyType.enemyName.ToLower().Contains(enteredText[1]))
                    {
                        try
                        {
                            if (!player.isInsideFactory) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is outside");
                            if (enteredText[1] == "jester" && !player.isInsideFactory || enteredText[1] == "butler" && !player.isInsideFactory || enteredText[1] == "cracker" && !player.isInsideFactory) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is Outside Building");

                        }
                        catch (Exception e)
                        {

                        }
                    }
                    found = true;
                }

            if (!found)
                foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.OutsideEnemies)
                {


                    if (outsideEnemy.enemyType.enemyName.ToLower().Contains(enteredText[1]))
                    {
                        found = true;
                        Mod.mls.LogInfo("Found Monster: " + enteredText[1]);
                        if (enteredText[1] == "giant" || enteredText[1] == "levi" || enteredText[1] == "radmech" || enteredText[1].ToLower().Contains("bush"))
                        {
                            try
                            {
                                if (player.isInsideFactory) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is outside");

                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }

            if (!found) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is outside");

            if (Mod.isHost)
            {

                Mod.ActionQueue.Enqueue(() =>
                {
                    foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.OutsideEnemies)
                    {

                        if (enteredText[1] == "mimic")
                        {
                            GameObject prefab = null;

                            foreach (var level in StartOfRound.Instance.levels)
                            {
                                if (prefab == null)
                                    foreach (var spawn in level.spawnableScrap)
                                    {
                                        if (spawn.spawnableItem.name.ToLower() == "tragedymask") prefab = spawn.spawnableItem.spawnPrefab;
                                    }
                            }

                            if (prefab == null)
                                return;

                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, player.transform.position, Quaternion.identity, Mod.currentStart.propsContainer);
                            HauntedMaskItem component = gameObject.GetComponent<HauntedMaskItem>();

                            GameObject obj = UnityEngine.Object.Instantiate(component.mimicEnemy.enemyPrefab, player.transform.position + player.transform.forward * 5.0f, Quaternion.Euler(Vector3.zero));
                            obj.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);


                            MaskedPlayerEnemy enemy = obj.GetComponent<MaskedPlayerEnemy>();
                            enemy.SetSuit(player.currentSuitID);
                            enemy.SetEnemyOutside(!player.isInsideFactory);
                            enemy.SetVisibilityOfMaskedEnemy();
                            enemy.SetMaskType(component.maskTypeId);

                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_mimic_{enemy.NetworkObject.NetworkObjectId}</size>");


                            obj.gameObject.GetComponentInChildren<EnemyAI>().stunNormalizedTimer = 6.0f;
                            UnityEngine.Object.Destroy(gameObject);
                            return;
                        }


                        if (enteredText[1] == "landmine")
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_landmine_{(int)player.playerClientId}</size>");

                            return;
                        }


                        if (outsideEnemy.enemyType.enemyName.ToLower().Contains(enteredText[1]))
                        {
                            try
                            {
                                Mod.SpawnCrewEnemy(player, outsideEnemy, 1, false);

                            }
                            catch (Exception e)
                            {

                            }
                            return;
                        }
                    }
                    foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.Enemies)
                    {

                        if (outsideEnemy.enemyType.enemyName.ToLower().Contains(enteredText[1]))
                        {
                            try
                            {
                                Mod.SpawnCrewEnemy(player, outsideEnemy, 1, false);

                            }
                            catch (Exception e)
                            {

                            }
                            return;
                        }
                    }

                });
            }
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_cspawn_{enteredText[1]}_{(int)player.playerClientId}</size>");
                });
            }
        }

        return new EffectResponse(req.ID, EffectStatus.Success);
    }

    #endregion

    #region Items
    public static EffectResponse TakeItem(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled || playerRef.currentlyHeldObjectServer == null) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    playerRef.DespawnHeldObject();
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse TakeCrewItem(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        List<PlayerControllerB> list = new List<PlayerControllerB>();

        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
        {
            if (player != null && !player.isPlayerDead && player != playerRef && player.isActiveAndEnabled && player.isPlayerControlled)
                list.Add(player);
        }

        if (list.Count <= 0)
        {
            return new EffectResponse(req.ID, EffectStatus.Retry, "");
        }

        try
        {
            var player = list[UnityEngine.Random.Range(0, list.Count)];

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !player.playersManager.shipDoorsEnabled || player.currentlyHeldObjectServer == null) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_take_{(int)player.playerClientId}</size>");
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse DropItem(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled || playerRef.currentlyHeldObjectServer == null) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    playerRef.DiscardHeldObject();
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse UnchargeItem(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled || playerRef.currentlyHeldObjectServer == null) status = EffectStatus.Retry;
            else if (!playerRef.currentlyHeldObjectServer.itemProperties.requiresBattery || playerRef.currentlyHeldObjectServer.insertedBattery == null || playerRef.currentlyHeldObjectServer.insertedBattery.charge == 0) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    playerRef.currentlyHeldObjectServer.insertedBattery.charge = 0;
                    playerRef.currentlyHeldObjectServer.SyncBatteryServerRpc((int)playerRef.currentlyHeldObjectServer.insertedBattery.charge);
                    playerRef.currentlyHeldObjectServer.UseUpBatteries();
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse ChargeItem(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled || playerRef.currentlyHeldObjectServer == null) status = EffectStatus.Retry;
            else if (!playerRef.currentlyHeldObjectServer.itemProperties.requiresBattery || playerRef.currentlyHeldObjectServer.insertedBattery == null || playerRef.currentlyHeldObjectServer.insertedBattery.charge >= 0.96f) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    playerRef.currentlyHeldObjectServer.insertedBattery.charge = 1.0f;
                    playerRef.currentlyHeldObjectServer.SyncBatteryServerRpc((int)playerRef.currentlyHeldObjectServer.insertedBattery.charge);
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse GiveItem(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        int give = 0;

        string[] enteredText = req.code.Split('_');
        if (enteredText.Length == 2)
        {
            string item = enteredText[1];
            try
            {
                Item Requested = StartOfRound.Instance.allItemsList.itemsList.Find(z => z.name.ToLower().Equals(item.ToLower()));//Lethal Level Loader patch. Search for item name instead, since it removes items from the list.
                give = StartOfRound.Instance.allItemsList.itemsList.IndexOf(Requested);
            }
            catch (IndexOutOfRangeException)
            {
                return new EffectResponse(req.ID, EffectStatus.Failure);
            }

        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }
        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            var slot = (int)callAndReturnFunc(playerRef, "FirstEmptyItemSlot", null);

            //if (StartOfRound.Instance.allItemsList.itemsList[2].itemName.ToLower() != "box" && give >= 2) //Lethal Level Loader Patch
            //{
            // give--;
            // }
            if (playerRef.inSpecialInteractAnimation || slot == -1 || givedelay > 0)
            {
                return new EffectResponse(req.ID, EffectStatus.Retry, "");
            }

            if (!Mod.isHost)
            {
                givedelay = 20;
                Mod.ActionQueue.Enqueue(() =>
                {
                    msgid++;
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_giver_{give}_{(int)playerRef.playerClientId}_{msgid}</size>");
                });
                return new EffectResponse(req.ID, status, message);
            }

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                givedelay = 20;
                Mod.ActionQueue.Enqueue(() =>
                {
                    Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(StartOfRound.Instance.allItemsList.itemsList[give].spawnPrefab, playerRef.transform.position, Quaternion.identity, Mod.currentStart.propsContainer);
                    gameObject.GetComponent<GrabbableObject>().fallTime = 0f;
                    int Value = UnityEngine.Random.Range(0, 100);
                    gameObject.GetComponent<GrabbableObject>().scrapValue = Value;
                    gameObject.GetComponent <GrabbableObject>().SetScrapValue(Value);
                    gameObject.GetComponent<NetworkObject>().Spawn(false);

                    var grab = gameObject.GetComponent<GrabbableObject>();

                    setProperty(playerRef, "currentlyGrabbingObject", grab);
                    setProperty(playerRef, "grabInvalidated", false);

                    NetworkObject networkObject = grab.NetworkObject;
                    if (networkObject == null || !networkObject.IsSpawned)
                    {
                        return;
                    }   
                    grab.InteractItem();

                    playerRef.playerBodyAnimator.SetBool("GrabInvalidated", false);
                    playerRef.playerBodyAnimator.SetBool("GrabValidated", false);
                    playerRef.playerBodyAnimator.SetBool("cancelHolding", false);
                    playerRef.playerBodyAnimator.ResetTrigger("Throw");

                    callFunc(playerRef, "SetSpecialGrabAnimationBool", new System.Object[] { true, null });

                    playerRef.isGrabbingObjectAnimation = true;
                    playerRef.cursorIcon.enabled = false;
                    playerRef.cursorTip.text = "";
                    playerRef.twoHanded = grab.itemProperties.twoHanded;
                    playerRef.carryWeight += Mathf.Clamp(grab.itemProperties.weight - 1f, 0f, 10f);
                    if (grab.itemProperties.grabAnimationTime > 0f)
                    {
                        playerRef.grabObjectAnimationTime = grab.itemProperties.grabAnimationTime;
                    }
                    else
                    {
                        playerRef.grabObjectAnimationTime = 0.4f;
                    }

                    callFunc(playerRef, "GrabObjectServerRpc", new NetworkObjectReference(networkObject));

                    Coroutine goc = (Coroutine)getProperty(playerRef, "grabObjectCoroutine");

                    if (goc != null)
                    {
                        ((MonoBehaviour)playerRef).StopCoroutine(goc);
                    }

                    setProperty(playerRef, "grabObjectCoroutine", ((UnityEngine.MonoBehaviour)playerRef).StartCoroutine("GrabObject"));
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse GiveCrewItem(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        int give = 0;
        var playerRef = StartOfRound.Instance.localPlayerController;

        string[] enteredText = req.code.Split('_');
        if (enteredText.Length == 2)
        {
            string item = enteredText[1];
            try
            {
                Item Requested = StartOfRound.Instance.allItemsList.itemsList.Find(z => z.name.ToLower().Equals(item.ToLower()));//Lethal Level Loader patch. Search for item name instead, since it removes items from the list.
                give = StartOfRound.Instance.allItemsList.itemsList.IndexOf(Requested);
            }
            catch (IndexOutOfRangeException)
            {
                return new EffectResponse(req.ID, EffectStatus.Failure);
            }
        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }


        List<PlayerControllerB> list = new List<PlayerControllerB>();

        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
        {
            if (player != null && !player.isPlayerDead && player != playerRef && player.isActiveAndEnabled && player.isPlayerControlled && !playerRef.isGrabbingObjectAnimation)
            {
                list.Add(player);
            }
        }

        if (list.Count <= 0)
        {
            return new EffectResponse(req.ID, EffectStatus.Retry, "");
        }


        try
        {
            PlayerControllerB player;

            player = list[UnityEngine.Random.Range(0, list.Count)];

            if (!Mod.isHost)
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    msgid++;
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_giver_{give}_{(int)player.playerClientId}_{msgid}</size>");
                });
                return new EffectResponse(req.ID, status, message);
            }

            var slot = (int)callAndReturnFunc(player, "FirstEmptyItemSlot", null);

            if (player.inSpecialInteractAnimation || slot == -1)
            {
                return new EffectResponse(req.ID, EffectStatus.Retry, "");
            }

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(StartOfRound.Instance.allItemsList.itemsList[give].spawnPrefab, player.transform.position, Quaternion.identity, Mod.currentStart.propsContainer);
                    gameObject.GetComponent<GrabbableObject>().fallTime = 0f; 
                    int Value = UnityEngine.Random.Range(0, 100);
                    gameObject.GetComponent<GrabbableObject>().scrapValue = Value;
                    gameObject.GetComponent<GrabbableObject>().SetScrapValue(Value);
                    gameObject.GetComponent<NetworkObject>().Spawn(false);

                    msgid++;
                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_give_{give}_{(int)player.playerClientId}_{gameObject.GetComponent<NetworkObject>().NetworkObjectId}_{msgid}</size>");

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse BuyItem(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        int give = 0;
        var playerRef = StartOfRound.Instance.localPlayerController;

        string[] enteredText = req.code.Split('_');
        if (enteredText.Length == 2)
        {
            string item = enteredText[1];

            if (Enum.TryParse(item, out BuyableItemList itemNumber))
            {
                give = (int)itemNumber;
            }
            else
            {
                return new EffectResponse(req.ID, EffectStatus.Failure);
            }
        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }

        try
        {


            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

                    int[] a = { give };



                    terminal.BuyItemsServerRpc(a, terminal.groupCredits, 0); // a = terminal.buyableItemsList[give].itemId

                    HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " sent a pod with a " + terminal.buyableItemsList[give].name);
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    #endregion

    #region VehicleStuff

    public static EffectResponse BuyCruiser(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        int give = 0;
        var playerRef = StartOfRound.Instance.localPlayerController;


        Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
        var veh = terminal.buyableVehicles[0];
        string[] enteredText = req.code.Split('_');
        if (enteredText.Length == 2)
        {
        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }

        try
        {


            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    terminal.BuyVehicleServerRpc(0, terminal.groupCredits, false);
                    HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " sent a pod with a " + veh.vehicleDisplayName);
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }


    public static EffectResponse TurnOnVehicle(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;
        string[] enteredText = req.code.Split('_');
        bool found = false;
        VehicleController Veh1 = UnityEngine.Object.FindObjectOfType<VehicleController>();
        if (enteredText.Length == 2)
        {
        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }
        if (!found)
        {
            try
            {
                if (Veh1.currentDriver != playerRef) status = EffectStatus.Retry;
                else found = true;
            }
            catch (Exception e) { status = EffectStatus.Retry; }
        }
        if (found)
        {
            try
            {
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        if (Veh1)
                        {
                            Veh1.carHP = 12;
                            Veh1.ignitionStarted = true;
                            Veh1.StartIgnitionServerRpc(1);
                        }
                        else
                        {

                        }
                    });
                }
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }
        }
        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse SpringChair(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;
        bool found = false;
        string[] enteredText = req.code.Split('_');
        VehicleController Veh1 = UnityEngine.Object.FindObjectOfType<VehicleController>();
        if (enteredText.Length == 2)
        {
        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }
        if (!found)
        {
            try
            {
                if (Veh1.currentDriver != playerRef) status = EffectStatus.Retry;
                else found = true;
            }
            catch (Exception e) { status = EffectStatus.Retry; }
        }
        if (found)
        {
            try
            {
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        if (Veh1)
                        {
                            Veh1.SpringDriverSeatServerRpc();
                        }
                        else
                        {

                        }
                    });
                }
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }
        }
        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse TurnOffEngine(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;
        bool found = false;
        string[] enteredText = req.code.Split('_');
        VehicleController Veh1 = UnityEngine.Object.FindObjectOfType<VehicleController>();
        if (enteredText.Length == 3)
        {
        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }
        if (!found)
        {
            try
            {
                if (Veh1.currentDriver != playerRef) status = EffectStatus.Retry;
                else found = true;
            }
            catch (Exception e) { status = EffectStatus.Retry; }
        }
        if (found)
        {
            try
            {
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        if (Veh1)
                        {
                            Veh1.RemoveKeyFromIgnition();
                        }
                        else
                        {

                        }
                    });
                }
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }
        }
        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse DestroyVehicle(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;
        bool found = false;
        string[] enteredText = req.code.Split('_');
        VehicleController Veh1 = UnityEngine.Object.FindObjectOfType<VehicleController>();
        if (enteredText.Length == 2)
        {
        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }
        if (!found)
        {
            try
            {
                if (Veh1.currentDriver == playerRef) status = EffectStatus.Retry;
                else found = true;
            }
            catch (Exception e) { status = EffectStatus.Retry; }
        }
        if (found)
        {
            try
            {
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        if (Veh1)
                        {
                            Veh1.DestroyCarServerRpc(1);
                        }
                        else
                        {

                        }
                    });
                }
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }
        }
        return new EffectResponse(req.ID, status, message);
    }

    #endregion

    #region Weather
    public static EffectResponse Weather(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        int give = 0;

        string[] enteredText = req.code.Split('_');
        if (enteredText.Length == 2)
        {
            give = int.Parse(enteredText[1]);
        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }

        if (StartOfRound.Instance.currentLevel.currentWeather == (LevelWeatherType)give)
        {
            return new EffectResponse(req.ID, EffectStatus.Retry);
        }

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    TimeOfDay.Instance.DisableAllWeather(true);

                    WeatherEffect wo;

                    try
                    {
                        if ((int)StartOfRound.Instance.currentLevel.currentWeather >= 0)
                        {
                            wo = TimeOfDay.Instance.effects[(int)StartOfRound.Instance.currentLevel.currentWeather];
                            if (wo != null)
                            {
                                wo.effectEnabled = false;
                                if (wo.effectPermanentObject != null)
                                {
                                    wo.effectPermanentObject.SetActive(false);
                                }
                                if (wo.effectObject != null)
                                {
                                    wo.effectObject.SetActive(false);
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }
                    StartOfRound.Instance.currentLevel.currentWeather = (LevelWeatherType)give;

                    //TestMod.mls.LogInfo("weather changed");

                    try
                    {
                        if ((int)StartOfRound.Instance.currentLevel.currentWeather >= 0)
                        {
                            wo = TimeOfDay.Instance.effects[give];
                            if (wo != null)
                            {
                                wo.effectEnabled = true;
                                if (wo.effectPermanentObject != null)
                                {
                                    wo.effectPermanentObject.SetActive(true);
                                }
                                if (wo.effectObject != null)
                                {
                                    wo.effectObject.SetActive(true);
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }
                    //callFunc(RoundManager.Instance, "SetToCurrentWeatherLevel", null);

                    //TestMod.mls.LogInfo($"round manager: {RoundManager.Instance}");
                    //TestMod.mls.LogInfo($"tod: {TimeOfDay.Instance}");

                    try
                    {
                        TimeOfDay.Instance.currentLevelWeather = RoundManager.Instance.currentLevel.currentWeather;
                        if (TimeOfDay.Instance.currentLevelWeather != LevelWeatherType.None && RoundManager.Instance.currentLevel.randomWeathers != null)
                        {
                            for (int i = 0; i < RoundManager.Instance.currentLevel.randomWeathers.Length; i++)
                            {
                                if (RoundManager.Instance.currentLevel.randomWeathers[i].weatherType == RoundManager.Instance.currentLevel.currentWeather)
                                {
                                    TimeOfDay.Instance.currentWeatherVariable = (float)RoundManager.Instance.currentLevel.randomWeathers[i].weatherVariable;
                                    TimeOfDay.Instance.currentWeatherVariable2 = (float)RoundManager.Instance.currentLevel.randomWeathers[i].weatherVariable2;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Mod.mls.LogError(ex.ToString());
                    }

                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_weather_{(int)give}</size>");

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    #endregion

    #region Scrap_Money
    public static EffectResponse AddCredits(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";


        int give = 0;

        string[] enteredText = req.code.Split('_');
        if (enteredText.Length == 2)
        {
            give = int.Parse(enteredText[1]);
        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }

        Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            if (give > 0 && terminal.groupCredits + give > 99999) return new EffectResponse(req.ID, EffectStatus.Retry);
            if (give < 0 && terminal.groupCredits + give < 0) return new EffectResponse(req.ID, EffectStatus.Retry);

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                if (Mod.isHost)
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        terminal.groupCredits += give;
                        terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);

                        if (give > 0)
                            HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " gave " + give + " credits");
                        else
                            HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " took " + (-1 * give) + " credits");

                    });
                }
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_credits_{give}</size>");
                    });
                }
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse AddQuota(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";


        int give = 0;

        string[] enteredText = req.code.Split('_');
        if (enteredText.Length == 2)
        {
            give = int.Parse(enteredText[1]);
        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }

        Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            var tod = TimeOfDay.Instance;
            if (give > 0 && tod.profitQuota + give > 25000) return new EffectResponse(req.ID, EffectStatus.Retry);
            if (give < 0 && tod.profitQuota + give < 5) return new EffectResponse(req.ID, EffectStatus.Retry);

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    tod.profitQuota += give;
                    if (give > 0)
                        HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " raised the quota by " + give);
                    else
                        HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " lowered the quota by " + give);

                    int val = tod.quotaFulfilled;

                    //DepositItemsDesk desk = UnityEngine.Object.FindObjectOfType<DepositItemsDesk>();

                    //tod.SyncNewProfitQuotaClientRpc(tod.profitQuota, 0, tod.quotaFulfilled);

                    //if (desk != null)
                    //desk.SellItemsClientRpc(val, terminal.groupCredits, desk.itemsOnCounterAmount, StartOfRound.Instance.companyBuyingRate);
                    //else
                    tod.quotaFulfilled = val;

                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_quota_{val}_{tod.profitQuota}</size>");


                    StartOfRound.Instance.profitQuotaMonitorText.text = string.Format("PROFIT QUOTA:\n${0} / ${1}", val, tod.profitQuota);

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse AddProfit(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";


        int give = 0;

        string[] enteredText = req.code.Split('_');
        if (enteredText.Length == 2)
        {
            give = int.Parse(enteredText[1]);
        }
        else
        {
            return new EffectResponse(req.ID, EffectStatus.Failure);
        }



        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            var tod = TimeOfDay.Instance;

            //TestMod.mls.LogInfo($"playerref: {playerRef}");
            //TestMod.mls.LogInfo($"TimeOfDay: {tod}");

            if (give > 0 && tod.quotaFulfilled >= tod.profitQuota) return new EffectResponse(req.ID, EffectStatus.Retry);
            if (give < 0 && tod.quotaFulfilled <= 0) return new EffectResponse(req.ID, EffectStatus.Retry);



            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    //DepositItemsDesk desk = UnityEngine.Object.FindObjectOfType<DepositItemsDesk>();
                    Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

                    int givep = tod.profitQuota * give / 100;

                    int val = tod.quotaFulfilled;

                    val += givep;

                    if (give > 0 && val > tod.profitQuota) val = tod.profitQuota;
                    if (give < 0 && val < 0) val = 0;

                    givep = val - tod.quotaFulfilled;

                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_quota_{val}_{tod.profitQuota}</size>");

                    //if (desk != null)
                    //{

                    //desk.SellItemsClientRpc(givep, terminal.groupCredits, desk.itemsOnCounterAmount, StartOfRound.Instance.companyBuyingRate);
                    //callFunc(desk, "SellAndDisplayItemProfits", new object[] { givep, terminal.groupCredits });

                    //}
                    //else
                    {
                        tod.quotaFulfilled = val;

                        if (give > 0)
                            HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " filled the quota by " + give + "%");
                        else
                            HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " emptied the quota by " + give + "%");

                    }
                    StartOfRound.Instance.profitQuotaMonitorText.text = string.Format("PROFIT QUOTA:\n${0} / ${1}", tod.quotaFulfilled, tod.profitQuota);

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse AddScrap(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;


        try
        {


            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    RoundManager.Instance.SpawnScrapInLevel();
                    HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " spawned more scrap in the level");
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    #endregion

    #region Time
    public static EffectResponse AddHour(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            var tod = TimeOfDay.Instance;

            if (!StartOfRound.Instance.currentLevel.planetHasTime || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                if (Mod.isHost)
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        float num = tod.lengthOfHours;
                        tod.globalTime += num;
                        tod.timeUntilDeadline -= num;
                        callFunc(tod, "MoveTimeOfDay", null);
                    });
                }
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_addhour</size>");
                    });
                }


            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse RemoveHour(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            var tod = TimeOfDay.Instance;

            if (!StartOfRound.Instance.currentLevel.planetHasTime || tod.globalTime <= tod.lengthOfHours || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                if (Mod.isHost)
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        float num = tod.lengthOfHours;
                        tod.globalTime -= num;
                        tod.timeUntilDeadline += num;
                        callFunc(tod, "MoveTimeOfDay", null);
                    });
                }
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_remhour</size>");
                    });
                }

            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse AddDay(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            var tod = TimeOfDay.Instance;

            if (!StartOfRound.Instance.currentLevel.planetHasTime || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    TimeOfDay.Instance.timeUntilDeadline += TimeOfDay.Instance.totalTime;
                    TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
                    HUDManager.Instance.DisplayDaysLeft((int)Mathf.Floor(TimeOfDay.Instance.timeUntilDeadline / TimeOfDay.Instance.totalTime));
                    UnityEngine.Object.FindObjectOfType<Terminal>().SetItemSales();

                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_deadline_{TimeOfDay.Instance.timeUntilDeadline}</size>");

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse RemoveDay(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            var tod = TimeOfDay.Instance;

            if (TimeOfDay.Instance.timeUntilDeadline <= TimeOfDay.Instance.totalTime || !StartOfRound.Instance.currentLevel.planetHasTime || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    TimeOfDay.Instance.timeUntilDeadline -= TimeOfDay.Instance.totalTime;
                    TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
                    HUDManager.Instance.DisplayDaysLeft((int)Mathf.Floor(TimeOfDay.Instance.timeUntilDeadline / TimeOfDay.Instance.totalTime));
                    UnityEngine.Object.FindObjectOfType<Terminal>().SetItemSales();

                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_deadline_{TimeOfDay.Instance.timeUntilDeadline}</size>");

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    #endregion

    #region Ship
    public static EffectResponse CloseDoors(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            var tod = TimeOfDay.Instance;

            if (StartOfRound.Instance.hangarDoorsClosed || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    StartOfRound.Instance.SetDoorsClosedServerRpc(true);
                    HangarShipDoor hangarShipDoor = UnityEngine.Object.FindObjectOfType<HangarShipDoor>();
                    hangarShipDoor.PlayDoorAnimation(true);

                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_doors_1</size>");
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse OpenDoors(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            var tod = TimeOfDay.Instance;

            if (!StartOfRound.Instance.hangarDoorsClosed || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    StartOfRound.Instance.SetDoorsClosedServerRpc(false);

                    HangarShipDoor hangarShipDoor = UnityEngine.Object.FindObjectOfType<HangarShipDoor>();
                    hangarShipDoor.PlayDoorAnimation(false);

                    HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_doors_0</size>");

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse ShipLeave(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            var tod = TimeOfDay.Instance;

            if (TimeOfDay.Instance.votedShipToLeaveEarlyThisRound || !StartOfRound.Instance.currentLevel.planetHasTime || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    TimeOfDay.Instance.votesForShipToLeaveEarly = 998;
                    TimeOfDay.Instance.SetShipLeaveEarlyServerRpc();
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    #endregion

    #region Misc
    public static EffectResponse BreakersOn(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            BreakerBox breakerBox = UnityEngine.Object.FindObjectOfType<BreakerBox>();

            if (breakerBox == null || breakerBox.isPowerOn || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                if (Mod.isHost)
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        RoundManager.Instance.PowerSwitchOnClientRpc();
                    });
                }
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_poweron</size>");
                    });
                }
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse KillEnemies(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            EnemyAI[] array = UnityEngine.Object.FindObjectsByType<EnemyAI>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            bool found = false;
            for (int i = 0; i < array.Length; i++)
            {
                if (Vector3.Distance(array[i].transform.position, playerRef.transform.position) < 24f && !array[i].isEnemyDead && array[i].stunNormalizedTimer <= 0)
                    found = true;
            }

            if (!found || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (Vector3.Distance(array[i].transform.position, playerRef.transform.position) < 24f && !array[i].isEnemyDead && array[i].stunNormalizedTimer <= 0)
                        {
                            array[i].KillEnemy(false);
                            array[i].KillEnemyServerRpc(false);
                        }
                    }
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse BreakersOff(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            BreakerBox breakerBox = UnityEngine.Object.FindObjectOfType<BreakerBox>();

            if (breakerBox == null || !breakerBox.isPowerOn || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                if (Mod.isHost)
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        RoundManager.Instance.PowerSwitchOffClientRpc();
                    });
                }
                else
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_poweroff</size>");
                    });
                }
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    #endregion
    public static EffectResponse Lightning(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            var tod = TimeOfDay.Instance;

            if (!StartOfRound.Instance.currentLevel.planetHasTime || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    Vector3 pos = playerRef.transform.forward * 5.0f;
                    int r = UnityEngine.Random.Range(-30, 30);
                    pos = Quaternion.Euler(0, r, 0) * pos;

                    pos += playerRef.transform.position;

                    RoundManager.Instance.LightningStrikeServerRpc(pos);
                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse PlayHorn(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";


        try
        {
            var playerRef = StartOfRound.Instance.localPlayerController;

            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    GameObject gameObject = new GameObject();
                    NoisemakerProp prop = gameObject.AddComponent<NoisemakerProp>();

                    playerRef.movementAudio.PlayOneShot(prop.noiseSFX[0], 4.0f);
                    WalkieTalkie.TransmitOneShotAudio(playerRef.movementAudio, prop.noiseSFX[0], 4.0f);
                    RoundManager.Instance.PlayAudibleNoise(playerRef.transform.position, 12f, 4.0f, 0, false, 0);

                    UnityEngine.Object.Destroy(gameObject);

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static void SpawnAnMakeWebs(SpawnableEnemyWithRarity source)
    {
        //TestMod.mls.LogInfo($"Creating Webs");

        var playerRef = StartOfRound.Instance.localPlayerController;

        GameObject obj = UnityEngine.Object.Instantiate(source.enemyType.enemyPrefab, playerRef.transform.position + playerRef.transform.forward * 5.0f, Quaternion.Euler(Vector3.zero));
        //obj.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);

        obj.gameObject.GetComponentInChildren<EnemyAI>().stunNormalizedTimer = 6.0f;

        SandSpiderAI bird = obj.gameObject.GetComponent<SandSpiderAI>();


        Vector3 pos = playerRef.transform.forward * 10.0f;
        pos = Quaternion.Euler(0, -45, 0) * pos;
        pos += playerRef.transform.position;

        Vector3 pos2 = playerRef.transform.forward * 10.0f;
        pos2 = Quaternion.Euler(0, 45, 0) * pos2;
        pos2 += playerRef.transform.position;

        bird.SpawnWebTrapServerRpc(pos, pos2);

        UnityEngine.Object.Destroy(obj);
    }

    public static EffectResponse CreateWebs(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        //ISearchList results = UnityEngine.SearchService.Request($"p: t:prefab t:{typeof(SandSpiderAI).Name}", SearchFlags.Synchronous);
        //foreach (var result in results)

        try
        {


            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    foreach (var level in StartOfRound.Instance.levels)
                    {
                        foreach (var outsideEnemy in level.OutsideEnemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("sand"))
                            {
                                try
                                {
                                    SpawnAnMakeWebs(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                        foreach (var outsideEnemy in level.Enemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("sand"))
                            {
                                try
                                {
                                    SpawnAnMakeWebs(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                        foreach (var outsideEnemy in level.DaytimeEnemies)
                        {

                            if (outsideEnemy.enemyType.enemyName.ToLower().Contains("sand"))
                            {
                                try
                                {
                                    SpawnAnMakeWebs(outsideEnemy);
                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                    }

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse Ghost(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        var playerRef = StartOfRound.Instance.localPlayerController;

        try
        {


            if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
            else
            {
                Mod.ActionQueue.Enqueue(() =>
                {

                    DressGirlAI[] array = UnityEngine.Object.FindObjectsByType<DressGirlAI>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

                    if (array.Length == 0)
                    {
                        status = EffectStatus.Retry;
                    }
                    else
                    {

                        DressGirlAI bird = array[0];
                        playerRef.movementAudio.PlayOneShot(bird.appearStaringSFX[0], 4.0f);
                        WalkieTalkie.TransmitOneShotAudio(playerRef.movementAudio, bird.appearStaringSFX[0], 4.0f);
                        RoundManager.Instance.PlayAudibleNoise(playerRef.transform.position, 12f, 4.0f, 0, false, 911);
                    }

                });
            }

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse NightVision(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        var playerRef = StartOfRound.Instance.localPlayerController;
        if (Mod.nightVision) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        if (TimedThread.isRunning(TimedType.NIGHT_VISION)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.NIGHT_VISION, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }

    public static void setProperty(System.Object a, string prop, System.Object val)
    {
        var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
        f.SetValue(a, val);
    }

    public static System.Object getProperty(System.Object a, string prop)
    {
        var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
        return f.GetValue(a);
    }

    public static void setSubProperty(System.Object a, string prop, string prop2, System.Object val)
    {
        var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
        var f2 = f.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
        f2.SetValue(f, val);
    }

    public static void callSubFunc(System.Object a, string prop, string func, System.Object val)
    {
        callSubFunc(a, prop, func, new object[] { val });
    }

    public static void callSubFunc(System.Object a, string prop, string func, System.Object[] vals)
    {
        var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);


        var p = f.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        p.Invoke(f, vals);

    }

    public static void callFunc(System.Object a, string func, System.Object val)
    {
        callFunc(a, func, new object[] { val });
    }

    public static void callFunc(System.Object a, string func, System.Object[] vals)
    {
        var p = a.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic);
        p.Invoke(a, vals);

    }

    public static System.Object callAndReturnFunc(System.Object a, string func, System.Object val)
    {
        return callAndReturnFunc(a, func, new object[] { val });
    }

    public static System.Object callAndReturnFunc(System.Object a, string func, System.Object[] vals)
    {
        var p = a.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic);
        return p.Invoke(a, vals);

    }

}