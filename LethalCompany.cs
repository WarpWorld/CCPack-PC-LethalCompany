using CrowdControl.Common;
using System.ComponentModel;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs.LethalCompany
{
    public class LethalCompany : SimpleTCPPack
    {
        public override string Host => "127.0.0.1";

        public override ushort Port => 51338;

        public override ISimpleTCPPack.MessageFormat MessageFormat => ISimpleTCPPack.MessageFormat.CrowdControlLegacy;

        public LethalCompany(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

        public override Game Game { get; } = new("Lethal Company", "LethalCompany", "PC", ConnectorType.SimpleTCPServerConnector);

        public override EffectList Effects => new List<Effect>
        {
                new("Kill Player", "kill") { Category = "Health"},
                new("Kill Crewmate", "killcrew") { Category = "Health"},
                new("Damage Player", "damage") { Category = "Health"},
                new("Damage Crewmate", "damagecrew") { Category = "Health"},
                new("Heal Player", "heal") { Category = "Health"},
                new("Heal Crewmate", "healcrew") { Category = "Health"},

                new("OHKO", "ohko") { Category = "Health", Duration = 30},
                new("Invincible", "invul") { Category = "Health", Duration = 30},

                new("Drain Stamina", "drain") { Category = "Stamina"},
                new("Restore Stamina", "restore") { Category = "Stamina"},

                new("No Stamina", "nostam") { Category = "Stamina", Duration = 30},
                new("Infinite Stamina", "infstam") { Category = "Stamina", Duration = 30},

                new("High Pitched Voices", "highpitch") { Category = "Sound", Duration = 30},
                new("Low Pitched Voices", "lowpitch") { Category = "Sound", Duration = 30},

                //new("Launch Player", "launch") { Category = "Player"},

                new("Hyper Player", "hyper") { Category = "Player", Duration = 15},
                new("Fast Player", "fast") { Category = "Player", Duration = 30},
                new("Slow Player", "slow") { Category = "Player", Duration = 30},
                new("Freeze Player", "freeze") { Category = "Player", Duration = 5},

                new("Drunk Player", "drunk") { Category = "Player", Duration = 10, Price = 500, Description = "Gives the player a drunk effect."},

                new("Ultra Jump", "jumpultra") { Category = "Player", Duration = 15},
                new("High Jump", "jumphigh") { Category = "Player", Duration = 30},
                new("Low Jump", "jumplow") { Category = "Player", Duration = 30},
                //new("Night Vision", "nightvision") { Category = "Player", Duration = 30},
                new("Revive Dead Players", "revive") { Category = "Player"},

                new("Spawn Centipede", "spawn_pede") { Category = "Spawn Enemies"},
                new("Spawn Nutcracker", "spawn_cracker") { Category = "Spawn Enemies"},
                new("Spawn Bunker Spider", "spawn_spider") { Category = "Spawn Enemies"},
                new("Spawn Hoarding Bug", "spawn_hoard") { Category = "Spawn Enemies"},
                new("Spawn Bracken", "spawn_flower") { Category = "Spawn Enemies"},
                new("Spawn Crawler", "spawn_crawl") { Category = "Spawn Enemies"},
                new("Spawn Blob", "spawn_blob") { Category = "Spawn Enemies"},
                new("Spawn Coil-Head", "spawn_spring") { Category = "Spawn Enemies"},
                new("Spawn Puffer", "spawn_puff") { Category = "Spawn Enemies"},
                new("Spawn Eyeless Dog", "spawn_dog") { Category = "Spawn Enemies"},
                new("Spawn Forest Giant", "spawn_giant") { Category = "Spawn Enemies"},
                new("Spawn Earth Leviathan", "spawn_levi") { Category = "Spawn Enemies"},
                new("Spawn Baboon Hawk", "spawn_hawk") { Category = "Spawn Enemies"},
                new("Spawn Ghost Girl", "spawn_girl") { Category = "Spawn Enemies"},
                //new("Spawn Cobwebs", "webs") { Category = "Spawn Enemies"},
                new("Spawn Mimic", "spawn_mimic") { Category = "Spawn Enemies"},
                new("Spawn Landmine", "spawn_landmine") { Category = "Spawn Enemies"},
                new("Spawn Old Bird", "spawn_radmech") { Category = "Spawn Enemies"},
                new("Spawn Kidnapper Fox", "spawn_bushwolf"){Category = "Spawn Enemies"},

                new("Spawn Centipede at Crewmate", "cspawn_pede") { Category = "Spawn Enemies"},
                new("Spawn Nutcracker at Crewmate", "cspawn_cracker") { Category = "Spawn Enemies"},
                new("Spawn Bunker Spider at Crewmate", "cspawn_spider") { Category = "Spawn Enemies"},
                new("Spawn Hoarding Bug at Crewmate", "cspawn_hoard") { Category = "Spawn Enemies"},
                new("Spawn Bracken at Crewmate", "cspawn_flower") { Category = "Spawn Enemies"},
                new("Spawn Crawler at Crewmate", "cspawn_crawl") { Category = "Spawn Enemies"},
                new("Spawn Blob at Crewmate", "cspawn_blob") { Category = "Spawn Enemies"},
                new("Spawn Coil-Head at Crewmate", "cspawn_coil") { Category = "Spawn Enemies"},
                new("Spawn Puffer at Crewmate", "cspawn_puff") { Category = "Spawn Enemies"},
                new("Spawn Eyeless Dog at Crewmate", "cspawn_dog") { Category = "Spawn Enemies"},
                new("Spawn Forest Giant at Crewmate", "cspawn_giant") { Category = "Spawn Enemies"},
                new("Spawn Earth Leviathan at Crewmate", "cspawn_levi") { Category = "Spawn Enemies"},
                new("Spawn Baboon Hawk at Crewmate", "cspawn_hawk") { Category = "Spawn Enemies"},
                new("Spawn Ghost Girl at Crewmate", "cspawn_girl") { Category = "Spawn Enemies"},
                new("Spawn Mimic at Crewmate", "cspawn_mimic") { Category = "Spawn Enemies"},
                new("Spawn Landmine at Crewmate", "cspawn_landmine") { Category = "Spawn Enemies"},
                new("Spawn Old Bird at Crewmate", "cspawn_radmech") { Category = "Spawn Enemies" },

                new("Give Walkie-Talkie", "give_0") { Category = "Items"},
                new("Give Flashlight", "give_1") { Category = "Items"},
                new("Give Shovel", "give_2") { Category = "Items"},
                new("Give Lockpicker", "give_3") { Category = "Items"},
                new("Give Pro Flashlight", "give_4") { Category = "Items"},
                new("Give Stun Grenade", "give_5") { Category = "Items"},
                new("Give Boom Box", "give_6") { Category = "Items"},
                new("Give Inhaler", "give_7") { Category = "Items"},
                new("Give Stun Gun", "give_8") { Category = "Items"},
                new("Give Jet Pack", "give_9") { Category = "Items"},
                new("Give Extension Ladder", "give_10") { Category = "Items"},
                new("Give Radar Booster", "give_11") { Category = "Items"},
                new("Give Tragedy Mask", "givem_tragedymask") { Category = "Items"},
                new("Give Comedy Mask", "givem_comedymask") { Category = "Items"},
                new("Give Spray Paint", "give_12"){ Category = "Items"},
                new("Give Weed Killer", "give_13"){ Category = "Items"},
                new("Give Shotgun", "give_18"){ Category = "Items"},
                new("Give Shotgun Ammo", "give_19"){ Category = "Items"},

                new("Give Crewmate Walkie-Talkie", "cgive_0") { Category = "Items"},
                new("Give Crewmate Flashlight", "cgive_1") { Category = "Items"},
                new("Give Crewmate Shovel", "cgive_2") { Category = "Items"},
                new("Give Crewmate Lockpicker", "cgive_3") { Category = "Items"},
                new("Give Crewmate Pro Flashlight", "cgive_4") { Category = "Items"},
                new("Give Crewmate Stun Grenade", "cgive_5") { Category = "Items"},
                new("Give Crewmate Boom Box", "cgive_6") { Category = "Items"},
                new("Give Crewmate Inhaler", "cgive_7") { Category = "Items"},
                new("Give Crewmate Stun Gun", "cgive_8") { Category = "Items"},
                new("Give Crewmate Jet Pack", "cgive_9") { Category = "Items"},
                new("Give Crewmate Extension Ladder", "cgive_10") { Category = "Items"},
                new("Give Crewmate Radar Booster", "cgive_11") { Category = "Items"},
                new("Give Crewmate Tragedy Mask", "cgivem_tragedymask") { Category = "Items"},
                new("Give Crewmate Comedy Mask", "cgivem_comedymask") { Category = "Items"},
                new("Give Crewmate Spray Paint", "cgive_12"){Category= "Items"},
                new("Give Crewmate Weed Killer", "cgive_13"){Category= "Items"},

                new("Charge Item Battery", "charge") { Category = "Items"},
                new("Drain Item Battery", "uncharge") { Category = "Items"},

                new("Turn Breakers On", "breakerson"),
                new("Turn Breakers Off", "breakersoff"),

                new("Kill Nearby Enemies", "killenemies"),

                new("Stock Walkie-Talkie", "buy_0") { Category = "Items"},
                new("Stock Flashlight", "buy_1") { Category = "Items"},
                new("Stock Shovel", "buy_2") { Category = "Items"},
                new("Stock Lockpicker", "buy_3") { Category = "Items"},
                new("Stock Pro Flashlight", "buy_4") { Category = "Items"},
                new("Stock Stun Grenade", "buy_5") { Category = "Items"},
                new("Stock Boom Box", "buy_6") { Category = "Items"},
                new("Stock Inhaler", "buy_7") { Category = "Items"},
                new("Stock Stun Gun", "buy_8") { Category = "Items"},
                new("Stock Jet Pack", "buy_9") { Category = "Items"},
                new("Stock Extension Ladder", "buy_10") { Category = "Items"},
                new("Stock Radar Booster", "buy_11") { Category = "Items"},

                new("Take Held Item", "takeitem") { Category = "Items"},
                new("Drop Held Item", "dropitem") { Category = "Items"},
                new("Take Crewmate Held Item", "takecrewitem") { Category = "Items"},

                new("Weather - Clear", "weather_-1") { Category = "Weather"},
                new("Weather - Cloudy", "weather_0") { Category = "Weather"},
                new("Weather - Rainy", "weather_1") { Category = "Weather"},
                new("Weather - Stormy", "weather_2") { Category = "Weather"},
                new("Weather - Foggy", "weather_3") { Category = "Weather"},
                new("Weather - Flooded", "weather_4") { Category = "Weather"},
                new("Weather - Eclipsed", "weather_5") { Category = "Weather"},
                //new("Random Lightning Strike", "lightning") { Category = "Weather"},

                new("Give 5 Credits", "givecred_5") { Category = "Scrap/Money"},
                new("Give 50 Credits", "givecred_50") { Category = "Scrap/Money"},
                new("Give 500 Credits", "givecred_500") { Category = "Scrap/Money"},
                new("Take 5 Credits", "givecred_-5") { Category = "Scrap/Money"},
                new("Take 50 Credits", "givecred_-50") { Category = "Scrap/Money"},
                new("Take 500 Credits", "givecred_-500") { Category = "Scrap/Money"},
                new("Spawn Scrap in Level", "addscrap") { Category = "Scrap/Money"},

                new("Raise Quota by 5", "givequota_5") { Category = "Scrap/Money"},
                new("Raise Quota by 50", "givequota_50") { Category = "Scrap/Money"},
                new("Raise Quota by 500", "givequota_500") { Category = "Scrap/Money"},
                new("Lower Quota by 5", "givequota_-5") { Category = "Scrap/Money"},
                new("Lower Quota by 50", "givequota_-50") { Category = "Scrap/Money"},
                new("Lower Quota by 500", "givequota_-500") { Category = "Scrap/Money"},

                new("Fill Quota by 25%", "giveprofit_25") { Category = "Scrap/Money"},
                new("Fill Quota by 50%", "giveprofit_50") { Category = "Scrap/Money"},
                new("Complete Quota", "giveprofit_100") { Category = "Scrap/Money"},
                new("Empty Quota by 25%", "giveprofit_-25") { Category = "Scrap/Money"},
                new("Empty Quota by 50%", "giveprofit_-50") { Category = "Scrap/Money"},
                new("Reset Quota Progress", "giveprofit_-100") { Category = "Scrap/Money"},

                new("Return to Ship", "toship") { Category = "Player"},
                new("Return Crewmate to Ship", "crewship") { Category = "Player"},
                new("Teleport to Crewmate", "tocrew") { Category = "Player"},
                new("Teleport Crewmate Here", "crewto") { Category = "Player"},
                new("Spawn Dead Body", "body") { Category = "Player"},
                new("Spawn Dead Crewmate", "crewbody") { Category = "Player"},

                new("Play Bird Screech", "screech") { Category = "Sound"},
                //new("Play Blob Sound", "blob") { Category = "Sound"},
                new("Play Footstep", "footstep") { Category = "Sound"},
                //new("Play Horn", "horn") { Category = "Sound"},
                new("Play Breathing", "breathing") { Category = "Sound"},
                //new("Play Ghost Appearing", "ghost") { Category = "Sound"},

                new("Advance Time One Hour", "addhour") { Category = "Time"},
                new("Rollback Time One Hour", "remhour") { Category = "Time"},

                new("Extend Deadline by One Day", "addday") { Category = "Time"},
                new("Shorten Deadline by One Day", "remday") { Category = "Time"},

                new("Ship Leaves Early", "shipleave") { Category = "Ship"},
                new("Close Ship Doors", "closedoors") { Category = "Ship"},
                new("Open Ship Doors", "opendoors") { Category = "Ship"},
        };
    }
}