﻿﻿using System;
using System.Collections.Generic;
using CrowdControl.Common;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs.LethalCompany {

    public class LethalCompany : SimpleTCPPack
    {
        public override string Host => "127.0.0.1";

        public override ushort Port => 51337;

        public override ISimpleTCPPack.MessageFormat MessageFormat => ISimpleTCPPack.MessageFormat.CrowdControlLegacy;

        public LethalCompany(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

        public override Game Game { get; } = new("Lethal Company", "LethalCompany", "PC", ConnectorType.SimpleTCPServerConnector);

        public override EffectList Effects => new List<Effect>
        {
                new Effect("Kill Player", "kill") { Category = "Health"},
                new Effect("Damage Player", "damage") { Category = "Health"},
                new Effect("Heal Player", "heal") { Category = "Health"},

                new Effect("OHKO", "ohko") { Category = "Health", Duration = 30},
                new Effect("Invincible", "invul") { Category = "Health", Duration = 30},

                new Effect("Drain Stamina", "drain") { Category = "Stamina"},
                new Effect("Restore Stamina", "restore") { Category = "Stamina"},

                new Effect("No Stamina", "nostam") { Category = "Stamina", Duration = 30},
                new Effect("Infinite Stamina", "infstam") { Category = "Stamina", Duration = 30},


                //new Effect("Launch Player", "launch") { Category = "Player"},

                new Effect("Hyper Player", "hyper") { Category = "Player", Duration = 15},
                new Effect("Fast Player", "fast") { Category = "Player", Duration = 30},
                new Effect("Slow Player", "slow") { Category = "Player", Duration = 30},
                new Effect("Freeze Player", "freeze") { Category = "Player", Duration = 5},

                new Effect("Ultra Jump", "jumpultra") { Category = "Player", Duration = 15},
                new Effect("High Jump", "jumphigh") { Category = "Player", Duration = 30},
                new Effect("Low Jump", "jumplow") { Category = "Player", Duration = 30},
                //new Effect("Night Vision", "nightvision") { Category = "Player", Duration = 30},
                new Effect("Revive Dead Players", "revive") { Category = "Player"},

                new Effect("Spawn Centipede", "spawn_pede") { Category = "Spawn Enemies"},
                new Effect("Spawn Bunker Spider", "spawn_spider") { Category = "Spawn Enemies"},
                new Effect("Spawn Hoarding Bug", "spawn_hoard") { Category = "Spawn Enemies"},
                new Effect("Spawn Bracken", "spawn_flower") { Category = "Spawn Enemies"},
                new Effect("Spawn Crawler", "spawn_crawl") { Category = "Spawn Enemies"},
                new Effect("Spawn Blob", "spawn_blob") { Category = "Spawn Enemies"},
                new Effect("Spawn Coil-Head", "spawn_coil") { Category = "Spawn Enemies"},
                new Effect("Spawn Puffer", "spawn_puff") { Category = "Spawn Enemies"},
                new Effect("Spawn Eyeless Dog", "spawn_dog") { Category = "Spawn Enemies"},
                new Effect("Spawn Forest Giant", "spawn_giant") { Category = "Spawn Enemies"},
                new Effect("Spawn Earth Leviathan", "spawn_levi") { Category = "Spawn Enemies"},
                new Effect("Spawn Baboon Hawk", "spawn_hawk") { Category = "Spawn Enemies"},
                new Effect("Spawn Ghost Girl", "spawn_girl") { Category = "Spawn Enemies"},
                //new Effect("Spawn Cobwebs", "webs") { Category = "Spawn Enemies"},

                new Effect("Give Walkie-Talkie", "give_0") { Category = "Items"},
                new Effect("Give Flashlight", "give_1") { Category = "Items"},
                new Effect("Give Shovel", "give_2") { Category = "Items"},
                new Effect("Give Lockpicker", "give_3") { Category = "Items"},
                new Effect("Give Pro Flashlight", "give_4") { Category = "Items"},
                new Effect("Give Stun Grenade", "give_5") { Category = "Items"},
                new Effect("Give Boom Box", "give_6") { Category = "Items"},
                new Effect("Give Inhaler", "give_7") { Category = "Items"},
                new Effect("Give Stun Gun", "give_8") { Category = "Items"},
                new Effect("Give Jet Pack", "give_9") { Category = "Items"},
                new Effect("Give Extension Ladder", "give_10") { Category = "Items"},
                new Effect("Give Radar Booster", "give_11") { Category = "Items"},

                new Effect("Charge Item Battery", "charge") { Category = "Items"},
                new Effect("Drain Item Battery", "uncharge") { Category = "Items"},

                new Effect("Turn Breakers On", "breakerson"),
                new Effect("Turn Breakers Off", "breakersoff"),

                new Effect("Kill Nearby Enemies", "killenemies"),


                new Effect("Stock Walkie-Talkie", "buy_0") { Category = "Items"},
                new Effect("Stock Flashlight", "buy_1") { Category = "Items"},
                new Effect("Stock Shovel", "buy_2") { Category = "Items"},
                new Effect("Stock Lockpicker", "buy_3") { Category = "Items"},
                new Effect("Stock Pro Flashlight", "buy_4") { Category = "Items"},
                new Effect("Stock Stun Grenade", "buy_5") { Category = "Items"},
                new Effect("Stock Boom Box", "buy_6") { Category = "Items"},
                new Effect("Stock Inhaler", "buy_7") { Category = "Items"},
                new Effect("Stock Stun Gun", "buy_8") { Category = "Items"},
                new Effect("Stock Jet Pack", "buy_9") { Category = "Items"},
                new Effect("Stock Extension Ladder", "buy_10") { Category = "Items"},
                new Effect("Stock Radar Booster", "buy_11") { Category = "Items"},

                new Effect("Take Held Item", "takeitem") { Category = "Items"},
                new Effect("Drop Held Item", "dropitem") { Category = "Items"},

                new Effect("Weather - Clear", "weather_-1") { Category = "Weather"},
                new Effect("Weather - Cloudy", "weather_0") { Category = "Weather"},
                new Effect("Weather - Rainy", "weather_1") { Category = "Weather"},
                new Effect("Weather - Stormy", "weather_2") { Category = "Weather"},
                new Effect("Weather - Foggy", "weather_3") { Category = "Weather"},
                new Effect("Weather - Flooded", "weather_4") { Category = "Weather"},
                new Effect("Weather - Eclipsed", "weather_5") { Category = "Weather"},
                //new Effect("Random Lightning Strike", "lightning") { Category = "Weather"},

                new Effect("Give 5 Credits", "givecred_5") { Category = "Scrap/Money"},
                new Effect("Give 50 Credits", "givecred_50") { Category = "Scrap/Money"},
                new Effect("Give 500 Credits", "givecred_500") { Category = "Scrap/Money"},
                new Effect("Take 5 Credits", "givecred_-5") { Category = "Scrap/Money"},
                new Effect("Take 50 Credits", "givecred_-50") { Category = "Scrap/Money"},
                new Effect("Take 500 Credits", "givecred_-500") { Category = "Scrap/Money"},
                new Effect("Spawn Scrap in Level", "addscrap") { Category = "Scrap/Money"},

                new Effect("Raise Quota by 5", "givequota_5") { Category = "Scrap/Money"},
                new Effect("Raise Quota by 50", "givequota_50") { Category = "Scrap/Money"},
                new Effect("Raise Quota by 500", "givequota_500") { Category = "Scrap/Money"},
                new Effect("Lower Quota by 5", "givequota_-5") { Category = "Scrap/Money"},
                new Effect("Lower Quota by 50", "givequota_-50") { Category = "Scrap/Money"},
                new Effect("Lower Quota by 500", "givequota_-500") { Category = "Scrap/Money"},

                new Effect("Fill Quota by 25%", "giveprofit_25") { Category = "Scrap/Money"},
                new Effect("Fill Quota by 50%", "giveprofit_50") { Category = "Scrap/Money"},
                new Effect("Complete Quota", "giveprofit_100") { Category = "Scrap/Money"},
                new Effect("Empty Quota by 25%", "giveprofit_-25") { Category = "Scrap/Money"},
                new Effect("Empty Quota by 50%", "giveprofit_-50") { Category = "Scrap/Money"},
                new Effect("Reset Quota Progress", "giveprofit_-100") { Category = "Scrap/Money"},

                new Effect("Return to Ship", "toship") { Category = "Player"},
                new Effect("Spawn Dead Body", "body") { Category = "Player"},

                new Effect("Play Bird Screech", "screech") { Category = "Sound"},
                //new Effect("Play Blob Sound", "blob") { Category = "Sound"},
                new Effect("Play Footstep", "footstep") { Category = "Sound"},
                //new Effect("Play Horn", "horn") { Category = "Sound"},
                new Effect("Play Breathing", "breathing") { Category = "Sound"},
                //new Effect("Play Ghost Appearing", "ghost") { Category = "Sound"},

                new Effect("Advance Time One Hour", "addhour") { Category = "Time"},
                new Effect("Rollback Time One Hour", "remhour") { Category = "Time"},

                new Effect("Extend Deadline by One Day", "addday") { Category = "Time"},
                new Effect("Shorten Deadline by One Day", "remday") { Category = "Time"},

                new Effect("Ship Leaves Early", "shipleave") { Category = "Ship"},
                new Effect("Close Ship Doors", "closedoors") { Category = "Ship"},
                new Effect("Open Ship Doors", "opendoors") { Category = "Ship"},

        };
    }
}