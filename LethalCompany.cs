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
                new("Spawn Clay Surgeon", "spawn_clay") { Category = "Spawn Enemies"},//Tested on Dine
                new("Spawn Butler", "spawn_butler"){ Category = "Spawn Enemies"},//Tested on Dine
                new("Spawn Jester", "spawn_jester"){Category = "Spawn Enemies"},
                new("Spawn Maneater", "spawn_eater"){Category = "Spawn Enemies"},
                new("Spawn Tulip Snake", "spawn_snake"){Category = "Spawn Enemies"},
                new("Spawn Sapsucker", "spawn_kiwi"){Category = "Spawn Enemies"},

                new("Spawn Centipede at Crewmate", "cspawn_pede") { Category = "Spawn Enemies"},
                new("Spawn Nutcracker at Crewmate", "cspawn_cracker") { Category = "Spawn Enemies"},
                new("Spawn Bunker Spider at Crewmate", "cspawn_spider") { Category = "Spawn Enemies"},
                new("Spawn Hoarding Bug at Crewmate", "cspawn_hoard") { Category = "Spawn Enemies"},
                new("Spawn Bracken at Crewmate", "cspawn_flower") { Category = "Spawn Enemies"},
                new("Spawn Crawler at Crewmate", "cspawn_crawl") { Category = "Spawn Enemies"},
                new("Spawn Blob at Crewmate", "cspawn_blob") { Category = "Spawn Enemies"},
                new("Spawn Coil-Head at Crewmate", "cspawn_spring") { Category = "Spawn Enemies"},
                new("Spawn Puffer at Crewmate", "cspawn_puff") { Category = "Spawn Enemies"},
                new("Spawn Eyeless Dog at Crewmate", "cspawn_dog") { Category = "Spawn Enemies"},
                new("Spawn Forest Giant at Crewmate", "cspawn_giant") { Category = "Spawn Enemies"},
                new("Spawn Earth Leviathan at Crewmate", "cspawn_levi") { Category = "Spawn Enemies"},
                new("Spawn Baboon Hawk at Crewmate", "cspawn_hawk") { Category = "Spawn Enemies"},
                new("Spawn Ghost Girl at Crewmate", "cspawn_girl") { Category = "Spawn Enemies"},
                new("Spawn Mimic at Crewmate", "cspawn_mimic") { Category = "Spawn Enemies"},
                new("Spawn Landmine at Crewmate", "cspawn_landmine") { Category = "Spawn Enemies"},
                new("Spawn Old Bird at Crewmate", "cspawn_radmech") { Category = "Spawn Enemies" },
                new("Spawn Clay Surgeon at Crewmate", "cspawn_clay") { Category = "Spawn Enemies"},//Tested on Dine
                new("Spawn Butler at Crewmate", "cspawn_butler") { Category = "Spawn Enemies"},//Tested on Dine
		        new("Spawn Jester at Crewmate", "cspawn_jester"){Category = "Spawn Enemies"},
                new("Spawn Maneater at Crewmate", "cspawn_eater"){Category = "Spawn Enemies"},
                new("Spawn Tulip Snake at Crewmate", "cspawn_snake"){Category = "Spawn Enemies"},
                new("Spawn Sapsucker at Crewmate", "cspawn_kiwi"){Category = "Spawn Enemies"},

                new("Give Binoculars", "give_binoculars") { Category = "Items"},
                new("Give Boombox", "give_boombox") { Category = "Items"},
                new("Give Flashlight", "give_flashlight") { Category = "Items"},
                new("Give Jetpack", "give_jetpack") { Category = "Items"},
                new("Give Key", "give_key") { Category = "Items"},
                new("Give Lockpicker", "give_lockpicker") { Category = "Items"},
                new("Give Apparatus", "give_lungapparatus") { Category = "Items"},
                new("Give Mapper", "give_mapdevice") { Category = "Items"},
                new("Give Pro-Flashlight", "give_proflashlight") { Category = "Items"},
                new("Give Shovel", "give_shovel") { Category = "Items"},
                new("Give Stun Grenade", "give_stungrenade") { Category = "Items"},
                new("Give Extension Ladder", "give_extensionladder") { Category = "Items"},
                new("Give TZP Inhalant", "give_tzpinhalant") { Category = "Items"},
                new("Give Walkie Talkie", "give_walkietalkie") { Category = "Items"},
                new("Give Zap Gun", "give_zapgun") { Category = "Items"},
                new("Give Magic 7 Ball", "give_7ball") { Category = "Items"},
                new("Give Airhorn", "give_airhorn") { Category = "Items"},
                new("Give Bottles", "give_bottlebin") { Category = "Items"},
                new("Give Clown Horn", "give_clownhorn") { Category = "Items"},
                new("Give Gold Bar", "give_goldbar") { Category = "Items"},
                new("Give Stop Sign", "give_stopsign") { Category = "Items"},
                new("Give Radar Booster", "give_radarbooster") { Category = "Items"},
                new("Give Yield Sign", "give_yieldsign") { Category = "Items"},
                new("Give Shotgun", "give_shotgun") { Category = "Items"},
                new("Give Shells", "give_gunAmmo") { Category = "Items"},
                new("Give Spray Paint", "give_spraypaint") { Category = "Items"},
                new("Give Present", "give_giftbox") { Category = "Items"},
                new("Give Tragedy Mask", "give_tragedymask") { Category = "Items"},
                new("Give Comedy Mask", "give_comedymask") { Category = "Items"},
                new("Give Kitchen Knife", "give_knife") { Category = "Items"},
                new("Give Easter Egg", "give_easteregg") { Category = "Items"},
                new("Give Weed Killer", "give_weedkillerbottle") { Category = "Items"},//v55 item

                new("Give Crewmate Binoculars", "cgive_binoculars") { Category = "Items"},
                new("Give Crewmate Boombox", "cgive_boombox") { Category = "Items"},
                new("Give Crewmate Flashlight", "cgive_flashlight") { Category = "Items"},
                new("Give Crewmate Jetpack", "cgive_jetpack") { Category = "Items"},
                new("Give Crewmate Key", "cgive_key") { Category = "Items"},
                new("Give Crewmate Lockpicker", "cgive_lockpicker") { Category = "Items"},
                new("Give Crewmate Apparatus", "cgive_lungapparatus") { Category = "Items"},
                new("Give Crewmate Mapper", "cgive_mapdevice") { Category = "Items"},
                new("Give Crewmate Pro-Flashlight", "cgive_proflashlight") { Category = "Items"},
                new("Give Crewmate Shovel", "cgive_shovel") { Category = "Items"},
                new("Give Crewmate Stun Grenade", "cgive_stungrenade") { Category = "Items"},
                new("Give Crewmate Extension Ladder", "cgive_extensionladder") { Category = "Items"},
                new("Give Crewmate TZP Inhalant", "cgive_tzpinhalant") { Category = "Items"},
                new("Give Crewmate Walkie Talkie", "cgive_walkietalkie") { Category = "Items"},
                new("Give Crewmate Zap Gun", "cgive_zapgun") { Category = "Items"},
                new("Give Crewmate Magic 7 Ball", "cgive_7ball") { Category = "Items"},
                new("Give Crewmate Airhorn", "cgive_airhorn") { Category = "Items"},
                new("Give Crewmate Bottles", "cgive_bottlebin") { Category = "Items"},
                new("Give Crewmate Clown Horn", "cgive_clownhorn") { Category = "Items"},
                new("Give Crewmate Gold Bar", "cgive_goldbar") { Category = "Items"},
                new("Give Crewmate Stop Sign", "cgive_stopsign") { Category = "Items"},
                new("Give Crewmate Radar Booster", "cgive_radarbooster") { Category = "Items"},
                new("Give Crewmate Yield Sign", "cgive_yieldsign") { Category = "Items"},
                new("Give Crewmate Shotgun", "cgive_shotgun") { Category = "Items"},
                new("Give Crewmate Shells", "cgive_gunAmmo") { Category = "Items"},
                new("Give Crewmate Spray Paint", "cgive_spraypaint") { Category = "Items"},
                new("Give Crewmate Present", "cgive_giftbox") { Category = "Items"},
                new("Give Crewmate Tragedy Mask", "cgive_tragedymask") { Category = "Items"},
                new("Give Crewmate Comedy Mask", "cgive_comedymask") { Category = "Items"},
                new("Give Crewmate Kitchen Knife", "cgive_knife") { Category = "Items"},
                new("Give Crewmate Easter Egg", "cgive_easteregg") { Category = "Items"},
                new("Give Crewmate Weed Killer", "cgive_weedkillerbottle") { Category = "Items"},//v55 item


                new("Charge Item Battery", "charge") { Category = "Items"},
                new("Drain Item Battery", "uncharge") { Category = "Items"},

                new("Turn Breakers On", "breakerson"),
                new("Turn Breakers Off", "breakersoff"),
                new("Kill Nearby Enemies", "killenemies"),
                
                new("Spring Vehicle Chair", "spring_chair") { Category = "Vehicle"},
                new("Turn off Vehicle Engine", "turn_off_engine") { Category = "Vehicle"},
                new("Destroy Vehicle", "destroy_vehicle") { Category = "Vehicle"},
                new("Stock Company Cruiser", "buy_cruiser") { Category = "Vehicle"},
                new("Start Vehicle Engine", "start_vehicle") { Category = "Vehicle"},

                new("Stock Walkie-Talkie", "buy_walkie") { Category = "Items"},
                new("Stock Flashlight", "buy_flashlight") { Category = "Items"},
                new("Stock Shovel", "buy_shovel") { Category = "Items"},
                new("Stock Lockpicker", "buy_lockpicker") { Category = "Items"},
                new("Stock Pro Flashlight", "buy_proflashlight") { Category = "Items"},
                new("Stock Stun Grenade", "buy_stungrenade") { Category = "Items"},
                new("Stock Boom Box", "buy_boombox") { Category = "Items"},
                new("Stock Inhaler", "buy_inhaler") { Category = "Items"},
                new("Stock Stun Gun", "buy_stungun") { Category = "Items"},
                new("Stock Jet Pack", "buy_jetpack") { Category = "Items"},
                new("Stock Extension Ladder", "buy_extensionladder") { Category = "Items"},
                new("Stock Radar Booster", "buy_radarbooster") { Category = "Items"},
                new("Stock Spray Paint", "buy_spraypaint") { Category = "Items"},
                new("Stock Weed Killer", "buy_weedkiller") { Category = "Items"},

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
		        new("Inverse Teleport", "inverse"){Category = "Player"},
                new("Inverse Teleport Random Crewmate", "cinverse"){Category ="Player" },

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