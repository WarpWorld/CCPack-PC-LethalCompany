#nullable disable
using DunGen;
using GameNetcodeStuff;
using BepinControl;
using ConnectorLib.JSON;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Xml.Linq;
using Unity.Netcode;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;
using Zeekerss.Core.Singletons;
using UnityEngine.AI;




namespace CrowdControl
{



    public class GameActions
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
        public static EffectResponse HealFull(EffectRequest req)
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
                    LethalCompanyControl.test = true;

                    //StartOfRound.Instance.ChangeLevel(6);
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }


            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse Kill(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (playerRef.health <= 0 || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        if (LethalCompanyControl.isHost)
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse KillCrewmate(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_kill_{(int)player.playerClientId}</size>");
                    });
                }
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse Damage(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            if (ActiveBuffs.IsRunning(BuffType.OHKO)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.INVUL)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (playerRef.health <= 20 || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse DamageCrew(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {

                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_damage_{(int)player.playerClientId}</size>");

                    });
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse Heal(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            if (ActiveBuffs.IsRunning(BuffType.OHKO)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.INVUL)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            try
            {

                var playerRef = StartOfRound.Instance.localPlayerController;

                if (playerRef.health >= 100 || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {

                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }


            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse HealCrew(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {

                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_heal_{(int)player.playerClientId}</size>");

                    });
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }


        public static EffectResponse OHKO(EffectRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (ActiveBuffs.IsRunning(BuffType.OHKO)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.INVUL)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        public static EffectResponse Invul(EffectRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (ActiveBuffs.IsRunning(BuffType.OHKO)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.INVUL)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        #endregion

        #region Stamina
        public static EffectResponse DrainStamins(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            if (playerRef.sprintMeter < 0.1f) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            playerRef.sprintMeter = 0;
            playerRef.isExhausted = true;


            return new EffectResponse(req.ID, EffectStatus.Success);
        }

        public static EffectResponse RestoreStamins(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            if (playerRef.sprintMeter > 0.9f) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            playerRef.sprintMeter = 1.0f;
            playerRef.isExhausted = false;


            return new EffectResponse(req.ID, EffectStatus.Success);
        }

        public static EffectResponse InfiniteStamina(EffectRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (ActiveBuffs.IsRunning(BuffType.INFSTAM)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.NOSTAM)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        public static EffectResponse NoStamina(EffectRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (ActiveBuffs.IsRunning(BuffType.INFSTAM)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.NOSTAM)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        #endregion

        #region Sounds
        public static EffectResponse Footstep(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
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
        public static EffectResponse Screech(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {

                        foreach (var level in StartOfRound.Instance.levels)
                        {
                            foreach (var outsideEnemy in level.OutsideEnemies)
                            {

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "hawk"))
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

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "hawk"))
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

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "hawk"))
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
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

        public static EffectResponse Breathing(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {

                        foreach (var level in StartOfRound.Instance.levels)
                        {
                            foreach (var outsideEnemy in level.OutsideEnemies)
                            {

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "girl"))
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

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "girl"))
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

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "girl"))
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
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

        public static EffectResponse BlobSound(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {

                        foreach (var level in StartOfRound.Instance.levels)
                        {
                            foreach (var outsideEnemy in level.OutsideEnemies)
                            {

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "blob"))
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

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "blob"))
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

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "blob"))
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse HighPitch(EffectRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (ActiveBuffs.IsRunning(BuffType.LOW_PITCH)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.HIGH_PITCH)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            LethalCompanyControl.ActionQueue.Enqueue(() =>
            {
                HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_1.5</size>");
            });

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        public static EffectResponse LowPitch(EffectRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (ActiveBuffs.IsRunning(BuffType.LOW_PITCH)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.HIGH_PITCH)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            LethalCompanyControl.ActionQueue.Enqueue(() =>
            {
                HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_0.65</size>");
            });

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        #endregion

        #region Players
        public static EffectResponse HyperMove(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);
            if (playerRef.isPlayerDead) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.FAST_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.SLOW_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.HYPER_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.FREEZE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }
        public static EffectResponse FastJetpack(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);
            if (playerRef.isPlayerDead) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.FAST_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.SLOW_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.HYPER_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.FREEZE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        public static EffectResponse FastMove(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (playerRef.isPlayerDead) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.FASTJETPACK)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.FAST_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.SLOW_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.HYPER_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.FREEZE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");


            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        public static EffectResponse Drunk(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            int dur = 10;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (playerRef.isPlayerDead) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.DRUNK)) return new EffectResponse(req.ID, EffectStatus.Retry, "");


            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        public static EffectResponse SlowMove(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (playerRef.isPlayerDead) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.FAST_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.SLOW_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.HYPER_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.FREEZE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        public static EffectResponse Freeze(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (playerRef.isPlayerDead) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.FAST_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.SLOW_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.HYPER_MOVE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.FREEZE)) return new EffectResponse(req.ID, EffectStatus.Retry, "");


            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        public static EffectResponse UltraJump(EffectRequest req)
        {

            var playerRef = StartOfRound.Instance.localPlayerController;
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (playerRef.isPlayerDead) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.ULTRA_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.HIGH_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.LOW_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        public static EffectResponse HighJump(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (playerRef.isPlayerDead) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.ULTRA_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.HIGH_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.LOW_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        public static EffectResponse LowJump(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            if (playerRef.isPlayerDead) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.ULTRA_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.HIGH_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
            if (ActiveBuffs.IsRunning(BuffType.LOW_JUMP)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
        }

        public static EffectResponse Revive(EffectRequest req)
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {

                            StartOfRound.Instance.ReviveDeadPlayers();
                            HUDManager.Instance.HideHUD(false);
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_revive</size>");

                        });
                    }
                }
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse TeleportToShip(EffectRequest req)
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {

                            StartOfRound.Instance.ForcePlayerIntoShip();

                        });
                    }
                }
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }
            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse TeleportCrewToShip(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {

                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_ship_{(int)player.playerClientId}</size>");

                    });
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse TeleportToCrew(EffectRequest req)
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            playerRef.TeleportPlayer(player.transform.position);

                        });
                    }
                }
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse TeleportCrewTo(EffectRequest req)
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_tele_{(int)player.playerClientId}_{(int)playerRef.playerClientId}</size>");

                        });
                    }
                }
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse SpawnBody(EffectRequest req)
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse SpawnCrewBody(EffectRequest req)
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse Launch(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse InverseTeleport(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled)
                {
                    status = EffectStatus.Retry;
                }
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        var randomSeed = new System.Random(StartOfRound.Instance.randomMapSeed + 17 + (int)GameNetworkManager.Instance.localPlayerController.playerClientId);//use the actual seed function the tele uses, avoid invalid tp
                        Vector3 position = RoundManager.Instance.insideAINodes[randomSeed.Next(0, RoundManager.Instance.insideAINodes.Length)].transform.position;
                        Vector3 inBoxPredictable = RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(position,10,RoundManager.Instance.navHit, randomSeed: randomSeed);
                        playerRef.TeleportPlayer(inBoxPredictable);
                        playerRef.isInsideFactory = true; playerRef.isInHangarShipRoom = false; playerRef.isInElevator = false;//all bools referring to player, so fine to go on one line as readable.

                    });
                }
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse InverseTeleportCrew(EffectRequest req)
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_inverse_{(int)player.playerClientId}</size>");

                        });
                    }
                }
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }
        #endregion

        #region Enemies

        public static EffectResponse Spawn(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            if (playerRef.isPlayerDead) return new EffectResponse(req.ID, EffectStatus.Failure, "Player is Dead");

            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {

            }
            else
            {
                return new EffectResponse(req.ID, EffectStatus.Failure, "What");
            }
            //a Kidnapper Fox (BushWolf) despawns itself on the spot unless the moon has Vain Shrouds, so a viewer
            //would pay for nothing - refuse it (and get the refund) instead
            if (enteredText[1] == "bush" && !LethalCompanyControl.MoonHasVainShrouds())
                return new EffectResponse(req.ID, EffectStatus.Failure, "Kidnapper Foxes only appear on moons with Vain Shrouds");
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
                    return new EffectResponse(req.ID, EffectStatus.Retry, "");

            }    
            if (enteredText[1] == "landmine" ||enteredText[1] ==  "turretcont" ||enteredText[1] == "spiketrap")
            {
                if (playerRef.isInElevator) return new EffectResponse(req.ID, EffectStatus.Failure, "Player is inside ship");
                found = true;
            }
            if (!found)
                foreach (var Enemy in StartOfRound.Instance.currentLevel.Enemies)
                {
                    if (LethalCompanyControl.IsEnemy(Enemy.enemyType, enteredText[1]) || enteredText[1] == "jester" || enteredText[1] == "cracker")
                    {
                        try
                        {
                            if (!playerRef.isInsideFactory) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is outside");
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


                    if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, enteredText[1]))
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

            if (LethalCompanyControl.isHost)
            {

                LethalCompanyControl.ActionQueue.Enqueue(() =>
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

                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, playerRef.transform.position, Quaternion.identity, LethalCompanyControl.currentStart.propsContainer);
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
                    if (enteredText[1] == "turret")
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_turret_{(int)playerRef.playerClientId}</size>");

                        return;
                    }
                    if (enteredText[1] == "spiketrap")
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_spiketrap_{(int)playerRef.playerClientId}</size>");

                        return;
                    }
                    foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.OutsideEnemies)
                    {

                        if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, enteredText[1]))
                        {

                            try
                            {
                                LethalCompanyControl.SpawnEnemy(outsideEnemy, 1, false);

                            }
                            catch (Exception e)
                            {
                                LethalCompanyControl.mls.LogWarning("Monster is Not Present Outside on Moon, Aborting");

                            }
                            return;
                        }
                    }
                    foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.Enemies)
                    {
                        if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, enteredText[1]))
                        {
                            try
                            {
                                LethalCompanyControl.SpawnEnemy(outsideEnemy, 1, true);

                            }
                            catch (Exception e)
                            {
                                LethalCompanyControl.mls.LogWarning("Monster is Not Present Inside on Moon, Aborting");
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_cspawn_{enteredText[1]}_{(int)playerRef.playerClientId}</size>");
                    });
                }
                catch (Exception e)
                {
                    LethalCompanyControl.mls.LogError(e.ToString());
                }
            }

            return new EffectResponse(req.ID, EffectStatus.Success, "");
        }

        public static EffectResponse CrewSpawn(EffectRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;

            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {

            }
            else
            {
                return new EffectResponse(req.ID, EffectStatus.Failure, "");
            }
            //a Kidnapper Fox (BushWolf) despawns itself on the spot unless the moon has Vain Shrouds, so a viewer
            //would pay for nothing - refuse it (and get the refund) instead
            if (enteredText[1] == "bush" && !LethalCompanyControl.MoonHasVainShrouds())
                return new EffectResponse(req.ID, EffectStatus.Failure, "Kidnapper Foxes only appear on moons with Vain Shrouds");

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

                if (enteredText[1] == "landmine" || enteredText[1] == "turretcont" || enteredText[1] == "spiketrap")
                {
                    if (player.isInElevator) return new EffectResponse(req.ID, EffectStatus.Failure, "Player is inside ship");
                    found = true;
                }


                if (!found)
                    foreach (var Enemy in StartOfRound.Instance.currentLevel.Enemies)
                    {


                        if (LethalCompanyControl.IsEnemy(Enemy.enemyType, enteredText[1]) || enteredText[1] == "jester" || enteredText[1] ==  "cracker")
                        {
                            try
                            {
                                if (!player.isInsideFactory) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is Inside");
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
                        if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, enteredText[1]))
                        {
                            found = true;
                            LethalCompanyControl.mls.LogInfo("Found Monster: " + enteredText[1]);
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
                if (!found) return new EffectResponse(req.ID, EffectStatus.Retry, "Player is inside ship");

                if (LethalCompanyControl.isHost)
                {

                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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

                                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, player.transform.position, Quaternion.identity, LethalCompanyControl.currentStart.propsContainer);
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
                            if (enteredText[1] == "turret")
                            {
                                HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_turret_{(int)player.playerClientId}</size>");

                                return;
                            }
                            if (enteredText[1] == "spiketrap")
                            {
                                HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_spiketrap_{(int)player.playerClientId}</size>");

                                return;
                            }
                            if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, enteredText[1]))
                            {
                                try
                                {
                                    LethalCompanyControl.SpawnCrewEnemy(player, outsideEnemy, 1, false);

                                }
                                catch (Exception e)
                                {

                                }
                                return;
                            }
                        }
                        foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.Enemies)
                        {

                            if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, enteredText[1]))
                            {
                                try
                                {
                                    LethalCompanyControl.SpawnCrewEnemy(player, outsideEnemy, 1, false);

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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_cspawn_{enteredText[1]}_{(int)player.playerClientId}</size>");
                    });
                }
            }

            return new EffectResponse(req.ID, EffectStatus.Success, "");
        }
        #endregion

        #region Items
        public static EffectResponse TakeItem(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        playerRef.DespawnHeldObject();
                    });
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse TakeCrewItem(EffectRequest req)
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

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !player.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_take_{(int)player.playerClientId}</size>");
                    });
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse DropItem(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        playerRef.DiscardHeldObject();
                    });
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse UnchargeItem(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled || playerRef.currentlyHeldObjectServer == null) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse ChargeItem(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        playerRef.currentlyHeldObjectServer.insertedBattery.charge = 1.0f;
                        playerRef.currentlyHeldObjectServer.SyncBatteryServerRpc((int)playerRef.currentlyHeldObjectServer.insertedBattery.charge);
                    });
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse GiveItem(EffectRequest req)
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

                
                if (!LethalCompanyControl.isHost)
                {
                    givedelay = 20;
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(StartOfRound.Instance.allItemsList.itemsList[give].spawnPrefab, playerRef.transform.position, Quaternion.identity, LethalCompanyControl.currentStart.propsContainer);
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse GiveCrewItem(EffectRequest req)
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

                if (!LethalCompanyControl.isHost)
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(StartOfRound.Instance.allItemsList.itemsList[give].spawnPrefab, player.transform.position, Quaternion.identity, LethalCompanyControl.currentStart.propsContainer);
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse BuyItem(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        #endregion

        #region VehicleStuff

        public static EffectResponse BuyCruiser(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            int give = 0;
            var playerRef = StartOfRound.Instance.localPlayerController;


            Terminal terminal = LethalCompanyControl.cachedTerminal;
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        terminal.BuyVehicleServerRpc(0, terminal.groupCredits, false);
                        HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " sent a pod with a " + veh.vehicleDisplayName);
                    });
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }


        public static EffectResponse TurnOnVehicle(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;
            string[] enteredText = req.code.Split('_');
            bool found = false;
            VehicleController Veh1 = LethalCompanyControl.cachedVehicle;
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                    LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                }
            }
            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse SpringChair(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;
            bool found = false;
            string[] enteredText = req.code.Split('_');
            VehicleController Veh1 = LethalCompanyControl.cachedVehicle;
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                    LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                }
            }
            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse TurnOffEngine(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;
            bool found = false;
            string[] enteredText = req.code.Split('_');
            VehicleController Veh1 = LethalCompanyControl.cachedVehicle;
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                    LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                }
            }
            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse DestroyVehicle(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;
            bool found = false;
            string[] enteredText = req.code.Split('_');
            VehicleController Veh1 = LethalCompanyControl.cachedVehicle;
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                    LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                }
            }
            return new EffectResponse(req.ID, status, message);
        }

        #endregion

        #region Weather
        public static EffectResponse Weather(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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

                        //LethalCompanyControl.mls.LogInfo("weather changed");

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

                        //LethalCompanyControl.mls.LogInfo($"round manager: {RoundManager.Instance}");
                        //LethalCompanyControl.mls.LogInfo($"tod: {TimeOfDay.Instance}");

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
                            LethalCompanyControl.mls.LogError(ex.ToString());
                        }

                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_weather_{(int)give}</size>");

                    });
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        #endregion

        #region Scrap_Money
        public static EffectResponse AddCredits(EffectRequest req)
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

            Terminal terminal = LethalCompanyControl.cachedTerminal;

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                if (give > 0 && terminal.groupCredits + give > 99999) return new EffectResponse(req.ID, EffectStatus.Retry);
                if (give < 0 && terminal.groupCredits + give < 0) return new EffectResponse(req.ID, EffectStatus.Retry);

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    if (LethalCompanyControl.isHost)
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_credits_{give}</size>");
                        });
                    }
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }
        public static EffectResponse AddQuota(EffectRequest req)
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

            Terminal terminal = LethalCompanyControl.cachedTerminal;

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var tod = TimeOfDay.Instance;
                if (give > 0 && tod.profitQuota + give > 25000) return new EffectResponse(req.ID, EffectStatus.Retry);
                if (give < 0 && tod.profitQuota + give < 5) return new EffectResponse(req.ID, EffectStatus.Retry);

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse AddProfit(EffectRequest req)
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

                //LethalCompanyControl.mls.LogInfo($"playerref: {playerRef}");
                //LethalCompanyControl.mls.LogInfo($"TimeOfDay: {tod}");

                if (give > 0 && tod.quotaFulfilled >= tod.profitQuota) return new EffectResponse(req.ID, EffectStatus.Retry);
                if (give < 0 && tod.quotaFulfilled <= 0) return new EffectResponse(req.ID, EffectStatus.Retry);



                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse AddScrap(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;


            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {

                        RoundManager.Instance.SpawnScrapInLevel();
                        HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " spawned more scrap in the level");
                    });
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        #endregion

        #region Time
        public static EffectResponse AddHour(EffectRequest req)
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
                    if (LethalCompanyControl.isHost)
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            float num = tod.lengthOfHours;
                            tod.globalTime += num;
                            tod.timeUntilDeadline -= num;
                            callFunc(tod, "MoveTimeOfDay", null);
                        });
                    }
                    else
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_addhour</size>");
                        });
                    }


                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse RemoveHour(EffectRequest req)
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
                    if (LethalCompanyControl.isHost)
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            float num = tod.lengthOfHours;
                            tod.globalTime -= num;
                            tod.timeUntilDeadline += num;
                            callFunc(tod, "MoveTimeOfDay", null);
                        });
                    }
                    else
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_remhour</size>");
                        });
                    }

                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse AddDay(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse RemoveDay(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        #endregion

        #region Ship
        public static EffectResponse CloseDoors(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse OpenDoors(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse ShipLeave(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {
                        TimeOfDay.Instance.votesForShipToLeaveEarly = 998;
                        TimeOfDay.Instance.SetShipLeaveEarlyServerRpc();
                    });
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        #endregion

        #region Misc
        public static EffectResponse BreakersOn(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                BreakerBox breakerBox = LethalCompanyControl.cachedBreakerBox;

                if (breakerBox == null || breakerBox.isPowerOn || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    if (LethalCompanyControl.isHost)
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            RoundManager.Instance.PowerSwitchOnClientRpc();
                        });
                    }
                    else
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_poweron</size>");
                        });
                    }
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse KillEnemies(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                EnemyAI[] array = LethalCompanyControl.cachedEnemies;

                bool found = false;
                for (int i = 0; i < array.Length; i++)
                {
                    if (Vector3.Distance(array[i].transform.position, playerRef.transform.position) < 24f && !array[i].isEnemyDead && array[i].stunNormalizedTimer <= 0)
                        found = true;
                }

                if (!found || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse BreakersOff(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                BreakerBox breakerBox = LethalCompanyControl.cachedBreakerBox;

                if (breakerBox == null || !breakerBox.isPowerOn || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    if (LethalCompanyControl.isHost)
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            RoundManager.Instance.PowerSwitchOffClientRpc();
                        });
                    }
                    else
                    {
                        LethalCompanyControl.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_poweroff</size>");
                        });
                    }
                }

            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        #endregion
        public static EffectResponse Lightning(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse PlayHorn(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";


            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static void SpawnAnMakeWebs(SpawnableEnemyWithRarity source)
        {
            //LethalCompanyControl.mls.LogInfo($"Creating Webs");

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

        public static EffectResponse CreateWebs(EffectRequest req)
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
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
                    {

                        foreach (var level in StartOfRound.Instance.levels)
                        {
                            foreach (var outsideEnemy in level.OutsideEnemies)
                            {

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "spider"))
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

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "spider"))
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

                                if (LethalCompanyControl.IsEnemy(outsideEnemy.enemyType, "spider"))
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse Ghost(EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = EffectStatus.Retry;
                else
                {
                    LethalCompanyControl.ActionQueue.Enqueue(() =>
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
                LethalCompanyControl.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }

        public static EffectResponse NightVision(EffectRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = (int)(req.duration.Value / 1000);

            var playerRef = StartOfRound.Instance.localPlayerController;
            if (LethalCompanyControl.nightVision) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            if (ActiveBuffs.IsRunning(BuffType.NIGHT_VISION)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

            return new EffectResponse(req.ID, EffectStatus.Success, ""); // TimedGameEffect applies the buff
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
            InvokeWithDefaults(p, f, vals);

        }

        /// <summary>
        /// Invokes a game method found by name, filling in any trailing optional parameters with their defaults.
        /// Reflection does not apply C# default arguments, so when the game adds an optional parameter (v81 added
        /// one to PlayerControllerB.FirstEmptyItemSlot) a call made with the old argument count throws
        /// TargetParameterCountException - which broke every Give Item effect on v81.
        /// </summary>
        private static System.Object InvokeWithDefaults(MethodInfo p, System.Object target, System.Object[] vals)
        {
            vals = vals ?? new System.Object[0];
            ParameterInfo[] ps = p.GetParameters();
            if (vals.Length < ps.Length && ps.Skip(vals.Length).All(x => x.IsOptional))
                vals = vals.Concat(ps.Skip(vals.Length).Select(x => x.DefaultValue is DBNull ? null : x.DefaultValue)).ToArray();
            return p.Invoke(target, vals);
        }

        public static void callFunc(System.Object a, string func, System.Object val)
        {
            callFunc(a, func, new object[] { val });
        }

        public static void callFunc(System.Object a, string func, System.Object[] vals)
        {
            var p = a.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic);
            InvokeWithDefaults(p, a, vals);

        }

        public static System.Object callAndReturnFunc(System.Object a, string func, System.Object val)
        {
            return callAndReturnFunc(a, func, new object[] { val });
        }

        public static System.Object callAndReturnFunc(System.Object a, string func, System.Object[] vals)
        {
            var p = a.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic);
            return InvokeWithDefaults(p, a, vals);

        }

    }
}
