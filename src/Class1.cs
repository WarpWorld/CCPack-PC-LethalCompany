using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using DunGen;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Security.AccessControl;
using GameNetcodeStuff;
using BepInEx.Configuration;
using System.Reflection;
using Unity.Netcode;
using static System.Net.Mime.MediaTypeNames;
using LethalCompanyTestMod.Component;
using Steamworks.Ugc;
using System.Threading;
using ControlValley;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using TerminalApi;
using TerminalApi.Classes;
using static TerminalApi.TerminalApi;
using TerminalApi.Events;
using static TerminalApi.Events.Events;

namespace LethalCompanyTestMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class TestMod : BaseUnityPlugin
    {
        // Mod Details
        private const string modGUID = "WarpWorld.CrowdControl";
        private const string modName = "Crowd Control";
        private const string modVersion = "1.1.7.0";

        public static string tsVersion = "1.1.7";
        public static Dictionary<string, (string name, string conn)> version = new Dictionary<string, (string name, string conn)>();

        private readonly Harmony harmony = new Harmony(modGUID);


        public static Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>> levelEnemySpawns;
        public static Dictionary<SpawnableEnemyWithRarity, int> enemyRaritys;
        public static Dictionary<SpawnableEnemyWithRarity, AnimationCurve> enemyPropCurves;
        public static ManualLogSource mls;

        public static SelectableLevel currentLevel;
        public static EnemyVent[] currentLevelVents;
        public static RoundManager currentRound;
        public static StartOfRound currentStart;
        // plan for more in the future
        private static SpawnableEnemyWithRarity jesterRef;

        public static bool noClipEnabled;
        public static bool enableGod;
        public static bool nightVision;
        public static bool infSprint;


        public static PlayerControllerB playerRef;
        public static bool speedHack;
        public static float nightVisionIntensity;
        public static float nightVisionRange;
        public static UnityEngine.Color nightVisionColor;

        private static bool hasGUISynced = false;
        internal static bool isHost = false;

        internal static TestMod Instance = null;
        private ControlClient client = null;

        public static bool test = false;
        public static uint msgid = 0;
        public static uint msgid2 = 0;
        public static uint msgid3 = 0;
        public static uint verwait = 0;

        public static uint floodtime = 0;
        void Awake()
        {

            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource("Crowd Control");
            // Plugin startup logic
            mls.LogInfo($"Loaded {modGUID}. Patching.");
            harmony.PatchAll(typeof(TestMod));

            mls.LogInfo($"Initializing Crowd Control");

            try
            {
                client = new ControlClient();
                new Thread(new ThreadStart(client.NetworkLoop)).Start();
                new Thread(new ThreadStart(client.RequestLoop)).Start();
            }
            catch (Exception e)
            {
                mls.LogInfo($"CC Init Error: {e.ToString()}");
            }

            mls.LogInfo($"Crowd Control Initialized");


            mls = Logger;
            enemyRaritys = new Dictionary<SpawnableEnemyWithRarity, int>();
            levelEnemySpawns = new Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>>();
            enemyPropCurves = new Dictionary<SpawnableEnemyWithRarity, AnimationCurve>();

            noClipEnabled = false;
            enableGod = false;
            infSprint = false;

            Events.TerminalBeginUsing += new TerminalEventHandler(OnBeginUsing);

        }

        private void OnBeginUsing(object sender, TerminalEventArgs e)
        {
            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_vercheck</size>");
        }

        private static string OnCCVersion()
        {
            string res = "Checking Crowd Control Versions...\n\n";


            foreach (var versionNum in version)
            {
                res += $"{versionNum.Key}: version {versionNum.Value.name} Live: {versionNum.Value.conn}\n";
            }
            return res;
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPrefix]
        static void startRound()
        {
            currentStart = StartOfRound.Instance;
        }

        [HarmonyPatch(typeof(RoundManager), "Start")]
        [HarmonyPrefix]
        static void setIsHost()
        {
            //mls.LogInfo("Host Status: " + RoundManager.Instance.NetworkManager.IsHost.ToString());
            isHost = RoundManager.Instance.NetworkManager.IsHost;

            verwait = 30;




            AddCommand("crowdcontrol", new CommandInfo
            {
                Category = "other",
                Description = "Checks crowd control version.",
                DisplayTextSupplier = OnCCVersion
            });

        }

        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        [HarmonyPrefix]
        static bool ModifyLevel(ref SelectableLevel newLevel)
        {
            // only called if you are host
            // avoid setting manually in case there is a missed path that executes even if not host
            //isHost = true;
            // doesn't need to be returned early as a result of above mentioned


            currentRound = RoundManager.Instance;
            if (!levelEnemySpawns.ContainsKey(newLevel))
            {
                List<SpawnableEnemyWithRarity> spawns = new List<SpawnableEnemyWithRarity>();
                foreach (var item in newLevel.Enemies)
                {
                    spawns.Add(item);
                }
                levelEnemySpawns.Add(newLevel, spawns);
            }
            List<SpawnableEnemyWithRarity> spawnableEnemies;
            levelEnemySpawns.TryGetValue(newLevel, out spawnableEnemies);
            newLevel.Enemies = spawnableEnemies;

            // make a dictionary of the inside enemy rarities
            foreach (var enemy in newLevel.Enemies)
            {
                if (!enemyRaritys.ContainsKey(enemy))
                {
                    enemyRaritys.Add(enemy, enemy.rarity);
                }
                int rare = 0;
                enemyRaritys.TryGetValue(enemy, out rare);
                enemy.rarity = rare;
            }

            // make a dictionary of the outside enemy rarities
            foreach (var enemy in newLevel.OutsideEnemies)
            {
                if (!enemyRaritys.ContainsKey(enemy))
                {
                    enemyRaritys.Add(enemy, enemy.rarity);
                }
                int rare = 0;
                enemyRaritys.TryGetValue(enemy, out rare);
                enemy.rarity = rare;
            }

            foreach (var enemy in newLevel.Enemies)
            {
                if (!enemyPropCurves.ContainsKey(enemy))
                {
                    enemyPropCurves.Add(enemy, enemy.enemyType.probabilityCurve);
                }
                AnimationCurve prob = new AnimationCurve();
                enemyPropCurves.TryGetValue(enemy, out prob);
                enemy.enemyType.probabilityCurve = prob;
            }
            HUDManager.Instance.AddTextToChatOnServer("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");




            return true;
        }

        [HarmonyPatch(typeof(RoundManager), "AdvanceHourAndSpawnNewBatchOfEnemies")]
        [HarmonyPrefix]
        static void updateCurrentLevelInfo(ref EnemyVent[] ___allEnemyVents, ref SelectableLevel ___currentLevel)
        {
            currentLevel = ___currentLevel;
            currentLevelVents = ___allEnemyVents;
        }


        public static Queue<Action> ActionQueue = new Queue<Action>();

        [HarmonyPatch(typeof(RoundManager), "Update")]
        [HarmonyPrefix]
        static void roundUpdate()
        {
            if (CrowdDelegates.givedelay > 0) CrowdDelegates.givedelay--;
            if (verwait > 0) verwait--;

            if (ActionQueue.Count > 0)
            {
                Action action = ActionQueue.Dequeue();
                action.Invoke();
            }

            lock (BuffThread.threads)
            {
                foreach (var thread in BuffThread.threads)
                {
                    if (!thread.paused)
                        thread.buff.tick();
                }
            }

        }

        static IEnumerator getVersions()
        {
            version.Clear();
            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_vercheck</size>");

            yield return new WaitForSeconds(0.5f);

            foreach (var versionNum in version)
            {
                mls.LogError($"{versionNum.Key} is running LC Crowd Control version {versionNum.Value}");
            }

        }

        static IEnumerator getTermVersions()
        {
            version.Clear();
            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_vercheck</size>");

            yield return new WaitForSeconds(0.5f);
        }




        [HarmonyPatch(typeof(HUDManager), "AddChatMessage")]
        [HarmonyPrefix]
        static bool CrowdControlCommands(HUDManager __instance, string chatMessage)
        {
            try
            {
                string text = chatMessage;

                //mls.LogError(chatMessage);

                if (chatMessage.ToLower() == "/ccversion" && isHost)
                {
                    if (verwait == 0)
                        __instance.StartCoroutine(getVersions());

                    return false;
                }



                if (!text.StartsWith("<size=0>")) return true;

                text = text.Replace("<size=0>", "");
                text = text.Replace("</size>", "");

                if (!text.StartsWith("/cc_")) return true;

                text = text.Replace("/cc_", "");

                //TestMod.mls.LogInfo(text);

                string[] values = text.Split('_');


                switch (values[0])
                {
                    case "version":
                        if (!(version.ContainsKey(values[1])))
                        {
                            version.Add(values[1], (values[2], values[3]));
                        }
                        return false;

                    case "vercheck":
                        {
                            if (verwait > 0) return false;
                            verwait = 30;
                            playerRef = StartOfRound.Instance.localPlayerController;

                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_version_{playerRef.playerUsername}_{tsVersion}_{ControlClient.connect}</size>");

                            return false;
                        }
                    case "spawn":
                        {
                            if (!isHost) return true;

                            //TestMod.mls.LogInfo($"client spawn received");

                            TestMod.ActionQueue.Enqueue(() =>
                            {
                                foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.OutsideEnemies)
                                {

                                    if (outsideEnemy.enemyType.enemyName.ToLower().Contains(values[1]))
                                    {
                                        try
                                        {
                                            //TestMod.mls.LogInfo($"client spawning {values[1]}");
                                            TestMod.SpawnEnemy(outsideEnemy, 1, false);

                                        }
                                        catch (Exception e)
                                        {

                                        }
                                        return;
                                    }
                                }
                                foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.Enemies)
                                {

                                    if (outsideEnemy.enemyType.enemyName.ToLower().Contains(values[1]))
                                    {
                                        try
                                        {
                                            //TestMod.mls.LogInfo($"client spawning {values[1]}");
                                            TestMod.SpawnEnemy(outsideEnemy, 1, false);

                                        }
                                        catch (Exception e)
                                        {

                                        }
                                        return;
                                    }
                                }
                            });


                            break;
                        }

                    case "cspawn":
                        {
                            if (!isHost) return true;

                            int id = int.Parse(values[2]);

                            PlayerControllerB player = null;

                            foreach (var playero in StartOfRound.Instance.allPlayerScripts)
                            {
                                if (playero != null && playero.isActiveAndEnabled && !playero.isPlayerDead && (int)playero.playerClientId == id && playero.isPlayerControlled)
                                    player = playero;
                            }

                            if (player == null) return true;


                            TestMod.ActionQueue.Enqueue(() =>
                            {
                                foreach (var outsideEnemy in StartOfRound.Instance.currentLevel.OutsideEnemies)
                                {

                                    if (values[1] == "mimic")
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
                                        enemy.mimickingPlayer = player;
                                        enemy.SetSuit(player.currentSuitID);
                                        enemy.SetEnemyOutside(!player.isInsideFactory);
                                        enemy.SetVisibilityOfMaskedEnemy();
                                        enemy.SetMaskType(component.maskTypeId);

                                        HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_mimic_{enemy.NetworkObject.NetworkObjectId}</size>");

                                        obj.gameObject.GetComponentInChildren<EnemyAI>().stunNormalizedTimer = 6.0f;
                                        UnityEngine.Object.Destroy(gameObject);
                                        return;
                                    }

                                    if (outsideEnemy.enemyType.enemyName.ToLower().Contains(values[1]))
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

                                    if (outsideEnemy.enemyType.enemyName.ToLower().Contains(values[1]))
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


                            break;
                        }

                    case "poweron":
                        if (!isHost) return true;
                        RoundManager.Instance.PowerSwitchOnClientRpc();
                        break;
                    case "poweroff":
                        if (!isHost) return true;
                        RoundManager.Instance.PowerSwitchOffClientRpc();
                        break;

                    case "addhour":
                        {
                            if (!isHost) return true;
                            var tod = TimeOfDay.Instance;
                            float num = tod.lengthOfHours;
                            tod.globalTime += num;
                            tod.timeUntilDeadline -= num;
                            CrowdDelegates.callFunc(tod, "MoveTimeOfDay", null);
                            break;
                        }
                    case "remhour":
                        {
                            if (!isHost) return true;
                            var tod = TimeOfDay.Instance;
                            float num = tod.lengthOfHours;
                            tod.globalTime -= num;
                            tod.timeUntilDeadline += num;
                            CrowdDelegates.callFunc(tod, "MoveTimeOfDay", null);
                            break;
                        }

                    case "take":

                        {
                            int id = int.Parse(values[1]);
                            playerRef = StartOfRound.Instance.localPlayerController;

                            if ((int)StartOfRound.Instance.localPlayerController.playerClientId != id) return true;

                            playerRef.DespawnHeldObject();
                            break;
                        }

                    case "damage":
                        {
                            int id = int.Parse(values[1]);
                            playerRef = StartOfRound.Instance.localPlayerController;

                            if ((int)StartOfRound.Instance.localPlayerController.playerClientId != id) return true;

                            int dmg = 25;
                            if (playerRef.health < 25) dmg = playerRef.health - 1;
                            playerRef.DamagePlayer(dmg);
                            break;
                        }
                    case "heal":
                        {
                            int id = int.Parse(values[1]);
                            playerRef = StartOfRound.Instance.localPlayerController;

                            if ((int)StartOfRound.Instance.localPlayerController.playerClientId != id) return true;

                            playerRef.health = Mathf.Clamp(playerRef.health + 25, 0, 100);

                            if (playerRef.health >= 20)
                            {
                                playerRef.MakeCriticallyInjured(false);
                            }

                            HUDManager.Instance.UpdateHealthUI(playerRef.health, true);
                            break;
                        }

                    case "landmine":
                        {

                            int id = int.Parse(values[1]);

                            PlayerControllerB player = null;

                            foreach (var playero in StartOfRound.Instance.allPlayerScripts)
                            {
                                if (playero != null && playero.isActiveAndEnabled && !playero.isPlayerDead && (int)playero.playerClientId == id && playero.isPlayerControlled)
                                    player = playero;
                            }

                            if (player == null) return true;

                            GameObject prefab = null;

                            GameObject[] array = Resources.FindObjectsOfTypeAll<GameObject>();
                            GameObject[] array2 = array;
                            foreach (GameObject val in array2)
                            {
                                if (((UnityEngine.Object)val).name == "Landmine")
                                {
                                    prefab = val;
                                    break;
                                }
                            }

                            if (prefab == null)
                                return true;

                            Vector3 pos = player.transform.position + player.transform.forward * 5.0f - player.transform.up * 0.5f;

                            Vector3 test = RoundManager.Instance.GetNavMeshPosition(pos, default(UnityEngine.AI.NavMeshHit), 5f, -1);
                            Vector3 dist = (test - pos);

                            if (dist.magnitude < 6.0f) pos = test;


                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, pos, Quaternion.Euler(-90, 0, 0), TestMod.currentStart.propsContainer);

                            break;
                        }

                    case "credits":

                        {
                            if (!isHost) return true;
                            int give = int.Parse(values[1]);
                            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

                            terminal.groupCredits += give;
                            terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);

                            if (give > 0)
                                HUDManager.Instance.DisplayTip("Crowd Control", "Crowd Control gave " + give + " credits");
                            else
                                HUDManager.Instance.DisplayTip("Crowd Control", "Crowd Control took " + (-1 * give) + " credits");

                            break;
                        }


                    case "giver":
                        {
                            if (!isHost) return true;

                            CrowdDelegates.givedelay = 20;

                            int give = int.Parse(values[1]);
                            int id = int.Parse(values[2]);
                            uint mid = uint.Parse(values[3]);

                            if (mid == msgid2) return true;

                            msgid2 = mid;

                            PlayerControllerB player = null;

                            if (id != -1)
                            {
                                foreach (var playero in StartOfRound.Instance.allPlayerScripts)
                                {
                                    if (playero != null && playero.isActiveAndEnabled && !playero.isPlayerDead && (int)playero.playerClientId == id && playero.isPlayerControlled)
                                        player = playero;
                                }
                                if (player == StartOfRound.Instance.localPlayerController) id = -1;
                            }
                            else player = StartOfRound.Instance.localPlayerController;

                            if (player == null)
                            {
                                player = StartOfRound.Instance.localPlayerController;
                                id = -1;
                            }



                            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(terminal.buyableItemsList[give].spawnPrefab, player.transform.position, Quaternion.identity, TestMod.currentStart.propsContainer);
                            gameObject.GetComponent<GrabbableObject>().fallTime = 0f;
                            gameObject.GetComponent<NetworkObject>().Spawn(false);

                            CrowdDelegates.msgid++;
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_give_{give}_{id}_{gameObject.GetComponent<NetworkObject>().NetworkObjectId}_{CrowdDelegates.msgid}</size>");

                            break;
                        }

                    case "mgiver":
                        {
                            if (!isHost) return true;

                            CrowdDelegates.givedelay = 20;

                            string give = values[1];
                            int id = int.Parse(values[2]);
                            uint mid = uint.Parse(values[3]);

                            if (mid == msgid2) return true;

                            msgid2 = mid;

                            PlayerControllerB player = null;

                            if (id != -1)
                            {
                                foreach (var playero in StartOfRound.Instance.allPlayerScripts)
                                {
                                    if (playero != null && playero.isActiveAndEnabled && !playero.isPlayerDead && (int)playero.playerClientId == id && playero.isPlayerControlled)
                                        player = playero;
                                }
                                if (player == StartOfRound.Instance.localPlayerController) id = -1;
                            }
                            else player = StartOfRound.Instance.localPlayerController;

                            if (player == null)
                            {
                                player = StartOfRound.Instance.localPlayerController;
                                id = -1;
                            }


                            GameObject prefab = null;

                            foreach (var level in StartOfRound.Instance.levels)
                            {
                                if (prefab == null)
                                    foreach (var spawn in level.spawnableScrap)
                                    {
                                        if (spawn.spawnableItem.name.ToLower() == give) prefab = spawn.spawnableItem.spawnPrefab;
                                    }
                            }

                            if (prefab == null)
                                return true;

                            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, player.transform.position, Quaternion.identity, TestMod.currentStart.propsContainer);
                            gameObject.GetComponent<GrabbableObject>().fallTime = 0f;
                            gameObject.GetComponent<NetworkObject>().Spawn(false);

                            CrowdDelegates.msgid++;
                            HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_mgive_{give}_{id}_{gameObject.GetComponent<NetworkObject>().NetworkObjectId}_{CrowdDelegates.msgid}</size>");

                            break;
                        }

                    case "mimic":
                        {
                            ulong gid = ulong.Parse(values[1]);
                            var gameObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[gid].gameObject;

                            var enemy = gameObject.GetComponent<MaskedPlayerEnemy>();
                            CrowdDelegates.setProperty(enemy, "enemyEnabled", true);
                            break;
                        }

                    case "give":

                        {
                            int give = int.Parse(values[1]);
                            int id = int.Parse(values[2]);
                            ulong gid = ulong.Parse(values[3]);
                            uint mid = uint.Parse(values[4]);

                            if (mid == msgid3) return true;

                            msgid3 = mid;

                            playerRef = StartOfRound.Instance.localPlayerController;

                            if (id == -1)
                            {
                                if (!isHost) return true;
                            }
                            else
                            {
                                if (isHost) return true;
                                if ((int)StartOfRound.Instance.localPlayerController.playerClientId != id) return true;
                            }

                            var gameObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[gid].gameObject;

                            var grab = gameObject.GetComponent<GrabbableObject>();

                            ControlValley.CrowdDelegates.setProperty(playerRef, "currentlyGrabbingObject", grab);
                            ControlValley.CrowdDelegates.setProperty(playerRef, "grabInvalidated", false);


                            NetworkObject networkObject = grab.NetworkObject;
                            if (networkObject == null || !networkObject.IsSpawned)
                            {
                                return true;
                            }
                            grab.InteractItem();


                            playerRef.playerBodyAnimator.SetBool("GrabInvalidated", false);
                            playerRef.playerBodyAnimator.SetBool("GrabValidated", false);
                            playerRef.playerBodyAnimator.SetBool("cancelHolding", false);
                            playerRef.playerBodyAnimator.ResetTrigger("Throw");

                            ControlValley.CrowdDelegates.callFunc(playerRef, "SetSpecialGrabAnimationBool", new System.Object[] { true, null });

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

                            ControlValley.CrowdDelegates.callFunc(playerRef, "GrabObjectServerRpc", new NetworkObjectReference(networkObject));

                            Coroutine goc = (Coroutine)ControlValley.CrowdDelegates.getProperty(playerRef, "grabObjectCoroutine");

                            if (goc != null)
                            {
                                ((UnityEngine.MonoBehaviour)playerRef).StopCoroutine(goc);
                            }

                            ControlValley.CrowdDelegates.setProperty(playerRef, "grabObjectCoroutine", ((UnityEngine.MonoBehaviour)playerRef).StartCoroutine("GrabObject"));
                        }
                        break;

                    case "mgive":

                        {
                            string give = values[1];
                            int id = int.Parse(values[2]);
                            ulong gid = ulong.Parse(values[3]);
                            uint mid = uint.Parse(values[4]);

                            if (mid == msgid3) return true;

                            msgid3 = mid;

                            playerRef = StartOfRound.Instance.localPlayerController;

                            if (id == -1)
                            {
                                if (!isHost) return true;
                            }
                            else
                            {
                                if (isHost) return true;
                                if ((int)StartOfRound.Instance.localPlayerController.playerClientId != id) return true;
                            }

                            var gameObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[gid].gameObject;

                            var grab = gameObject.GetComponent<GrabbableObject>();

                            ControlValley.CrowdDelegates.setProperty(playerRef, "currentlyGrabbingObject", grab);
                            ControlValley.CrowdDelegates.setProperty(playerRef, "grabInvalidated", false);


                            NetworkObject networkObject = grab.NetworkObject;
                            if (networkObject == null || !networkObject.IsSpawned)
                            {
                                return true;
                            }
                            grab.InteractItem();


                            playerRef.playerBodyAnimator.SetBool("GrabInvalidated", false);
                            playerRef.playerBodyAnimator.SetBool("GrabValidated", false);
                            playerRef.playerBodyAnimator.SetBool("cancelHolding", false);
                            playerRef.playerBodyAnimator.ResetTrigger("Throw");

                            ControlValley.CrowdDelegates.callFunc(playerRef, "SetSpecialGrabAnimationBool", new System.Object[] { true, null });

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

                            ControlValley.CrowdDelegates.callFunc(playerRef, "GrabObjectServerRpc", new NetworkObjectReference(networkObject));

                            Coroutine goc = (Coroutine)ControlValley.CrowdDelegates.getProperty(playerRef, "grabObjectCoroutine");

                            if (goc != null)
                            {
                                ((UnityEngine.MonoBehaviour)playerRef).StopCoroutine(goc);
                            }

                            ControlValley.CrowdDelegates.setProperty(playerRef, "grabObjectCoroutine", ((UnityEngine.MonoBehaviour)playerRef).StartCoroutine("GrabObject"));
                        }
                        break;

                    case "ship":
                        {
                            int cur = int.Parse(values[1]);
                            if ((int)StartOfRound.Instance.localPlayerController.playerClientId == cur)
                            {
                                StartOfRound.Instance.ForcePlayerIntoShip();
                            }
                        }
                        break;
                    case "kill":
                        {
                            int cur = int.Parse(values[1]);
                            foreach (var player in StartOfRound.Instance.allPlayerScripts)
                            {
                                if (player != null && player.isActiveAndEnabled && !player.isPlayerDead && (int)player.playerClientId == cur && player.isPlayerControlled)
                                    player.KillPlayer(player.transform.up * 100.0f, true, CauseOfDeath.Gravity, 2);
                            }
                        }
                        break;
                    case "pitch":
                        {
                            float cur = float.Parse(values[1]);

                            for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
                            {
                                try
                                {
                                    //TestMod.mls.LogInfo($"trying pitch {i}");


                                    //TestMod.mls.LogInfo($"pitch target {SoundManager.Instance.playerVoicePitchTargets[i]}");

                                    SoundManager.Instance.playerVoicePitchTargets[i] = cur;

                                    //TestMod.mls.LogInfo($"pitch value {SoundManager.Instance.playerVoicePitches[i]}");

                                    SoundManager.Instance.playerVoicePitches[i] = cur;

                                    if (StartOfRound.Instance.allPlayerScripts[i] != null)
                                    {
                                        if (StartOfRound.Instance.allPlayerScripts[i].currentVoiceChatAudioSource != null)
                                            StartOfRound.Instance.allPlayerScripts[i].currentVoiceChatAudioSource.pitch = cur;
                                        SoundManager.Instance.SetPlayerPitch(cur, i);
                                        //TestMod.mls.LogInfo($"set pitch {i} to {cur}");
                                    }
                                }
                                catch (Exception e)
                                {
                                    TestMod.mls.LogError(e.ToString());
                                }
                            }
                        }
                        break;

                    case "tele":
                        {
                            int cur = int.Parse(values[1]);
                            int dest = int.Parse(values[2]);

                            if ((int)StartOfRound.Instance.localPlayerController.playerClientId == cur)
                            {

                                foreach (var player in StartOfRound.Instance.allPlayerScripts)
                                {
                                    if (player != null && player.isActiveAndEnabled && !player.isPlayerDead && (int)player.playerClientId == dest && player.isPlayerControlled)
                                        StartOfRound.Instance.localPlayerController.TeleportPlayer(player.transform.position);
                                }
                            }
                        }
                        break;

                    case "body":
                        {
                            int cur = int.Parse(values[1]);
                            uint id = uint.Parse(values[2]);

                            if (id == msgid) return true;

                            msgid = id;

                            foreach (var player in StartOfRound.Instance.allPlayerScripts)
                            {
                                if (player != null && player.isActiveAndEnabled && !player.isPlayerDead && (int)player.playerClientId == cur && player.isPlayerControlled)
                                    player.SpawnDeadBody((int)player.playerClientId, player.transform.up * 2.0f + player.transform.forward * 5.0f, 0, player);
                            }
                        }
                        break;
                    case "deadline":
                        {
                            float cur = float.Parse(values[1]);

                            TimeOfDay.Instance.timeUntilDeadline = cur;
                            TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
                            HUDManager.Instance.DisplayDaysLeft((int)Mathf.Floor(TimeOfDay.Instance.timeUntilDeadline / TimeOfDay.Instance.totalTime));
                            UnityEngine.Object.FindObjectOfType<Terminal>().SetItemSales();
                        }
                        break;
                    case "revive":
                        StartOfRound.Instance.ReviveDeadPlayers();
                        HUDManager.Instance.HideHUD(false);
                        break;
                    case "doors":
                        {
                            int cur = int.Parse(values[1]);

                            HangarShipDoor hangarShipDoor = UnityEngine.Object.FindObjectOfType<HangarShipDoor>();
                            hangarShipDoor.PlayDoorAnimation(cur == 1);
                        }
                        break;
                    case "quota":
                        {
                            int cur = int.Parse(values[1]);
                            int max = int.Parse(values[2]);

                            TimeOfDay.Instance.quotaFulfilled = cur;
                            TimeOfDay.Instance.profitQuota = max;

                            StartOfRound.Instance.profitQuotaMonitorText.text = string.Format("PROFIT QUOTA:\n${0} / ${1}", cur, max);
                        }
                        break;
                    case "weather":
                        {
                            int give = int.Parse(values[1]);

                            TimeOfDay.Instance.DisableAllWeather(true);

                            WeatherEffect wo;


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
                            StartOfRound.Instance.currentLevel.currentWeather = (LevelWeatherType)give;

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
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                TestMod.mls.LogError(ex.ToString());
            }

            return false;

        }


        public static void SpawnEnemyWithConfigManager(string enemyName)
        {
            if (!isHost) { return; }
            //mls.LogInfo("CFGMGR tried to spawn an enemy");
            bool foundEnemy = false;
            string foundEnemyName = "";
            foreach (var enemy in currentLevel.Enemies)
            {
                //mls.LogInfo($"inside enemy: {enemy.enemyType.enemyName}");
                // jester
                // lasso
                // spider
                // centipede
                // blob
                // flowerman
                // spring
                // puffer

                if (enemy.enemyType.enemyName.ToLower().Contains(enemyName.ToLower()))
                {
                    try
                    {
                        foundEnemy = true;
                        foundEnemyName = enemy.enemyType.enemyName;
                        SpawnEnemy(enemy, 1, true);
                        //mls.LogInfo("Spawned " + enemy.enemyType.enemyName);
                    }
                    catch
                    {
                        //mls.LogInfo("Could not spawn enemy");
                    }
                    break;
                }
            }
            if (!foundEnemy)
            {
                foreach (var outsideEnemy in currentLevel.OutsideEnemies)
                {
                    mls.LogInfo($"outside enemy: {outsideEnemy.enemyType.enemyName}");
                    if (outsideEnemy.enemyType.enemyName.ToLower().Contains(enemyName.ToLower()))
                    {
                        try
                        {
                            foundEnemy = true;
                            foundEnemyName = outsideEnemy.enemyType.enemyName;
                            //mls.LogInfo(outsideEnemy.enemyType.enemyName);

                            //random ai node index Random.Range(0, GameObject.FindGameObjectsWithTag("OutsideAINode").Length) - 1

                            //mls.LogInfo("The index of " + outsideEnemy.enemyType.enemyName + " is " + currentLevel.OutsideEnemies.IndexOf(outsideEnemy));

                            SpawnEnemy(outsideEnemy, 1, false);


                            //mls.LogInfo("Spawned " + outsideEnemy.enemyType.enemyName);
                        }
                        catch (Exception e)
                        {
                            //mls.LogInfo("Could not spawn enemy");
                            mls.LogInfo("The game tossed an error: " + e.Message);
                        }
                        break;
                    }
                }

            }
        }

        public static void SpawnEnemy(SpawnableEnemyWithRarity enemy, int amount, bool inside)
        {
            var playerRef = StartOfRound.Instance.localPlayerController;

            GameObject obj = UnityEngine.Object.Instantiate(enemy.enemyType.enemyPrefab, playerRef.transform.position + playerRef.transform.forward * 5.0f, Quaternion.Euler(Vector3.zero));
            obj.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);

            obj.gameObject.GetComponentInChildren<EnemyAI>().stunNormalizedTimer = 6.0f;

            return;


        }

        public static void SpawnCrewEnemy(PlayerControllerB player, SpawnableEnemyWithRarity enemy, int amount, bool inside)
        {

            GameObject obj = UnityEngine.Object.Instantiate(enemy.enemyType.enemyPrefab, player.transform.position + player.transform.forward * 5.0f, Quaternion.Euler(Vector3.zero));
            obj.gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);

            obj.gameObject.GetComponentInChildren<EnemyAI>().stunNormalizedTimer = 6.0f;

            return;


        }



    }

}
