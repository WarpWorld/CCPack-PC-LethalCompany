#nullable disable
using ConnectorLib.JSON;

namespace CrowdControl.Delegates.Effects.Implementations;

// Instant effects. Each class forwards to the matching method in GameActions, which holds the game logic
// (kept from the previous version of the mod). Effect IDs must match LethalCompany.cs.

[Effect(["heal_full"])]
public class HealFullEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.HealFull(request);
}

[Effect(["kill"])]
public class KillEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Kill(request);
}

[Effect(["killcrew"])]
public class KillCrewmateEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.KillCrewmate(request);
}

[Effect(["damage"])]
public class DamageEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Damage(request);
}

[Effect(["damagecrew"])]
public class DamageCrewEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.DamageCrew(request);
}

[Effect(["heal"])]
public class HealEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Heal(request);
}

[Effect(["healcrew"])]
public class HealCrewEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.HealCrew(request);
}

[Effect(["launch"])]
public class LaunchEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Launch(request);
}

[Effect(["drain"])]
public class DrainStaminsEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.DrainStamins(request);
}

[Effect(["restore"])]
public class RestoreStaminsEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.RestoreStamins(request);
}

[Effect(["spawn_pede", "spawn_spider", "spawn_hoard", "spawn_flower", "spawn_crawl", "spawn_blob", "spawn_spring", "spawn_puff", "spawn_dog", "spawn_giant", "spawn_levi", "spawn_hawk", "spawn_girl", "spawn_mimic", "spawn_cracker", "spawn_landmine", "spawn_turret", "spawn_spiketrap", "spawn_radmech", "spawn_clay", "spawn_butler", "spawn_jester", "spawn_eater", "spawn_snake", "spawn_kiwi", "spawn_growths", "spawn_bloom", "spawn_bush", "spawn_pum", "spawn_ray"])]
public class SpawnEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Spawn(request);
}

[Effect(["webs"])]
public class CreateWebsEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.CreateWebs(request);
}

[Effect(["killenemies"])]
public class KillEnemiesEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.KillEnemies(request);
}

[Effect(["cspawn_pede", "cspawn_spider", "cspawn_hoard", "cspawn_flower", "cspawn_crawl", "cspawn_blob", "cspawn_spring", "cspawn_puff", "cspawn_dog", "cspawn_giant", "cspawn_levi", "cspawn_hawk", "cspawn_girl", "cspawn_cracker", "cspawn_mimic", "cspawn_landmine", "cspawn_turret", "cspawn_spiketrap", "cspawn_radmech", "cspawn_butler", "cspawn_clay", "cspawn_jester", "cspawn_eater", "cspawn_snake", "cspawn_kiwi", "cspawn_growths", "cspawn_bloom", "cspawn_bush", "cspawn_pum", "cspawn_ray"])]
public class CrewSpawnEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.CrewSpawn(request);
}

[Effect(["give_binoculars", "give_boombox", "give_flashlight", "give_jetpack", "give_key", "give_lockpicker", "give_lungapparatus", "give_mapdevice", "give_proflashlight", "give_shovel", "give_stungrenade", "give_extensionladder", "give_tzpinhalant", "give_walkietalkie", "give_zapgun", "give_7ball", "give_airhorn", "give_bottlebin", "give_clownhorn", "give_goldbar", "give_stopsign", "give_radarbooster", "give_yieldsign", "give_shotgun", "give_gunAmmo", "give_spraypaint", "give_giftbox", "give_tragedymask", "give_comedymask", "give_knife", "give_easteregg", "give_weedkillerbottle"])]
public class GiveItemEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.GiveItem(request);
}

[Effect(["cgive_binoculars", "cgive_boombox", "cgive_flashlight", "cgive_jetpack", "cgive_key", "cgive_lockpicker", "cgive_lungapparatus", "cgive_mapdevice", "cgive_proflashlight", "cgive_shovel", "cgive_stungrenade", "cgive_extensionladder", "cgive_tzpinhalant", "cgive_walkietalkie", "cgive_zapgun", "cgive_7ball", "cgive_airhorn", "cgive_bottlebin", "cgive_clownhorn", "cgive_goldbar", "cgive_stopsign", "cgive_radarbooster", "cgive_yieldsign", "cgive_shotgun", "cgive_gunAmmo", "cgive_spraypaint", "cgive_giftbox", "cgive_tragedymask", "cgive_comedymask", "cgive_knife", "cgive_easteregg", "cgive_weedkillerbottle"])]
public class GiveCrewItemEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.GiveCrewItem(request);
}

[Effect(["weather_-1", "weather_1", "weather_2", "weather_3", "weather_4", "weather_5", "weather_0"])]
public class WeatherEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Weather(request);
}

[Effect(["lightning"])]
public class LightningEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Lightning(request);
}

[Effect(["takeitem"])]
public class TakeItemEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.TakeItem(request);
}

[Effect(["dropitem"])]
public class DropItemEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.DropItem(request);
}

[Effect(["takecrewitem"])]
public class TakeCrewItemEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.TakeCrewItem(request);
}

[Effect(["buy_walkie", "buy_flashlight", "buy_shovel", "buy_lockpicker", "buy_proflashlight", "buy_stungrenade", "buy_boombox", "buy_inhaler", "buy_stungun", "buy_jetpack", "buy_extensionladder", "buy_radarbooster", "buy_spraypaint", "buy_weedkiller"])]
public class BuyItemEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.BuyItem(request);
}

[Effect(["buy_cruiser"])]
public class BuyCruiserEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.BuyCruiser(request);
}

[Effect(["turn_off_engine"])]
public class TurnOffEngineEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.TurnOffEngine(request);
}

[Effect(["destroy_vehicle"])]
public class DestroyVehicleEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.DestroyVehicle(request);
}

[Effect(["start_vehicle"])]
public class TurnOnVehicleEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.TurnOnVehicle(request);
}

[Effect(["spring_chair"])]
public class SpringChairEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.SpringChair(request);
}

[Effect(["charge"])]
public class ChargeItemEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.ChargeItem(request);
}

[Effect(["uncharge"])]
public class UnchargeItemEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.UnchargeItem(request);
}

[Effect(["breakerson"])]
public class BreakersOnEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.BreakersOn(request);
}

[Effect(["breakersoff"])]
public class BreakersOffEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.BreakersOff(request);
}

[Effect(["toship"])]
public class TeleportToShipEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.TeleportToShip(request);
}

[Effect(["crewship"])]
public class TeleportCrewToShipEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.TeleportCrewToShip(request);
}

[Effect(["body"])]
public class SpawnBodyEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.SpawnBody(request);
}

[Effect(["crewbody"])]
public class SpawnCrewBodyEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.SpawnCrewBody(request);
}

[Effect(["revive"])]
public class ReviveEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Revive(request);
}

[Effect(["tocrew"])]
public class TeleportToCrewEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.TeleportToCrew(request);
}

[Effect(["crewto"])]
public class TeleportCrewToEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.TeleportCrewTo(request);
}

[Effect(["screech"])]
public class ScreechEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Screech(request);
}

[Effect(["footstep"])]
public class FootstepEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Footstep(request);
}

[Effect(["breathing"])]
public class BreathingEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Breathing(request);
}

[Effect(["ghost"])]
public class GhostEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.Ghost(request);
}

[Effect(["horn"])]
public class PlayHornEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.PlayHorn(request);
}

[Effect(["blob"])]
public class BlobSoundEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.BlobSound(request);
}

[Effect(["addhour"])]
public class AddHourEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.AddHour(request);
}

[Effect(["remhour"])]
public class RemoveHourEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.RemoveHour(request);
}

[Effect(["addday"])]
public class AddDayEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.AddDay(request);
}

[Effect(["remday"])]
public class RemoveDayEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.RemoveDay(request);
}

[Effect(["givecred_5", "givecred_50", "givecred_500", "givecred_-5", "givecred_-50", "givecred_-500"])]
public class AddCreditsEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.AddCredits(request);
}

[Effect(["givequota_5", "givequota_50", "givequota_500", "givequota_-5", "givequota_-50", "givequota_-500"])]
public class AddQuotaEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.AddQuota(request);
}

[Effect(["giveprofit_25", "giveprofit_50", "giveprofit_100", "giveprofit_-25", "giveprofit_-50", "giveprofit_-100"])]
public class AddProfitEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.AddProfit(request);
}

[Effect(["addscrap"])]
public class AddScrapEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.AddScrap(request);
}

[Effect(["shipleave"])]
public class ShipLeaveEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.ShipLeave(request);
}

[Effect(["opendoors"])]
public class OpenDoorsEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.OpenDoors(request);
}

[Effect(["closedoors"])]
public class CloseDoorsEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.CloseDoors(request);
}

[Effect(["inverse"])]
public class InverseTeleportEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.InverseTeleport(request);
}

[Effect(["cinverse"])]
public class InverseTeleportCrewEffect(CrowdControlMod mod, NetworkClient client) : Effect(mod, client)
{
    public override EffectResponse Start(EffectRequest request) => GameActions.InverseTeleportCrew(request);
}
