using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace BepinControl.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
public class PlayerControllerBPatch
{

    [HarmonyPatch("Start")]
    [HarmonyPrefix]
    static void getNightVision(ref PlayerControllerB __instance)
    {
        Mod.playerRef = __instance;
        Mod.nightVision = Mod.playerRef.nightVision.enabled;
        // store nightvision values
        Mod.nightVisionIntensity = Mod.playerRef.nightVision.intensity;
        Mod.nightVisionColor = Mod.playerRef.nightVision.color;
        Mod.nightVisionRange = Mod.playerRef.nightVision.range;

        Mod.playerRef.nightVision.color = UnityEngine.Color.green;
        Mod.playerRef.nightVision.intensity = 1000f;
        Mod.playerRef.nightVision.range = 10000f;
    }

    [HarmonyPatch("SetNightVisionEnabled")]
    [HarmonyPostfix]
    static void updateNightVision()
    {
        //instead of enabling/disabling nightvision, set the variables

        if (Mod.nightVision)
        {
            Mod.playerRef.nightVision.color = UnityEngine.Color.green;
            Mod.playerRef.nightVision.intensity = 1000f;
            Mod.playerRef.nightVision.range = 10000f;
        }
        else
        {
            Mod.playerRef.nightVision.color = Mod.nightVisionColor;
            Mod.playerRef.nightVision.intensity = Mod.nightVisionIntensity;
            Mod.playerRef.nightVision.range = Mod.nightVisionRange;
        }

        // should always be on
        Mod.playerRef.nightVision.enabled = true;
    }
        
    [HarmonyPatch("AllowPlayerDeath")]
    [HarmonyPrefix]
    static bool OverrideDeath()
    {
        if (!Mod.isHost) { return true; }
        return !Mod.enableGod;
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    static void InfiniteSprint(ref float ___sprintMeter)
    {
        if (Mod.infSprint && Mod.isHost) { Mathf.Clamp(___sprintMeter += 0.02f, 0f, 1f); }
    }
}