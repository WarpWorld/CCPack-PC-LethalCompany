//this project is a retrofit, it should NOT be used as part of any example - kat
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using ConnectorLib.JSON;
using Newtonsoft.Json;
using UnityEngine;
using Object = System.Object;

namespace BepinControl;

[SuppressMessage("ReSharper", "GrammarMistakeInComment")]
public class ControlClient
{
    public static readonly string CV_HOST = "127.0.0.1";
    public static readonly int CV_PORT = 51337;

    private static readonly string[] CommonMetadata = new string[] { "health" };

    private static readonly Dictionary<string, MetadataDelegate> Metadata = new()
    {
        //{ "health", MetadataDelegates.Health }
    };

    private static readonly Dictionary<string, EffectDelegate> Delegate = new()
    {
        { "heal_full", EffectDelegates.HealFull },
        { "kill", EffectDelegates.Kill },
        { "killcrew", EffectDelegates.KillCrewmate },
        { "damage", EffectDelegates.Damage },
        { "damagecrew", EffectDelegates.DamageCrew },
        { "heal", EffectDelegates.Heal },
        { "healcrew", EffectDelegates.HealCrew },

        { "launch", EffectDelegates.Launch },
        { "fast", EffectDelegates.FastMove },
        { "slow", EffectDelegates.SlowMove },
        { "hyper", EffectDelegates.HyperMove },
        { "freeze", EffectDelegates.Freeze },
        { "drunk", EffectDelegates.Drunk },


        { "jumpultra", EffectDelegates.UltraJump },
        { "jumphigh", EffectDelegates.HighJump },
        { "jumplow", EffectDelegates.LowJump },

        { "ohko", EffectDelegates.OHKO },
        { "invul", EffectDelegates.Invul },
        { "drain", EffectDelegates.DrainStamins },
        { "restore", EffectDelegates.RestoreStamins },
        { "infstam", EffectDelegates.InfiniteStamina },
        { "nostam", EffectDelegates.NoStamina },

        { "spawn_pede", EffectDelegates.Spawn },
        { "spawn_spider", EffectDelegates.Spawn },
        { "spawn_hoard", EffectDelegates.Spawn },
        { "spawn_flower", EffectDelegates.Spawn },
        { "spawn_crawl", EffectDelegates.Spawn },
        { "spawn_blob", EffectDelegates.Spawn },
        { "spawn_spring", EffectDelegates.Spawn },
        { "spawn_puff", EffectDelegates.Spawn },
        { "spawn_dog", EffectDelegates.Spawn },
        { "spawn_giant", EffectDelegates.Spawn },
        { "spawn_levi", EffectDelegates.Spawn },
        { "spawn_hawk", EffectDelegates.Spawn },
        { "spawn_girl", EffectDelegates.Spawn },
        { "spawn_mimic", EffectDelegates.Spawn },
        { "spawn_cracker", EffectDelegates.Spawn },
        { "spawn_landmine", EffectDelegates.Spawn },
        { "webs", EffectDelegates.CreateWebs },
        { "killenemies", EffectDelegates.KillEnemies },
        { "spawn_radmech", EffectDelegates.Spawn },
        { "spawn_clay", EffectDelegates.Spawn },
        { "spawn_butler", EffectDelegates.Spawn },

        { "cspawn_pede", EffectDelegates.CrewSpawn },
        { "cspawn_spider", EffectDelegates.CrewSpawn },
        { "cspawn_hoard", EffectDelegates.CrewSpawn },
        { "cspawn_flower", EffectDelegates.CrewSpawn },
        { "cspawn_crawl", EffectDelegates.CrewSpawn },
        { "cspawn_blob", EffectDelegates.CrewSpawn },
        { "cspawn_spring", EffectDelegates.CrewSpawn },
        { "cspawn_puff", EffectDelegates.CrewSpawn },
        { "cspawn_dog", EffectDelegates.CrewSpawn },
        { "cspawn_giant", EffectDelegates.CrewSpawn },
        { "cspawn_levi", EffectDelegates.CrewSpawn },
        { "cspawn_hawk", EffectDelegates.CrewSpawn },
        { "cspawn_girl", EffectDelegates.CrewSpawn },
        { "cspawn_cracker", EffectDelegates.CrewSpawn },
        { "cspawn_mimic", EffectDelegates.CrewSpawn },
        { "cspawn_landmine", EffectDelegates.CrewSpawn },
        { "cspawn_radmech", EffectDelegates.CrewSpawn },
        { "cspawn_butler", EffectDelegates.CrewSpawn },

        { "give_binoculars", EffectDelegates.GiveItem }, //binoculars
        { "give_boombox", EffectDelegates.GiveItem }, //boombox
        { "give_flashlight", EffectDelegates.GiveItem }, //flashlight
        { "give_jetpack", EffectDelegates.GiveItem }, //jetpack
        { "give_key", EffectDelegates.GiveItem }, //Key
        { "give_lockpicker", EffectDelegates.GiveItem }, //Lockpicker
        { "give_lungapparatus", EffectDelegates.GiveItem }, //Apparatus
        { "give_mapdevice", EffectDelegates.GiveItem }, //Mapper
        { "give_proflashlight", EffectDelegates.GiveItem }, //Pro-Flashlight
        { "give_shovel", EffectDelegates.GiveItem }, //Shovel
        { "give_stungrenade", EffectDelegates.GiveItem }, //Stun Grenade
        { "give_extensionladder", EffectDelegates.GiveItem }, //Extension Ladder
        { "give_tzpinhalant", EffectDelegates.GiveItem }, //TZP Inhalant
        { "give_walkietalkie", EffectDelegates.GiveItem }, //Walkie Talkie
        { "give_zapgun", EffectDelegates.GiveItem }, //Zap Gun
        { "give_7ball", EffectDelegates.GiveItem }, //Magic 7 Ball
        { "give_airhorn", EffectDelegates.GiveItem }, //Airhorn
        { "give_bottlebin", EffectDelegates.GiveItem }, //Bottles
        { "give_clownhorn", EffectDelegates.GiveItem }, //Clown Horn
        { "give_goldbar", EffectDelegates.GiveItem }, //Gold Bar
        { "give_stopsign", EffectDelegates.GiveItem }, //Stop Sign
        { "give_radarbooster", EffectDelegates.GiveItem }, //Radar Booster
        { "give_yieldsign", EffectDelegates.GiveItem }, //Yield Sign
        { "give_shotgun", EffectDelegates.GiveItem }, //Shotgun
        { "give_gunAmmo", EffectDelegates.GiveItem }, //Ammo
        { "give_spraypaint", EffectDelegates.GiveItem }, //Spraypaint
        { "give_giftbox", EffectDelegates.GiveItem }, //Gift Box
        { "give_tragedymask", EffectDelegates.GiveItem }, //Tragedy Mask
        { "give_comedymask", EffectDelegates.GiveItem }, //Comedy Mask
        { "give_knife", EffectDelegates.GiveItem }, //Kitchen Knife
        { "give_easteregg", EffectDelegates.GiveItem }, //Easter Egg
        { "give_weedkiller", EffectDelegates.GiveItem }, //Weed Killer

        { "cgive_binoculars", EffectDelegates.GiveCrewItem }, //binoculars
        { "cgive_boombox", EffectDelegates.GiveCrewItem }, //boombox
        { "cgive_flashlight", EffectDelegates.GiveCrewItem }, //flashlight
        { "cgive_jetpack", EffectDelegates.GiveCrewItem }, //jetpack
        { "cgive_key", EffectDelegates.GiveCrewItem }, //Key
        { "cgive_lockpicker", EffectDelegates.GiveCrewItem }, //Lockpicker
        { "cgive_lungapparatus", EffectDelegates.GiveCrewItem }, //Apparatus
        { "cgive_mapdevice", EffectDelegates.GiveCrewItem }, //Mapper
        { "cgive_proflashlight", EffectDelegates.GiveCrewItem }, //Pro-Flashlight
        { "cgive_shovel", EffectDelegates.GiveCrewItem }, //Shovel
        { "cgive_stungrenade", EffectDelegates.GiveCrewItem }, //Stun Grenade
        { "cgive_extensionladder", EffectDelegates.GiveCrewItem }, //Extension Ladder
        { "cgive_tzpinhalant", EffectDelegates.GiveCrewItem }, //TZP Inhalant
        { "cgive_walkietalkie", EffectDelegates.GiveCrewItem }, //Walkie Talkie
        { "cgive_zapgun", EffectDelegates.GiveCrewItem }, //Zap Gun
        { "cgive_7ball", EffectDelegates.GiveCrewItem }, //Magic 7 Ball
        { "cgive_airhorn", EffectDelegates.GiveCrewItem }, //Airhorn
        { "cgive_bottlebin", EffectDelegates.GiveCrewItem }, //Bottles
        { "cgive_clownhorn", EffectDelegates.GiveCrewItem }, //Clown Horn
        { "cgive_goldbar", EffectDelegates.GiveCrewItem }, //Gold Bar
        { "cgive_stopsign", EffectDelegates.GiveCrewItem }, //Stop Sign
        { "cgive_radarbooster", EffectDelegates.GiveCrewItem }, //Radar Booster
        { "cgive_yieldsign", EffectDelegates.GiveCrewItem }, //Yield Sign
        { "cgive_shotgun", EffectDelegates.GiveCrewItem }, //Shotgun
        { "cgive_gunAmmo", EffectDelegates.GiveCrewItem }, //Ammo
        { "cgive_spraypaint", EffectDelegates.GiveCrewItem }, //Spraypaint
        { "cgive_giftbox", EffectDelegates.GiveCrewItem }, //Gift Box
        { "cgive_tragedymask", EffectDelegates.GiveCrewItem }, //Tragedy Mask
        { "cgive_comedymask", EffectDelegates.GiveCrewItem }, //Comedy Mask
        { "cgive_knife", EffectDelegates.GiveCrewItem }, //Kitchen Knife
        { "cgive_easteregg", EffectDelegates.GiveCrewItem }, //Easter Egg
        { "cgive_weedkiller", EffectDelegates.GiveCrewItem }, //Weed Killer

        { "weather_-1", EffectDelegates.Weather },
        { "weather_1", EffectDelegates.Weather },
        { "weather_2", EffectDelegates.Weather },
        { "weather_3", EffectDelegates.Weather },
        { "weather_4", EffectDelegates.Weather },
        { "weather_5", EffectDelegates.Weather },
        { "weather_6", EffectDelegates.Weather },
        { "lightning", EffectDelegates.Lightning },

        { "takeitem", EffectDelegates.TakeItem },
        { "dropitem", EffectDelegates.DropItem },
        { "takecrewitem", EffectDelegates.TakeCrewItem },

        { "buy_walkie", EffectDelegates.BuyItem },
        { "buy_flashlight", EffectDelegates.BuyItem },
        { "buy_shovel", EffectDelegates.BuyItem },
        { "buy_lockpicker", EffectDelegates.BuyItem },
        { "buy_proflashlight", EffectDelegates.BuyItem },
        { "buy_stungrenade", EffectDelegates.BuyItem },
        { "buy_boombox", EffectDelegates.BuyItem },
        { "buy_inhaler", EffectDelegates.BuyItem },
        { "buy_stungun", EffectDelegates.BuyItem },
        { "buy_jetpack", EffectDelegates.BuyItem },
        { "buy_extensionladder", EffectDelegates.BuyItem },
        { "buy_radarbooster", EffectDelegates.BuyItem },
        { "buy_spraypaint", EffectDelegates.BuyItem },
        { "buy_weedkiller", EffectDelegates.BuyItem },

        { "buy_cruiser", EffectDelegates.BuyCruiser },
        { "turn_off_engine", EffectDelegates.TurnOffEngine },
        { "destroy_vehicle", EffectDelegates.DestroyVehicle },
        { "start_vehicle", EffectDelegates.TurnOnVehicle },
        { "spring_chair", EffectDelegates.SpringChair },

        { "charge", EffectDelegates.ChargeItem },
        { "uncharge", EffectDelegates.UnchargeItem },

        { "breakerson", EffectDelegates.BreakersOn },
        { "breakersoff", EffectDelegates.BreakersOff },

        { "toship", EffectDelegates.TeleportToShip },
        { "crewship", EffectDelegates.TeleportCrewToShip },
        { "body", EffectDelegates.SpawnBody },
        { "crewbody", EffectDelegates.SpawnCrewBody },
        { "nightvision", EffectDelegates.NightVision },
        { "revive", EffectDelegates.Revive },
        { "tocrew", EffectDelegates.TeleportToCrew },
        { "crewto", EffectDelegates.TeleportCrewTo },

        { "screech", EffectDelegates.Screech },
        { "footstep", EffectDelegates.Footstep },
        { "breathing", EffectDelegates.Breathing },
        { "ghost", EffectDelegates.Ghost },
        { "horn", EffectDelegates.PlayHorn },
        { "blob", EffectDelegates.BlobSound },
        { "highpitch", EffectDelegates.HighPitch },
        { "lowpitch", EffectDelegates.LowPitch },

        { "addhour", EffectDelegates.AddHour },
        { "remhour", EffectDelegates.RemoveHour },
        { "addday", EffectDelegates.AddDay },
        { "remday", EffectDelegates.RemoveDay },

        { "givecred_5", EffectDelegates.AddCredits },
        { "givecred_50", EffectDelegates.AddCredits },
        { "givecred_500", EffectDelegates.AddCredits },
        { "givecred_-5", EffectDelegates.AddCredits },
        { "givecred_-50", EffectDelegates.AddCredits },
        { "givecred_-500", EffectDelegates.AddCredits },

        { "givequota_5", EffectDelegates.AddQuota },
        { "givequota_50", EffectDelegates.AddQuota },
        { "givequota_500", EffectDelegates.AddQuota },
        { "givequota_-5", EffectDelegates.AddQuota },
        { "givequota_-50", EffectDelegates.AddQuota },
        { "givequota_-500", EffectDelegates.AddQuota },

        { "giveprofit_25", EffectDelegates.AddProfit },
        { "giveprofit_50", EffectDelegates.AddProfit },
        { "giveprofit_100", EffectDelegates.AddProfit },
        { "giveprofit_-25", EffectDelegates.AddProfit },
        { "giveprofit_-50", EffectDelegates.AddProfit },
        { "giveprofit_-100", EffectDelegates.AddProfit },
        { "addscrap", EffectDelegates.AddScrap },

        { "shipleave", EffectDelegates.ShipLeave },
        { "opendoors", EffectDelegates.OpenDoors },
        { "closedoors", EffectDelegates.CloseDoors },
    };

    private IPEndPoint Endpoint { get; }
    private Queue<SimpleJSONRequest> Requests { get; }
    private bool Running { get; set; }

    private bool paused;
    public static Socket? Socket { get; set; }

    public bool inGame = true;

    public static readonly ControlClient Instance = new();

    public bool Connected => Socket?.Connected ?? false;

    private ControlClient()
    {
        Endpoint = new(IPAddress.Parse(CV_HOST), CV_PORT);
        Requests = new();
        Running = true;
        Socket = null;
    }

    public bool isReady()
    {
        try
        {
            //TestMod.mls.LogInfo($"landed: {StartOfRound.Instance.shipHasLanded}");
            //TestMod.mls.LogInfo($"planet: {RoundManager.Instance.currentLevel.PlanetName}");

            if (!StartOfRound.Instance.shipHasLanded) return false;

            if (RoundManager.Instance.currentLevel.PlanetName.ToLower().Contains("gordion")) return false;
            if (RoundManager.Instance.currentLevel.PlanetName.ToLower().Contains("company")) return false;
        }
        catch (Exception e)
        {
            Mod.mls.LogError(e.ToString());
            return false;
        }

        return true;
    }

    public bool HideEffect(string code)
        => Send(new EffectUpdate(code, EffectStatus.NotVisible));

    public bool ShowEffect(string code)
        => Send(new EffectUpdate(code, EffectStatus.Visible));

    public bool DisableEffect(string code)
        => Send(new EffectUpdate(code, EffectStatus.NotSelectable));

    public bool EnableEffect(string code)
        => Send(new EffectUpdate(code, EffectStatus.Selectable));

    private void ClientLoop()
    {

        Mod.mls.LogInfo("Connected to Crowd Control");

        var timer = new Timer(timeUpdate, null, 0, 200);

        try
        {
            while (Running)
            {
                SimpleJSONRequest? req = Recieve(this, Socket);
                if (req?.IsKeepAlive ?? true)
                {
                    Thread.Sleep(0); //prevent a meltdown if this ever tight loops
                    continue;
                }

                lock (Requests)
                    Requests.Enqueue(req);
            }
        }
        catch (Exception)
        {
            Mod.mls.LogInfo("Disconnected from Crowd Control");
            Socket?.Close();
        }
    }

    public static readonly int RECV_BUF = 4096;
    public static readonly int RECV_TIME = 5000000;

    public SimpleJSONRequest? Recieve(ControlClient client, Socket socket)
    {
        byte[] buf = new byte[RECV_BUF];
        string content = "";
        int read = 0;

        do
        {
            if (!client.IsRunning()) return null;

            if (socket.Poll(RECV_TIME, SelectMode.SelectRead))
            {
                read = socket.Receive(buf);
                if (read < 0) return null;

                content += Encoding.ASCII.GetString(buf);
            }
            else
                KeepAlive();
        } while (read == 0 || (read == RECV_BUF && buf[RECV_BUF - 1] != 0));

        return SimpleJSONRequest.TryParse(content, out SimpleJSONRequest? result) ? result : null;
    }

    private static readonly EmptyResponse KEEPALIVE = new() { type = ResponseType.KeepAlive };

    public bool KeepAlive() => Send(KEEPALIVE);

    public bool Send(SimpleJSONResponse message)
    {
        byte[] tmpData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));
        byte[] outData = new byte[tmpData.Length + 1];
        Buffer.BlockCopy(tmpData, 0, outData, 0, tmpData.Length);
        outData[tmpData.Length] = 0;
        int bytesSent = Socket?.Send(outData) ?? -1;
        return (bytesSent == outData.Length);
    }

    public void timeUpdate(Object state)
    {
        inGame = true;

        if (!isReady()) inGame = false;

        if (!inGame)
        {
            TimedThread.addTime(200);
            paused = true;
        }
        else if (paused)
        {
            paused = false;
            TimedThread.unPause();
            TimedThread.tickTime(200);
        }
        else
        {
            TimedThread.tickTime(200);
        }
    }

    public bool IsRunning() => Running;

    public void NetworkLoop()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        while (Running)
        {
            Mod.mls.LogInfo("Attempting to connect to Crowd Control");

            try
            {
                Socket = new Socket(Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                if (Socket.BeginConnect(Endpoint, null, null).AsyncWaitHandle.WaitOne(10000, true) && Socket.Connected)
                    ClientLoop();
                else
                    Mod.mls.LogInfo("Failed to connect to Crowd Control");
                Socket.Close();
            }
            catch (Exception e)
            {
                Mod.mls.LogInfo(e.GetType().Name);
                Mod.mls.LogInfo("Failed to connect to Crowd Control");
            }

            Thread.Sleep(10000);
        }
    }

    public void FixedUpdate()
    {
        //Log.Message(_game_status_update_timer);
        _game_status_update_timer += Time.fixedDeltaTime;
        if (_game_status_update_timer >= GAME_STATUS_UPDATE_INTERVAL)
        {
            UpdateGameState();
            _game_status_update_timer = 0f;
        }
    }

    public void RequestLoop()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        while (Running)
        {
            try
            {
                SimpleJSONRequest req;
                lock (Requests)
                {
                    if (Requests.Count == 0)
                        continue;
                    req = Requests.Dequeue();
                }

                if (req is EffectRequest er)
                {
                    string? code = er.code;

                    try
                    {
                        EffectResponse? res;
                        if (code == null)
                            res = new EffectResponse(er.ID, EffectStatus.Unavailable, "No effect code was sent.");
                        else if (isReady())
                            if (Delegate.TryGetValue(code, out EffectDelegate? del))
                            {
                                res = del(this, er);

                                //we add the common metadata here, effects COULD return more meta
                                //on a per-effect basis but we're not doing that in this example
                                res.metadata = new();
                                foreach (string key in CommonMetadata)
                                    res.metadata.Add(key, Metadata[key].Invoke(this));
                            }
                            else
                                res = new EffectResponse(er.ID, EffectStatus.Unavailable,
                                    $"Unknown effect code: {code}");
                        else
                            res = new EffectResponse(er.ID, EffectStatus.Retry);

                        Send(res);
                    }
                    catch (KeyNotFoundException)
                    {
                        Send(new EffectResponse(er.ID, EffectStatus.Unavailable, $"Request error for '{code}'"));
                    }
                }
                else if (req.type == RequestType.GameUpdate) UpdateGameState(true);
            }
            catch (Exception)
            {
                Mod.mls.LogInfo("Disconnected from Crowd Control");
                Socket?.Close();
            }
        }
    }

    public void Stop() => Running = false;

    private GameState? _last_game_state;

    private const float GAME_STATUS_UPDATE_INTERVAL = 1f;
    private float _game_status_update_timer = 0f;

    [SuppressMessage("ReSharper", "RedundantIfElseBlock")]
    private bool UpdateGameState(bool force = false)
    {
        try
        {
            if (!StartOfRound.Instance.shipHasLanded) return UpdateGameState(GameState.WrongMode, force);

            if (RoundManager.Instance.currentLevel.PlanetName.ToLower().Contains("gordion")) UpdateGameState(GameState.SafeArea, force); ;
            if (RoundManager.Instance.currentLevel.PlanetName.ToLower().Contains("company")) UpdateGameState(GameState.SafeArea, force);
        }
        catch (Exception e)
        {
            Mod.mls.LogError(e.ToString());
            return UpdateGameState(GameState.Error, force);
        }

        return UpdateGameState(GameState.Ready, force);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool UpdateGameState(GameState newState, bool force) => UpdateGameState(newState, null, force);
    private bool UpdateGameState(GameState newState, string? message = null, bool force = false)
    {
        if (force || (_last_game_state != newState))
        {
            _last_game_state = newState;
            return Send(new GameUpdate(newState, message));
        }
        return true;
    }
}