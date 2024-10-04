/*
 * ControlValley
 * Stardew Valley Support for Twitch Crowd Control
 * Copyright (C) 2021 TerribleTable
 * LGPL v2.1
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using LethalCompanyTestMod;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;


namespace ControlValley
{
    public class ControlClient
    {
        public static readonly string CV_HOST = "127.0.0.1";
        public static readonly int CV_PORT = 51338;

        private Dictionary<string, CrowdDelegate> Delegate { get; set; }
        private IPEndPoint Endpoint { get; set; }
        private Queue<CrowdRequest> Requests { get; set; }
        private bool Running { get; set; }
        private bool Saving { get; set; }
        private bool Spawn { get; set; }

        private bool paused = false;
        public static Socket Socket { get; set; }

        public bool inGame = true;
        public static bool connect = false;
        public ControlClient()
        {
            Endpoint = new IPEndPoint(IPAddress.Parse(CV_HOST), CV_PORT);
            Requests = new Queue<CrowdRequest>();
            Running = true;
            Saving = false;
            Spawn = true;
            Socket = null;
            connect = false;

            Delegate = new Dictionary<string, CrowdDelegate>()
            {

                {"heal_full", CrowdDelegates.HealFull},
                {"kill", CrowdDelegates.Kill},
                {"killcrew", CrowdDelegates.KillCrewmate},
                {"damage", CrowdDelegates.Damage},
                {"damagecrew", CrowdDelegates.DamageCrew},
                {"heal", CrowdDelegates.Heal},
                {"healcrew", CrowdDelegates.HealCrew},

                {"launch", CrowdDelegates.Launch},
                {"fast", CrowdDelegates.FastMove},
                {"slow", CrowdDelegates.SlowMove},
                {"hyper", CrowdDelegates.HyperMove},
                {"freeze", CrowdDelegates.Freeze},
                {"drunk", CrowdDelegates.Drunk},


                {"jumpultra", CrowdDelegates.UltraJump},
                {"jumphigh", CrowdDelegates.HighJump},
                {"jumplow", CrowdDelegates.LowJump},

                {"ohko", CrowdDelegates.OHKO},
                {"invul", CrowdDelegates.Invul},
                {"drain", CrowdDelegates.DrainStamins},
                {"restore", CrowdDelegates.RestoreStamins},
                {"infstam", CrowdDelegates.InfiniteStamina},
                {"nostam", CrowdDelegates.NoStamina},

                {"spawn_pede", CrowdDelegates.Spawn},
                {"spawn_spider", CrowdDelegates.Spawn},
                {"spawn_hoard", CrowdDelegates.Spawn},
                {"spawn_flower", CrowdDelegates.Spawn},
                {"spawn_crawl", CrowdDelegates.Spawn},
                {"spawn_blob", CrowdDelegates.Spawn},
                {"spawn_spring", CrowdDelegates.Spawn},
                {"spawn_puff", CrowdDelegates.Spawn},
                {"spawn_dog", CrowdDelegates.Spawn},
                {"spawn_giant", CrowdDelegates.Spawn},
                {"spawn_levi", CrowdDelegates.Spawn},
                {"spawn_hawk", CrowdDelegates.Spawn},
                {"spawn_girl", CrowdDelegates.Spawn},
                {"spawn_mimic", CrowdDelegates.Spawn},
                {"spawn_cracker", CrowdDelegates.Spawn},
                {"spawn_landmine", CrowdDelegates.Spawn},
                {"webs", CrowdDelegates.CreateWebs},
                {"killenemies", CrowdDelegates.KillEnemies},
                {"spawn_radmech", CrowdDelegates.Spawn},
                {"spawn_clay", CrowdDelegates.Spawn},
                {"spawn_butler", CrowdDelegates.Spawn},
                {"spawn_jester", CrowdDelegates.Spawn},
                {"spawn_eater", CrowdDelegates.Spawn},

                {"cspawn_pede", CrowdDelegates.CrewSpawn},
                {"cspawn_spider", CrowdDelegates.CrewSpawn},
                {"cspawn_hoard", CrowdDelegates.CrewSpawn},
                {"cspawn_flower", CrowdDelegates.CrewSpawn},
                {"cspawn_crawl", CrowdDelegates.CrewSpawn},
                {"cspawn_blob", CrowdDelegates.CrewSpawn},
                {"cspawn_spring", CrowdDelegates.CrewSpawn},
                {"cspawn_puff", CrowdDelegates.CrewSpawn},
                {"cspawn_dog", CrowdDelegates.CrewSpawn},
                {"cspawn_giant", CrowdDelegates.CrewSpawn},
                {"cspawn_levi", CrowdDelegates.CrewSpawn},
                {"cspawn_hawk", CrowdDelegates.CrewSpawn},
                {"cspawn_girl", CrowdDelegates.CrewSpawn},
                {"cspawn_cracker", CrowdDelegates.CrewSpawn},
                {"cspawn_mimic", CrowdDelegates.CrewSpawn},
                {"cspawn_landmine", CrowdDelegates.CrewSpawn},
                {"cspawn_radmech", CrowdDelegates.CrewSpawn},
                {"cspawn_butler", CrowdDelegates.CrewSpawn},
                {"cspawn_clay", CrowdDelegates.CrewSpawn},
                {"cspawn_jester", CrowdDelegates.CrewSpawn},
                {"cspawn_eater", CrowdDelegates.CrewSpawn},

                { "give_binoculars", CrowdDelegates.GiveItem},//binoculars
                { "give_boombox", CrowdDelegates.GiveItem},//boombox
                { "give_flashlight", CrowdDelegates.GiveItem},//flashlight
                { "give_jetpack", CrowdDelegates.GiveItem},//jetpack
                { "give_key", CrowdDelegates.GiveItem},//Key
                { "give_lockpicker", CrowdDelegates.GiveItem},//Lockpicker
                { "give_lungapparatus", CrowdDelegates.GiveItem},//Apparatus
                { "give_mapdevice", CrowdDelegates.GiveItem},//Mapper
                { "give_proflashlight", CrowdDelegates.GiveItem},//Pro-Flashlight
                { "give_shovel", CrowdDelegates.GiveItem},//Shovel
                { "give_stungrenade", CrowdDelegates.GiveItem},//Stun Grenade
                { "give_extensionladder", CrowdDelegates.GiveItem},//Extension Ladder
                { "give_tzpinhalant", CrowdDelegates.GiveItem},//TZP Inhalant
                { "give_walkietalkie", CrowdDelegates.GiveItem},//Walkie Talkie
                { "give_zapgun", CrowdDelegates.GiveItem},//Zap Gun
                { "give_7ball", CrowdDelegates.GiveItem},//Magic 7 Ball
                { "give_airhorn", CrowdDelegates.GiveItem},//Airhorn
                { "give_bottlebin", CrowdDelegates.GiveItem},//Bottles
                { "give_clownhorn", CrowdDelegates.GiveItem},//Clown Horn
                { "give_goldbar", CrowdDelegates.GiveItem},//Gold Bar
                { "give_stopsign", CrowdDelegates.GiveItem},//Stop Sign
                { "give_radarbooster", CrowdDelegates.GiveItem},//Radar Booster
                { "give_yieldsign", CrowdDelegates.GiveItem},//Yield Sign
                { "give_shotgun", CrowdDelegates.GiveItem},//Shotgun
                { "give_gunAmmo", CrowdDelegates.GiveItem},//Ammo
                { "give_spraypaint", CrowdDelegates.GiveItem},//Spraypaint
                { "give_giftbox", CrowdDelegates.GiveItem},//Gift Box
                { "give_tragedymask", CrowdDelegates.GiveItem},//Tragedy Mask
                { "give_comedymask", CrowdDelegates.GiveItem},//Comedy Mask
                { "give_knife", CrowdDelegates.GiveItem},//Kitchen Knife
                { "give_easteregg", CrowdDelegates.GiveItem},//Easter Egg
                { "give_weedkiller", CrowdDelegates.GiveItem},//Weed Killer
                
                { "cgive_binoculars", CrowdDelegates.GiveCrewItem},//binoculars
                { "cgive_boombox", CrowdDelegates.GiveCrewItem},//boombox
                { "cgive_flashlight", CrowdDelegates.GiveCrewItem},//flashlight
                { "cgive_jetpack", CrowdDelegates.GiveCrewItem},//jetpack
                { "cgive_key", CrowdDelegates.GiveCrewItem},//Key
                { "cgive_lockpicker", CrowdDelegates.GiveCrewItem},//Lockpicker
                { "cgive_lungapparatus", CrowdDelegates.GiveCrewItem},//Apparatus
                { "cgive_mapdevice", CrowdDelegates.GiveCrewItem},//Mapper
                { "cgive_proflashlight", CrowdDelegates.GiveCrewItem},//Pro-Flashlight
                { "cgive_shovel", CrowdDelegates.GiveCrewItem},//Shovel
                { "cgive_stungrenade", CrowdDelegates.GiveCrewItem},//Stun Grenade
                { "cgive_extensionladder", CrowdDelegates.GiveCrewItem},//Extension Ladder
                { "cgive_tzpinhalant", CrowdDelegates.GiveCrewItem},//TZP Inhalant
                { "cgive_walkietalkie", CrowdDelegates.GiveCrewItem},//Walkie Talkie
                { "cgive_zapgun", CrowdDelegates.GiveCrewItem},//Zap Gun
                { "cgive_7ball", CrowdDelegates.GiveCrewItem},//Magic 7 Ball
                { "cgive_airhorn", CrowdDelegates.GiveCrewItem},//Airhorn
                { "cgive_bottlebin", CrowdDelegates.GiveCrewItem},//Bottles
                { "cgive_clownhorn", CrowdDelegates.GiveCrewItem},//Clown Horn
                { "cgive_goldbar", CrowdDelegates.GiveCrewItem},//Gold Bar
                { "cgive_stopsign", CrowdDelegates.GiveCrewItem},//Stop Sign
                { "cgive_radarbooster", CrowdDelegates.GiveCrewItem},//Radar Booster
                { "cgive_yieldsign", CrowdDelegates.GiveCrewItem},//Yield Sign
                { "cgive_shotgun", CrowdDelegates.GiveCrewItem},//Shotgun
                { "cgive_gunAmmo", CrowdDelegates.GiveCrewItem},//Ammo
                { "cgive_spraypaint", CrowdDelegates.GiveCrewItem},//Spraypaint
                { "cgive_giftbox", CrowdDelegates.GiveCrewItem},//Gift Box
                { "cgive_tragedymask", CrowdDelegates.GiveCrewItem},//Tragedy Mask
                { "cgive_comedymask", CrowdDelegates.GiveCrewItem},//Comedy Mask
                { "cgive_knife", CrowdDelegates.GiveCrewItem},//Kitchen Knife
                { "cgive_easteregg", CrowdDelegates.GiveCrewItem},//Easter Egg
                { "cgive_weedkiller", CrowdDelegates.GiveCrewItem},//Weed Killer

                {"weather_-1", CrowdDelegates.Weather},
                {"weather_1", CrowdDelegates.Weather},
                {"weather_2", CrowdDelegates.Weather},
                {"weather_3", CrowdDelegates.Weather},
                {"weather_4", CrowdDelegates.Weather},
                {"weather_5", CrowdDelegates.Weather},
                {"weather_6", CrowdDelegates.Weather},
                {"lightning", CrowdDelegates.Lightning},

                {"takeitem", CrowdDelegates.TakeItem},
                {"dropitem", CrowdDelegates.DropItem},
                {"takecrewitem", CrowdDelegates.TakeCrewItem},

                {"buy_walkie",  CrowdDelegates.BuyItem},
                {"buy_flashlight",  CrowdDelegates.BuyItem},
                {"buy_shovel",  CrowdDelegates.BuyItem},
                {"buy_lockpicker",  CrowdDelegates.BuyItem},
                {"buy_proflashlight",  CrowdDelegates.BuyItem},
                {"buy_stungrenade",  CrowdDelegates.BuyItem},
                {"buy_boombox",  CrowdDelegates.BuyItem},
                {"buy_inhaler",  CrowdDelegates.BuyItem},
                {"buy_stungun",  CrowdDelegates.BuyItem},
                {"buy_jetpack",  CrowdDelegates.BuyItem},
                {"buy_extensionladder",  CrowdDelegates.BuyItem},
                {"buy_radarbooster",  CrowdDelegates.BuyItem},
                {"buy_spraypaint",  CrowdDelegates.BuyItem},
                {"buy_weedkiller",  CrowdDelegates.BuyItem},
                
                {"buy_cruiser", CrowdDelegates.BuyCruiser},
                {"turn_off_engine", CrowdDelegates.TurnOffEngine},
                {"destroy_vehicle", CrowdDelegates.DestroyVehicle},
                {"start_vehicle", CrowdDelegates.TurnOnVehicle},
                {"spring_chair", CrowdDelegates.SpringChair},

                {"charge", CrowdDelegates.ChargeItem},
                {"uncharge", CrowdDelegates.UnchargeItem},

                {"breakerson", CrowdDelegates.BreakersOn},
                {"breakersoff", CrowdDelegates.BreakersOff},

                {"toship", CrowdDelegates.TeleportToShip},
                {"crewship", CrowdDelegates.TeleportCrewToShip },
                {"body", CrowdDelegates.SpawnBody},
                {"crewbody", CrowdDelegates.SpawnCrewBody},
                {"nightvision", CrowdDelegates.NightVision},
                {"revive", CrowdDelegates.Revive},
                {"tocrew", CrowdDelegates.TeleportToCrew},
                {"crewto", CrowdDelegates.TeleportCrewTo},

                {"screech", CrowdDelegates.Screech},
                {"footstep", CrowdDelegates.Footstep},
                {"breathing", CrowdDelegates.Breathing},
                {"ghost", CrowdDelegates.Ghost},
                {"horn", CrowdDelegates.PlayHorn},
                {"blob", CrowdDelegates.BlobSound},
                {"highpitch", CrowdDelegates.HighPitch},
                {"lowpitch", CrowdDelegates.LowPitch},

                {"addhour", CrowdDelegates.AddHour},
                {"remhour", CrowdDelegates.RemoveHour},
                {"addday", CrowdDelegates.AddDay},
                {"remday", CrowdDelegates.RemoveDay},

                {"givecred_5", CrowdDelegates.AddCredits},
                {"givecred_50", CrowdDelegates.AddCredits},
                {"givecred_500", CrowdDelegates.AddCredits},
                {"givecred_-5", CrowdDelegates.AddCredits},
                {"givecred_-50", CrowdDelegates.AddCredits},
                {"givecred_-500", CrowdDelegates.AddCredits},

                {"givequota_5", CrowdDelegates.AddQuota},
                {"givequota_50", CrowdDelegates.AddQuota},
                {"givequota_500", CrowdDelegates.AddQuota},
                {"givequota_-5", CrowdDelegates.AddQuota},
                {"givequota_-50", CrowdDelegates.AddQuota},
                {"givequota_-500", CrowdDelegates.AddQuota},

                {"giveprofit_25", CrowdDelegates.AddProfit},
                {"giveprofit_50", CrowdDelegates.AddProfit},
                {"giveprofit_100", CrowdDelegates.AddProfit},
                {"giveprofit_-25", CrowdDelegates.AddProfit},
                {"giveprofit_-50", CrowdDelegates.AddProfit},
                {"giveprofit_-100", CrowdDelegates.AddProfit},
                {"addscrap", CrowdDelegates.AddScrap},

                {"shipleave", CrowdDelegates.ShipLeave},
                {"opendoors", CrowdDelegates.OpenDoors},
                {"closedoors", CrowdDelegates.CloseDoors},
                {"inverse", CrowdDelegates.InverseTeleport},
                {"cinverse", CrowdDelegates.InverseTeleportCrew},
            };
        }

        public static void HideEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_NOTVISIBLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void ShowEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_VISIBLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void DisableEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_NOTSELECTABLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void EnableEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_SELECTABLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }
        private void ClientLoop()
        {

            TestMod.mls.LogInfo("Connected to Crowd Control");
            connect = true;

            var timer = new Timer(timeUpdate, null, 0, 200);

            try
            {
                while (Running)
                {
                    CrowdRequest req = CrowdRequest.Recieve(this, Socket);
                    if (req == null || req.IsKeepAlive()) continue;

                    lock (Requests)
                        Requests.Enqueue(req);
                }
            }
            catch (Exception)
            {
                TestMod.mls.LogInfo("Disconnected from Crowd Control");
                connect = false;
                Socket.Close();
            }
        }

        public void timeUpdate(System.Object state)
        {
            inGame = true;

            if (StartOfRound.Instance == null || StartOfRound.Instance.allPlayersDead || StartOfRound.Instance.livingPlayers < 1) inGame = false;

            if (Saving || !inGame)
            {
                BuffThread.addTime(200);
                paused = true;
            }
            else if (paused)
            {
                paused = false;
                BuffThread.unPause();
                BuffThread.tickTime(200);
            }
            else
            {
                BuffThread.tickTime(200);
            }
        }

        public bool CanSpawn() => Spawn;
        public bool IsRunning() => Running;

        public void NetworkLoop()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            while (Running)
            {

                TestMod.mls.LogInfo("Attempting to connect to Crowd Control");

                try
                {
                    Socket = new Socket(Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    if (Socket.BeginConnect(Endpoint, null, null).AsyncWaitHandle.WaitOne(10000, true) && Socket.Connected)
                        ClientLoop();
                    else
                        TestMod.mls.LogInfo("Failed to connect to Crowd Control");
                    Socket.Close();
                }
                catch (Exception e)
                {
                    TestMod.mls.LogInfo("Failed to connect to Crowd Control");
                }

                Thread.Sleep(10000);
            }
        }

        public void RequestLoop()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            while (Running)
            {
                try
                {
                    while (Saving || !inGame)
                        Thread.Yield();

                    CrowdRequest req = null;
                    lock (Requests)
                    {
                        if (Requests.Count == 0)
                            continue;
                        req = Requests.Dequeue();
                    }

                    string code = req.GetReqCode();
                    try
                    {
                        CrowdResponse res;
                        if (!isReady())
                            res = new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);
                        else
                            res = Delegate[code](this, req);
                        if (res == null)
                        {
                            new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, $"Request error for '{code}'").Send(Socket);
                        }

                        res.Send(Socket);
                    }
                    catch (KeyNotFoundException)
                    {
                        new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, $"Request error for '{code}'").Send(Socket);
                    }
                }
                catch (Exception)
                {
                    TestMod.mls.LogInfo("Disconnected from Crowd Control");
                    Socket.Close();
                }
            }
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
                TestMod.mls.LogError(e.ToString());
                return false;
            }

            return true;
        }

        public void Stop()
        {
            Running = false;
        }
    }
}
