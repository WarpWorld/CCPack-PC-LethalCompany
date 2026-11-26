using System.Text;

namespace CrowdControl.UI;

/// <summary>
/// Display names for effect codes, used by the on-screen overlay.
/// </summary>
/// <remarks>
/// The Crowd Control pack (the .cs file describing your effects to the app) is the single source of
/// truth for what an effect is called. An overlay that disagreed with the menu a viewer just bought
/// from would be worse than no label at all, so fill <see cref="Names"/> in from the pack - a
/// generated block is easiest to keep honest:
///
/// <code>
///     { "speedUp",   "Speed Up" },
///     { "lowGravity", "Low Gravity" },
/// </code>
///
/// Anything missing falls back to a tidied-up version of the code, so the overlay stays readable
/// while an effect is still being written.
/// </remarks>
public static class EffectNames
{
    private static readonly Dictionary<string, string> Names = new(StringComparer.OrdinalIgnoreCase)
    {
        //display names from the pack definition (LethalCompany.cs), shown in the overlay for timed effects
        ["kill"] = "Kill Player",
        ["killcrew"] = "Kill Crewmate",
        ["damage"] = "Damage Player",
        ["damagecrew"] = "Damage Crewmate",
        ["heal"] = "Heal Player",
        ["healcrew"] = "Heal Crewmate",
        ["ohko"] = "OHKO",
        ["invul"] = "Invincible",
        ["drain"] = "Drain Stamina",
        ["restore"] = "Restore Stamina",
        ["nostam"] = "No Stamina",
        ["infstam"] = "Infinite Stamina",
        ["highpitch"] = "High Pitched Voices",
        ["lowpitch"] = "Low Pitched Voices",
        ["hyper"] = "Hyper Player",
        ["fast"] = "Fast Player",
        ["slow"] = "Slow Player",
        ["freeze"] = "Freeze Player",
        ["fastjetpack"] = "Fast Player Jetpacks",
        ["drunk"] = "Drunk Player",
        ["jumpultra"] = "Ultra Jump",
        ["jumphigh"] = "High Jump",
        ["jumplow"] = "Low Jump",
        ["revive"] = "Revive Dead Players",
        ["spawn_pede"] = "Spawn Centipede",
        ["spawn_cracker"] = "Spawn Nutcracker",
        ["spawn_spider"] = "Spawn Bunker Spider",
        ["spawn_hoard"] = "Spawn Hoarding Bug",
        ["spawn_flower"] = "Spawn Bracken",
        ["spawn_crawl"] = "Spawn Crawler",
        ["spawn_blob"] = "Spawn Blob",
        ["spawn_spring"] = "Spawn Coil-Head",
        ["spawn_puff"] = "Spawn Puffer",
        ["spawn_dog"] = "Spawn Eyeless Dog",
        ["spawn_giant"] = "Spawn Forest Giant",
        ["spawn_levi"] = "Spawn Earth Leviathan",
        ["spawn_hawk"] = "Spawn Baboon Hawk",
        ["spawn_girl"] = "Spawn Ghost Girl",
        ["spawn_mimic"] = "Spawn Mimic",
        ["spawn_landmine"] = "Spawn Landmine",
        ["spawn_turret"] = "Spawn Turret",
        ["spawn_spiketrap"] = "Spawn Spike Trap",
        ["spawn_radmech"] = "Spawn Old Bird",
        ["spawn_clay"] = "Spawn Clay Surgeon",
        ["spawn_butler"] = "Spawn Butler",
        ["spawn_jester"] = "Spawn Jester",
        ["spawn_eater"] = "Spawn Maneater",
        ["spawn_snake"] = "Spawn Tulip Snake",
        ["spawn_kiwi"] = "Spawn Sapsucker",
        ["spawn_growths"] = "Spawn Cadaver Growths",
        ["spawn_bloom"] = "Spawn Cadaver Bloom",
        ["spawn_bush"] = "Spawn Kidnapper Fox",
        ["spawn_pum"] = "Spawn Feiopar",
        ["spawn_ray"] = "Spawn Stingray",
        ["cspawn_pede"] = "Spawn Centipede at Crewmate",
        ["cspawn_cracker"] = "Spawn Nutcracker at Crewmate",
        ["cspawn_spider"] = "Spawn Bunker Spider at Crewmate",
        ["cspawn_hoard"] = "Spawn Hoarding Bug at Crewmate",
        ["cspawn_flower"] = "Spawn Bracken at Crewmate",
        ["cspawn_crawl"] = "Spawn Crawler at Crewmate",
        ["cspawn_blob"] = "Spawn Blob at Crewmate",
        ["cspawn_spring"] = "Spawn Coil-Head at Crewmate",
        ["cspawn_puff"] = "Spawn Puffer at Crewmate",
        ["cspawn_dog"] = "Spawn Eyeless Dog at Crewmate",
        ["cspawn_giant"] = "Spawn Forest Giant at Crewmate",
        ["cspawn_levi"] = "Spawn Earth Leviathan at Crewmate",
        ["cspawn_hawk"] = "Spawn Baboon Hawk at Crewmate",
        ["cspawn_girl"] = "Spawn Ghost Girl at Crewmate",
        ["cspawn_mimic"] = "Spawn Mimic at Crewmate",
        ["cspawn_landmine"] = "Spawn Landmine at Crewmate",
        ["cspawn_turret"] = "Spawn Turret at Crewmate",
        ["cspawn_spiketrap"] = "Spawn Spike Trap at Crewmate",
        ["cspawn_radmech"] = "Spawn Old Bird at Crewmate",
        ["cspawn_clay"] = "Spawn Clay Surgeon at Crewmate",
        ["cspawn_butler"] = "Spawn Butler at Crewmate",
        ["cspawn_jester"] = "Spawn Jester at Crewmate",
        ["cspawn_eater"] = "Spawn Maneater at Crewmate",
        ["cspawn_snake"] = "Spawn Tulip Snake at Crewmate",
        ["cspawn_kiwi"] = "Spawn Sapsucker at Crewmate",
        ["cspawn_growths"] = "Spawn Cadaver Growths at Crewmate",
        ["cspawn_bloom"] = "Spawn Cadaver Bloom at Crewmate",
        ["cspawn_bush"] = "Spawn Kidnapper Fox at Crewmate",
        ["cspawn_pum"] = "Spawn Feiopar at Crewmate",
        ["cspawn_ray"] = "Spawn Stingray at Crewmate",
        ["give_binoculars"] = "Give Binoculars",
        ["give_boombox"] = "Give Boombox",
        ["give_flashlight"] = "Give Flashlight",
        ["give_jetpack"] = "Give Jetpack",
        ["give_key"] = "Give Key",
        ["give_lockpicker"] = "Give Lockpicker",
        ["give_lungapparatus"] = "Give Apparatus",
        ["give_mapdevice"] = "Give Mapper",
        ["give_proflashlight"] = "Give Pro-Flashlight",
        ["give_shovel"] = "Give Shovel",
        ["give_stungrenade"] = "Give Stun Grenade",
        ["give_extensionladder"] = "Give Extension Ladder",
        ["give_tzpinhalant"] = "Give TZP Inhalant",
        ["give_walkietalkie"] = "Give Walkie Talkie",
        ["give_zapgun"] = "Give Zap Gun",
        ["give_7ball"] = "Give Magic 7 Ball",
        ["give_airhorn"] = "Give Airhorn",
        ["give_bottlebin"] = "Give Bottles",
        ["give_clownhorn"] = "Give Clown Horn",
        ["give_goldbar"] = "Give Gold Bar",
        ["give_stopsign"] = "Give Stop Sign",
        ["give_radarbooster"] = "Give Radar Booster",
        ["give_yieldsign"] = "Give Yield Sign",
        ["give_shotgun"] = "Give Shotgun",
        ["give_gunAmmo"] = "Give Shells",
        ["give_spraypaint"] = "Give Spray Paint",
        ["give_giftbox"] = "Give Present",
        ["give_tragedymask"] = "Give Tragedy Mask",
        ["give_comedymask"] = "Give Comedy Mask",
        ["give_knife"] = "Give Kitchen Knife",
        ["give_easteregg"] = "Give Easter Egg",
        ["give_weedkillerbottle"] = "Give Weed Killer",
        ["cgive_binoculars"] = "Give Crewmate Binoculars",
        ["cgive_boombox"] = "Give Crewmate Boombox",
        ["cgive_flashlight"] = "Give Crewmate Flashlight",
        ["cgive_jetpack"] = "Give Crewmate Jetpack",
        ["cgive_key"] = "Give Crewmate Key",
        ["cgive_lockpicker"] = "Give Crewmate Lockpicker",
        ["cgive_lungapparatus"] = "Give Crewmate Apparatus",
        ["cgive_mapdevice"] = "Give Crewmate Mapper",
        ["cgive_proflashlight"] = "Give Crewmate Pro-Flashlight",
        ["cgive_shovel"] = "Give Crewmate Shovel",
        ["cgive_stungrenade"] = "Give Crewmate Stun Grenade",
        ["cgive_extensionladder"] = "Give Crewmate Extension Ladder",
        ["cgive_tzpinhalant"] = "Give Crewmate TZP Inhalant",
        ["cgive_walkietalkie"] = "Give Crewmate Walkie Talkie",
        ["cgive_zapgun"] = "Give Crewmate Zap Gun",
        ["cgive_7ball"] = "Give Crewmate Magic 7 Ball",
        ["cgive_airhorn"] = "Give Crewmate Airhorn",
        ["cgive_bottlebin"] = "Give Crewmate Bottles",
        ["cgive_clownhorn"] = "Give Crewmate Clown Horn",
        ["cgive_goldbar"] = "Give Crewmate Gold Bar",
        ["cgive_stopsign"] = "Give Crewmate Stop Sign",
        ["cgive_radarbooster"] = "Give Crewmate Radar Booster",
        ["cgive_yieldsign"] = "Give Crewmate Yield Sign",
        ["cgive_shotgun"] = "Give Crewmate Shotgun",
        ["cgive_gunAmmo"] = "Give Crewmate Shells",
        ["cgive_spraypaint"] = "Give Crewmate Spray Paint",
        ["cgive_giftbox"] = "Give Crewmate Present",
        ["cgive_tragedymask"] = "Give Crewmate Tragedy Mask",
        ["cgive_comedymask"] = "Give Crewmate Comedy Mask",
        ["cgive_knife"] = "Give Crewmate Kitchen Knife",
        ["cgive_easteregg"] = "Give Crewmate Easter Egg",
        ["cgive_weedkillerbottle"] = "Give Crewmate Weed Killer",
        ["charge"] = "Charge Item Battery",
        ["uncharge"] = "Drain Item Battery",
        ["breakerson"] = "Turn Breakers On",
        ["breakersoff"] = "Turn Breakers Off",
        ["killenemies"] = "Kill Nearby Enemies",
        ["spring_chair"] = "Spring Vehicle Chair",
        ["turn_off_engine"] = "Turn off Vehicle Engine",
        ["destroy_vehicle"] = "Destroy Vehicle",
        ["buy_cruiser"] = "Stock Company Cruiser",
        ["start_vehicle"] = "Start Vehicle Engine",
        ["buy_walkie"] = "Stock Walkie-Talkie",
        ["buy_flashlight"] = "Stock Flashlight",
        ["buy_shovel"] = "Stock Shovel",
        ["buy_lockpicker"] = "Stock Lockpicker",
        ["buy_proflashlight"] = "Stock Pro Flashlight",
        ["buy_stungrenade"] = "Stock Stun Grenade",
        ["buy_boombox"] = "Stock Boom Box",
        ["buy_inhaler"] = "Stock Inhaler",
        ["buy_stungun"] = "Stock Stun Gun",
        ["buy_jetpack"] = "Stock Jet Pack",
        ["buy_extensionladder"] = "Stock Extension Ladder",
        ["buy_radarbooster"] = "Stock Radar Booster",
        ["buy_spraypaint"] = "Stock Spray Paint",
        ["buy_weedkiller"] = "Stock Weed Killer",
        ["takeitem"] = "Take Held Item",
        ["dropitem"] = "Drop Held Item",
        ["takecrewitem"] = "Take Crewmate Held Item",
        ["weather_-1"] = "Weather - Clear",
        ["weather_0"] = "Weather - Cloudy",
        ["weather_1"] = "Weather - Rainy",
        ["weather_2"] = "Weather - Stormy",
        ["weather_3"] = "Weather - Foggy",
        ["weather_4"] = "Weather - Flooded",
        ["weather_5"] = "Weather - Eclipsed",
        ["givecred_5"] = "Give 5 Credits",
        ["givecred_50"] = "Give 50 Credits",
        ["givecred_500"] = "Give 500 Credits",
        ["givecred_-5"] = "Take 5 Credits",
        ["givecred_-50"] = "Take 50 Credits",
        ["givecred_-500"] = "Take 500 Credits",
        ["addscrap"] = "Spawn Scrap in Level",
        ["givequota_5"] = "Raise Quota by 5",
        ["givequota_50"] = "Raise Quota by 50",
        ["givequota_500"] = "Raise Quota by 500",
        ["givequota_-5"] = "Lower Quota by 5",
        ["givequota_-50"] = "Lower Quota by 50",
        ["givequota_-500"] = "Lower Quota by 500",
        ["giveprofit_25"] = "Fill Quota by 25%",
        ["giveprofit_50"] = "Fill Quota by 50%",
        ["giveprofit_100"] = "Complete Quota",
        ["giveprofit_-25"] = "Empty Quota by 25%",
        ["giveprofit_-50"] = "Empty Quota by 50%",
        ["giveprofit_-100"] = "Reset Quota Progress",
        ["toship"] = "Return to Ship",
        ["crewship"] = "Return Crewmate to Ship",
        ["tocrew"] = "Teleport to Crewmate",
        ["crewto"] = "Teleport Crewmate Here",
        ["body"] = "Spawn Dead Body",
        ["crewbody"] = "Spawn Dead Crewmate",
        ["inverse"] = "Inverse Teleport",
        ["cinverse"] = "Inverse Teleport Random Crewmate",
        ["screech"] = "Play Bird Screech",
        ["footstep"] = "Play Footstep",
        ["breathing"] = "Play Breathing",
        ["addhour"] = "Advance Time One Hour",
        ["remhour"] = "Rollback Time One Hour",
        ["addday"] = "Extend Deadline by One Day",
        ["remday"] = "Shorten Deadline by One Day",
        ["shipleave"] = "Ship Leaves Early",
        ["closedoors"] = "Close Ship Doors",
        ["opendoors"] = "Open Ship Doors",
    };

    /// <summary>The display name for an effect code.</summary>
    /// <remarks>
    /// The fallback handles both naming styles effect codes come in: "speedUp" becomes "Speed Up",
    /// "stat_up_speed" becomes "Stat Up Speed".
    /// </remarks>
    public static string Pretty(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return "Effect";
        if (Names.TryGetValue(code, out string name)) return name;

        StringBuilder sb = new(code.Length + 4);
        bool startOfWord = true;
        foreach (char c in code)
        {
            if (c is '_' or '-')
            {
                sb.Append(' ');
                startOfWord = true;
                continue;
            }

            if (!startOfWord && char.IsUpper(c)) sb.Append(' ');

            sb.Append(startOfWord ? char.ToUpperInvariant(c) : c);
            startOfWord = false;
        }

        return sb.ToString();
    }
}
