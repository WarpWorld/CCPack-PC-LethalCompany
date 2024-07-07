
using DunGen;
using GameNetcodeStuff;
using LethalCompanyTestMod;
using Newtonsoft.Json.Linq;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Unity.Netcode;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;


namespace ControlValley
{
    public delegate CrowdResponse CrowdDelegate(ControlClient client, CrowdRequest req);

    public class CrowdDelegates
    {
        public static uint msgid = 0;

        public static uint givedelay = 0;

        #region Health
        public static CrowdResponse HealFull(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            try
            {
                NetworkManager networkManager = StartOfRound.Instance.NetworkManager;
                if (networkManager == null || !networkManager.IsListening)
                {
                    status = CrowdResponse.Status.STATUS_RETRY;
                }
                else
                {
                    TestMod.test = true;

                    //StartOfRound.Instance.ChangeLevel(6);
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }


            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Kill(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (playerRef.health <= 0 || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        if (TestMod.isHost)
                        {
                            playerRef.KillPlayer(playerRef.transform.up * 100.0f);
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse KillCrewmate(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            try
            {
                var player = list[UnityEngine.Random.Range(0, list.Count)];

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_kill_{(int)player.playerClientId}</size>");
                    });
                }
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Damage(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (BuffThread.isRunning(BuffType.OHKO)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.INVUL)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (playerRef.health <= 20 || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        int dmg = 25;
                        if (playerRef.health < 25) dmg = playerRef.health - 1;
                        playerRef.DamagePlayer(dmg);
                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse DamageCrew(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            try
            {
                var player = list[UnityEngine.Random.Range(0, list.Count)];

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !player.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {

                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_damage_{(int)player.playerClientId}</size>");

                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Heal(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (BuffThread.isRunning(BuffType.OHKO)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.INVUL)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {

                var playerRef = StartOfRound.Instance.localPlayerController;

                if (playerRef.health >= 100 || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {

                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }


            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse HealCrew(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            try
            {
                var player = list[UnityEngine.Random.Range(0, list.Count)];

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !player.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {

                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_heal_{(int)player.playerClientId}</size>");

                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }


        public static CrowdResponse OHKO(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.OHKO)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.INVUL)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new BuffThread(req.GetReqID(), BuffType.OHKO, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse Invul(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.OHKO)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.INVUL)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new BuffThread(req.GetReqID(), BuffType.INVUL, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        #endregion

        #region Stamina
        public static CrowdResponse DrainStamins(ControlClient client, CrowdRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            if (playerRef.sprintMeter < 0.1f) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            playerRef.sprintMeter = 0;
            playerRef.isExhausted = true;


            return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse RestoreStamins(ControlClient client, CrowdRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            if (playerRef.sprintMeter > 0.9f) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            playerRef.sprintMeter = 1.0f;
            playerRef.isExhausted = false;


            return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse InfiniteStamina(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.INFSTAM)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.NOSTAM)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new BuffThread(req.GetReqID(), BuffType.INFSTAM, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse NoStamina(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.INFSTAM)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.NOSTAM)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new BuffThread(req.GetReqID(), BuffType.NOSTAM, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        #endregion

        #region Sounds
        public static CrowdResponse Footstep(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        playerRef.PlayFootstepServer();
                        playerRef.movementAudio.PlayOneShot(StartOfRound.Instance.footstepSurfaces[0].clips[0], 4.0f);
                        WalkieTalkie.TransmitOneShotAudio(playerRef.movementAudio, StartOfRound.Instance.footstepSurfaces[0].clips[0], 4.0f);
                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
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
        public static CrowdResponse Screech(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
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

        public static CrowdResponse Breathing(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
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

        public static CrowdResponse BlobSound(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse HighPitch(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.LOW_PITCH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.HIGH_PITCH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            TestMod.ActionQueue.Enqueue(() =>
            {
                HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_1.5</size>");
            });

            new Thread(new BuffThread(req.GetReqID(), BuffType.HIGH_PITCH, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse LowPitch(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.LOW_PITCH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.HIGH_PITCH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            TestMod.ActionQueue.Enqueue(() =>
            {
                HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_pitch_0.65</size>");
            });

            new Thread(new BuffThread(req.GetReqID(), BuffType.LOW_PITCH, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        #endregion

        #region Players
        public static CrowdResponse HyperMove(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.FAST_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.SLOW_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.HYPER_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.FREEZE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new BuffThread(req.GetReqID(), BuffType.HYPER_MOVE, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse FastMove(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.FAST_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.SLOW_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.HYPER_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.FREEZE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new BuffThread(req.GetReqID(), BuffType.FAST_MOVE, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse Drunk(ControlClient client, CrowdRequest req)
        {
            int dur = 10;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.DRUNK)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new BuffThread(req.GetReqID(), BuffType.DRUNK, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse SlowMove(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.FAST_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.SLOW_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.HYPER_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.FREEZE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new BuffThread(req.GetReqID(), BuffType.SLOW_MOVE, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse Freeze(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.FAST_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.SLOW_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.HYPER_MOVE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.FREEZE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new BuffThread(req.GetReqID(), BuffType.FREEZE, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse UltraJump(ControlClient client, CrowdRequest req)
        {

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.ULTRA_JUMP)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.HIGH_JUMP)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.LOW_JUMP)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new BuffThread(req.GetReqID(), BuffType.ULTRA_JUMP, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse HighJump(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.ULTRA_JUMP)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.HIGH_JUMP)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.LOW_JUMP)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new BuffThread(req.GetReqID(), BuffType.HIGH_JUMP, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse LowJump(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (BuffThread.isRunning(BuffType.ULTRA_JUMP)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.HIGH_JUMP)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (BuffThread.isRunning(BuffType.LOW_JUMP)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new BuffThread(req.GetReqID(), BuffType.LOW_JUMP, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse Revive(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {

                {
                    if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                    else
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {

                            StartOfRound.Instance.ReviveDeadPlayers();
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_revive</size>");

                        });
                    }
                }
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse TeleportToShip(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {

                if (GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                    else
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {

                            StartOfRound.Instance.ForcePlayerIntoShip();

                        });
                    }
                }
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }
            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse TeleportCrewToShip(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            try
            {
                var player = list[UnityEngine.Random.Range(0, list.Count)];

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {

                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_ship_{(int)player.playerClientId}</size>");

                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse TeleportToCrew(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            try
            {
                var player = list[UnityEngine.Random.Range(0, list.Count)];

                {
                    if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                    else
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            playerRef.TeleportPlayer(player.transform.position);

                        });
                    }
                }
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse TeleportCrewTo(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            try
            {
                var player = list[UnityEngine.Random.Range(0, list.Count)];

                {
                    if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                    else
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_tele_{(int)player.playerClientId}_{(int)playerRef.playerClientId}</size>");

                        });
                    }
                }
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse SpawnBody(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {

                if (GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                    else
                    {
                        TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse SpawnCrewBody(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "Could not find Active player to Spawn Crew Body");
            }

            try
            {
                var player = list[UnityEngine.Random.Range(0, StartOfRound.Instance.connectedPlayersAmount)]; //test fixing Crew bodies?

                if (player.isInHangarShipRoom) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                    else
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {

                            msgid++;
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_body_{(int)player.playerClientId}_{msgid}</size>");

                        });
                    }
                }
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Launch(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        #endregion

        #region Enemies

        public static CrowdResponse Spawn(ControlClient client, CrowdRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;
            if (playerRef.isPlayerDead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "Player is dead");

            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {

            }
            else
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE);
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
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);

            }

            if (enteredText[1] == "landmine")
            {
                if (playerRef.isInElevator) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is inside ship");
                found = true;
            }




            if (!found)
                foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.Enemies)
                {


                    if (outsideEnemy.enemyType.enemyName.ToLower().Contains(enteredText[1]))
                    {
                        found = true;
                        try
                        {
                            if (!playerRef.isInsideFactory) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "Player is outside");

                        }
                        catch (Exception e)
                        {

                        }
                    }
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
                                if (playerRef.isInsideFactory) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "Player is outside");

                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }

            if (found == false) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "Player is outside");

            if (TestMod.isHost)
            {

                TestMod.ActionQueue.Enqueue(() =>
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

                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, playerRef.transform.position, Quaternion.identity, TestMod.currentStart.propsContainer);
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
                                TestMod.SpawnEnemy(outsideEnemy, 1, false);

                            }
                            catch (Exception e)
                            {
                                TestMod.mls.LogWarning("Monster is Not Present Outside on Moon, Aborting");

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
                                TestMod.SpawnEnemy(outsideEnemy, 1, true);

                            }
                            catch (Exception e)
                            {
                                TestMod.mls.LogWarning("Monster is Not Present Inside on Moon, Aborting");
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
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_cspawn_{enteredText[1]}_{(int)playerRef.playerClientId}</size>");
                    });
                }
                catch (Exception e)
                {
                    TestMod.mls.LogError(e.ToString());
                }
            }


            return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse CrewSpawn(ControlClient client, CrowdRequest req)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;


            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {

            }
            else
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE);
            }

            List<PlayerControllerB> list = new List<PlayerControllerB>();


            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player != null && !player.isPlayerDead && player != playerRef && player.isActiveAndEnabled && player.isPlayerControlled)
                    list.Add(player);
            }

            if (list.Count <= 0)
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            {
                PlayerControllerB player;
                player = list[UnityEngine.Random.Range(0, list.Count)];

                bool found = false;

                if (enteredText[1] == "landmine")
                {
                    if (player.isInElevator) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is inside ship");
                    found = true;
                }

                if (!found)
                    foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.Enemies)
                    {

                        if (outsideEnemy.enemyType.enemyName.ToLower().Contains(enteredText[1]))
                        {
                            found = true;
                            try
                            {
                                if (!player.isInsideFactory) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);

                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }

                if (!found)
                    foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.OutsideEnemies)
                    {


                        if (outsideEnemy.enemyType.enemyName.ToLower().Contains(enteredText[1]))
                        {
                            found = true;
                            TestMod.mls.LogInfo("Found Monster: " + enteredText[1]);
                            if (enteredText[1] == "giant" || enteredText[1] == "levi" || enteredText[1] == "radmech" || enteredText[1].ToLower().Contains("bush"))
                            {
                                try
                                {
                                    if (player.isInsideFactory) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "Player is outside");

                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }
                    }

                if (!found) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "Player is outside");

                if (TestMod.isHost)
                {

                    TestMod.ActionQueue.Enqueue(() =>
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

                                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, player.transform.position, Quaternion.identity, TestMod.currentStart.propsContainer);
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
                                    TestMod.SpawnCrewEnemy(player, outsideEnemy, 1, false);

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
                                    TestMod.SpawnCrewEnemy(player, outsideEnemy, 1, false);

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
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_cspawn_{enteredText[1]}_{(int)player.playerClientId}</size>");
                    });
                }
            }

            return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_SUCCESS);
        }

        #endregion

        #region Items
        public static CrowdResponse TakeItem(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled || playerRef.currentlyHeldObjectServer == null) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        playerRef.DespawnHeldObject();
                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse TakeCrewItem(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            try
            {
                var player = list[UnityEngine.Random.Range(0, list.Count)];

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !player.playersManager.shipDoorsEnabled || player.currentlyHeldObjectServer == null) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_take_{(int)player.playerClientId}</size>");
                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse DropItem(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled || playerRef.currentlyHeldObjectServer == null) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        playerRef.DiscardHeldObject();
                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse UnchargeItem(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled || playerRef.currentlyHeldObjectServer == null) status = CrowdResponse.Status.STATUS_RETRY;
                else if (!playerRef.currentlyHeldObjectServer.itemProperties.requiresBattery || playerRef.currentlyHeldObjectServer.insertedBattery == null || playerRef.currentlyHeldObjectServer.insertedBattery.charge == 0) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        playerRef.currentlyHeldObjectServer.insertedBattery.charge = 0;
                        playerRef.currentlyHeldObjectServer.SyncBatteryServerRpc((int)playerRef.currentlyHeldObjectServer.insertedBattery.charge);
                        playerRef.currentlyHeldObjectServer.UseUpBatteries();
                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse ChargeItem(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled || playerRef.currentlyHeldObjectServer == null) status = CrowdResponse.Status.STATUS_RETRY;
                else if (!playerRef.currentlyHeldObjectServer.itemProperties.requiresBattery || playerRef.currentlyHeldObjectServer.insertedBattery == null || playerRef.currentlyHeldObjectServer.insertedBattery.charge >= 0.96f) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        playerRef.currentlyHeldObjectServer.insertedBattery.charge = 1.0f;
                        playerRef.currentlyHeldObjectServer.SyncBatteryServerRpc((int)playerRef.currentlyHeldObjectServer.insertedBattery.charge);
                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse GiveItem(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            int give = 0;

            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {
                give = int.Parse(enteredText[1]);
            }
            else
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE);
            }
            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var slot = (int)callAndReturnFunc(playerRef, "FirstEmptyItemSlot", null);



                if (playerRef.inSpecialInteractAnimation || slot == -1 || givedelay > 0)
                {
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
                }

                if (!TestMod.isHost)
                {
                    givedelay = 20;
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        msgid++;
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_giver_{give}_{(int)playerRef.playerClientId}_{msgid}</size>");
                    });
                    return new CrowdResponse(req.GetReqID(), status, message);
                }

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    givedelay = 20;
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(StartOfRound.Instance.allItemsList.itemsList[give].spawnPrefab, playerRef.transform.position, Quaternion.identity, TestMod.currentStart.propsContainer);
                        gameObject.GetComponent<GrabbableObject>().fallTime = 0f;
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        //This might not be needed anymore, since GiveItem now works for Masks?
        //public static CrowdResponse GiveSpecial(ControlClient client, CrowdRequest req)
        //{
        //    CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
        //    string message = "";

        //    try
        //    {
        //        string[] enteredText = req.code.Split('_');
        //        if (enteredText.Length == 2)
        //        {

        //        }
        //        else
        //        {
        //            return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE);
        //        }
        //        var playerRef = StartOfRound.Instance.localPlayerController;
        //        var slot = (int)callAndReturnFunc(playerRef, "FirstEmptyItemSlot", null);
        //        GameObject prefab = null;
        //        foreach (var level in StartOfRound.Instance.levels)
        //        {
        //            if (prefab == null)
        //                foreach (var spawn in level.spawnableScrap)
        //                {
        //                    if (spawn.spawnableItem.name.ToLower() == enteredText[1]) prefab = spawn.spawnableItem.spawnPrefab;
        //                }
        //         }
                
        //        if (playerRef.inSpecialInteractAnimation || slot == -1 || givedelay > 0 || prefab == null)
        //        {
        //            return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
        //        }

        //        if (!TestMod.isHost)
        //        {
        //            givedelay = 20;
        //            TestMod.ActionQueue.Enqueue(() =>
        //            {
        //                msgid++;
        //                HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_mgiver_{enteredText[1]}_{(int)playerRef.playerClientId}_{msgid}</size>");
        //            });
        //            return new CrowdResponse(req.GetReqID(), status, message);
        //        }

        //        if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
        //        else
        //        {
        //            givedelay = 20;
        //            TestMod.ActionQueue.Enqueue(() =>
        //            {

        //                Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
        //                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, playerRef.transform.position, Quaternion.identity, TestMod.currentStart.propsContainer);
        //                gameObject.GetComponent<GrabbableObject>().fallTime = 0f;
        //                gameObject.GetComponent<NetworkObject>().Spawn(false);

        //                var grab = gameObject.GetComponent<GrabbableObject>();

        //                setProperty(playerRef, "currentlyGrabbingObject", grab);
        //                setProperty(playerRef, "grabInvalidated", false);


        //                NetworkObject networkObject = grab.NetworkObject;
        //                if (networkObject == null || !networkObject.IsSpawned)
        //                {
        //                    return;
        //                }
        //                grab.InteractItem();


        //                playerRef.playerBodyAnimator.SetBool("GrabInvalidated", false);
        //                playerRef.playerBodyAnimator.SetBool("GrabValidated", false);
        //                playerRef.playerBodyAnimator.SetBool("cancelHolding", false);
        //                playerRef.playerBodyAnimator.ResetTrigger("Throw");

        //                callFunc(playerRef, "SetSpecialGrabAnimationBool", new System.Object[] { true, null });

        //                playerRef.isGrabbingObjectAnimation = true;
        //                playerRef.cursorIcon.enabled = false;
        //                playerRef.cursorTip.text = "";
        //                playerRef.twoHanded = grab.itemProperties.twoHanded;
        //                playerRef.carryWeight += Mathf.Clamp(grab.itemProperties.weight - 1f, 0f, 10f);
        //                if (grab.itemProperties.grabAnimationTime > 0f)
        //                {
        //                    playerRef.grabObjectAnimationTime = grab.itemProperties.grabAnimationTime;
        //                }
        //                else
        //                {
        //                    playerRef.grabObjectAnimationTime = 0.4f;
        //                }

        //                callFunc(playerRef, "GrabObjectServerRpc", new NetworkObjectReference(networkObject));

        //                Coroutine goc = (Coroutine)getProperty(playerRef, "grabObjectCoroutine");

        //                if (goc != null)
        //                {
        //                    ((UnityEngine.MonoBehaviour)playerRef).StopCoroutine(goc);
        //                }

        //                setProperty(playerRef, "grabObjectCoroutine", ((UnityEngine.MonoBehaviour)playerRef).StartCoroutine("GrabObject"));
        //            });
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        status = CrowdResponse.Status.STATUS_RETRY;
        //        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        //    }

        //    return new CrowdResponse(req.GetReqID(), status, message);
        //}

        public static CrowdResponse GiveCrewItem(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            int give = 0;
            var playerRef = StartOfRound.Instance.localPlayerController;

            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {
                give = int.Parse(enteredText[1]);
            }
            else
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE);
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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }


            try
            {
                PlayerControllerB player;

                player = list[UnityEngine.Random.Range(0, list.Count)];

                if (!TestMod.isHost)
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        msgid++;
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_giver_{give}_{(int)player.playerClientId}_{msgid}</size>");
                    });
                    return new CrowdResponse(req.GetReqID(), status, message);
                }

                var slot = (int)callAndReturnFunc(player, "FirstEmptyItemSlot", null);

                if (player.inSpecialInteractAnimation || slot == -1)
                {
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
                }

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(StartOfRound.Instance.allItemsList.itemsList[give].spawnPrefab, player.transform.position, Quaternion.identity, TestMod.currentStart.propsContainer);
                        gameObject.GetComponent<GrabbableObject>().fallTime = 0f;
                        gameObject.GetComponent<NetworkObject>().Spawn(false);

                        msgid++;
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_give_{give}_{(int)player.playerClientId}_{gameObject.GetComponent<NetworkObject>().NetworkObjectId}_{msgid}</size>");

                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse GiveCrewSpecial(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {
            }
            else
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE);
            }

            GameObject prefab = null;

            foreach (var level in StartOfRound.Instance.levels)
            {
                if (prefab == null)
                    foreach (var spawn in level.spawnableScrap)
                    {
                        if (spawn.spawnableItem.name.ToLower() == enteredText[1]) prefab = spawn.spawnableItem.spawnPrefab;
                    }
            }

            if (prefab == null)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);


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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }


            try
            {
                PlayerControllerB player;

                player = list[UnityEngine.Random.Range(0, list.Count)];

                if (!TestMod.isHost)
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        msgid++;
                        if (player.IsHost)
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_mgiver_{enteredText[1]}_-1_{msgid}</size>");
                        else
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_mgiver_{enteredText[1]}_{(int)player.playerClientId}_{msgid}</size>");
                    });
                    return new CrowdResponse(req.GetReqID(), status, message);
                }

                var slot = (int)callAndReturnFunc(player, "FirstEmptyItemSlot", null);

                if (player.inSpecialInteractAnimation || slot == -1)
                {
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
                }

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, player.transform.position, Quaternion.identity, TestMod.currentStart.propsContainer);
                        gameObject.GetComponent<GrabbableObject>().fallTime = 0f;
                        gameObject.GetComponent<NetworkObject>().Spawn(false);

                        msgid++;
                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_mgive_{enteredText[1]}_{(int)player.playerClientId}_{gameObject.GetComponent<NetworkObject>().NetworkObjectId}_{msgid}</size>");

                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse BuyItem(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            int give = 0;
            var playerRef = StartOfRound.Instance.localPlayerController;

            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {
                give = int.Parse(enteredText[1]);
            }
            else
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE);
            }


            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {

                        Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

                        int[] a = { give };



                        terminal.BuyItemsServerRpc(a, terminal.groupCredits, 0);

                        HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " sent a pod with a " + terminal.buyableItemsList[give].name);
                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        #endregion

        #region Weather
        public static CrowdResponse Weather(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            int give = 0;

            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {
                give = int.Parse(enteredText[1]);
            }
            else
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE);
            }

            if (StartOfRound.Instance.currentLevel.currentWeather == (LevelWeatherType)give)
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);
            }

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                            TestMod.mls.LogError(ex.ToString());
                        }

                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_weather_{(int)give}</size>");

                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        #endregion

        #region Scrap_Money
        public static CrowdResponse AddCredits(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            int give = 0;

            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {
                give = int.Parse(enteredText[1]);
            }
            else
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE);
            }

            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                if (give > 0 && terminal.groupCredits + give > 99999) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);
                if (give < 0 && terminal.groupCredits + give < 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    if (TestMod.isHost)
                    {
                        TestMod.ActionQueue.Enqueue(() =>
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
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_credits_{give}</size>");
                        });
                    }
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }
        public static CrowdResponse AddQuota(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            int give = 0;

            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {
                give = int.Parse(enteredText[1]);
            }
            else
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE);
            }

            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var tod = TimeOfDay.Instance;
                if (give > 0 && tod.profitQuota + give > 25000) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);
                if (give < 0 && tod.profitQuota + give < 5) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse AddProfit(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            int give = 0;

            string[] enteredText = req.code.Split('_');
            if (enteredText.Length == 2)
            {
                give = int.Parse(enteredText[1]);
            }
            else
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE);
            }



            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var tod = TimeOfDay.Instance;

                //TestMod.mls.LogInfo($"playerref: {playerRef}");
                //TestMod.mls.LogInfo($"TimeOfDay: {tod}");

                if (give > 0 && tod.quotaFulfilled >= tod.profitQuota) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);
                if (give < 0 && tod.quotaFulfilled <= 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);



                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse AddScrap(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;


            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {

                        RoundManager.Instance.SpawnScrapInLevel();
                        HUDManager.Instance.DisplayTip("Crowd Control", req.viewer + " spawned more scrap in the level");
                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        #endregion

        #region Time
        public static CrowdResponse AddHour(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var tod = TimeOfDay.Instance;

                if (!StartOfRound.Instance.currentLevel.planetHasTime || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    if (TestMod.isHost)
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            float num = tod.lengthOfHours;
                            tod.globalTime += num;
                            tod.timeUntilDeadline -= num;
                            callFunc(tod, "MoveTimeOfDay", null);
                        });
                    }
                    else
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_addhour</size>");
                        });
                    }


                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse RemoveHour(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var tod = TimeOfDay.Instance;

                if (!StartOfRound.Instance.currentLevel.planetHasTime || tod.globalTime <= tod.lengthOfHours || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    if (TestMod.isHost)
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            float num = tod.lengthOfHours;
                            tod.globalTime -= num;
                            tod.timeUntilDeadline += num;
                            callFunc(tod, "MoveTimeOfDay", null);
                        });
                    }
                    else
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_remhour</size>");
                        });
                    }

                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse AddDay(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var tod = TimeOfDay.Instance;

                if (!StartOfRound.Instance.currentLevel.planetHasTime || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse RemoveDay(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var tod = TimeOfDay.Instance;

                if (TimeOfDay.Instance.timeUntilDeadline <= TimeOfDay.Instance.totalTime || !StartOfRound.Instance.currentLevel.planetHasTime || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        #endregion

        #region Ship
        public static CrowdResponse CloseDoors(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var tod = TimeOfDay.Instance;

                if (StartOfRound.Instance.hangarDoorsClosed || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse OpenDoors(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var tod = TimeOfDay.Instance;

                if (!StartOfRound.Instance.hangarDoorsClosed || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse ShipLeave(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var tod = TimeOfDay.Instance;

                if (TimeOfDay.Instance.votedShipToLeaveEarlyThisRound || !StartOfRound.Instance.currentLevel.planetHasTime || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        TimeOfDay.Instance.votesForShipToLeaveEarly = 998;
                        TimeOfDay.Instance.SetShipLeaveEarlyServerRpc();
                    });
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        #endregion

        #region Misc
        public static CrowdResponse BreakersOn(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                BreakerBox breakerBox = UnityEngine.Object.FindObjectOfType<BreakerBox>();

                if (breakerBox == null || breakerBox.isPowerOn || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    if (TestMod.isHost)
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            RoundManager.Instance.PowerSwitchOnClientRpc();
                        });
                    }
                    else
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_poweron</size>");
                        });
                    }
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse KillEnemies(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
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

                if (!found || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse BreakersOff(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                BreakerBox breakerBox = UnityEngine.Object.FindObjectOfType<BreakerBox>();

                if (breakerBox == null || !breakerBox.isPowerOn || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    if (TestMod.isHost)
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            RoundManager.Instance.PowerSwitchOffClientRpc();
                        });
                    }
                    else
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_poweroff</size>");
                        });
                    }
                }

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        #endregion
        public static CrowdResponse Lightning(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;
                var tod = TimeOfDay.Instance;

                if (!StartOfRound.Instance.currentLevel.planetHasTime || StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse PlayHorn(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
           

            try
            {
                var playerRef = StartOfRound.Instance.localPlayerController;

                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
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

        public static CrowdResponse CreateWebs(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            //ISearchList results = UnityEngine.SearchService.Request($"p: t:prefab t:{typeof(SandSpiderAI).Name}", SearchFlags.Synchronous);
            //foreach (var result in results)

                try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }



        public static CrowdResponse Ghost(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            var playerRef = StartOfRound.Instance.localPlayerController;

            try
            {


                if (StartOfRound.Instance.timeSinceRoundStarted < 2f || !playerRef.playersManager.shipDoorsEnabled) status = CrowdResponse.Status.STATUS_RETRY;
                else
                {
                    TestMod.ActionQueue.Enqueue(() =>
                    {

                        DressGirlAI[] array = UnityEngine.Object.FindObjectsByType<DressGirlAI>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

                        if (array.Length == 0)
                        {
                            status = CrowdResponse.Status.STATUS_RETRY;
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
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse NightVision(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            var playerRef = StartOfRound.Instance.localPlayerController;
            if(TestMod.nightVision) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (BuffThread.isRunning(BuffType.NIGHT_VISION)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            
            new Thread(new BuffThread(req.GetReqID(), BuffType.NIGHT_VISION, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
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

        protected EnemyAI GetEnemyAIFromEnemyGameObject(GameObject enemyObj)
        {
            return enemyObj.GetComponentInChildren<EnemyAI>();
        }
    }
}
