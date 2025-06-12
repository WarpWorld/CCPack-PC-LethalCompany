using BepinControl;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using UnityEngine;

namespace LethalCompanyTestMod.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class PlayerControllerBPatch
    {

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        static void getNightVision(ref PlayerControllerB __instance)
        {
            LethalCompanyControl.playerRef = __instance;
            LethalCompanyControl.nightVision = LethalCompanyControl.playerRef.nightVision.enabled;
            // store nightvision values
            LethalCompanyControl.nightVisionIntensity = LethalCompanyControl.playerRef.nightVision.intensity;
            LethalCompanyControl.nightVisionColor = LethalCompanyControl.playerRef.nightVision.color;
            LethalCompanyControl.nightVisionRange = LethalCompanyControl.playerRef.nightVision.range;

            LethalCompanyControl.playerRef.nightVision.color = UnityEngine.Color.green;
            LethalCompanyControl.playerRef.nightVision.intensity = 1000f;
            LethalCompanyControl.playerRef.nightVision.range = 10000f;
        }

        [HarmonyPatch("SetNightVisionEnabled")]
        [HarmonyPostfix]
        static void updateNightVision()
        {
            //instead of enabling/disabling nightvision, set the variables

            if (LethalCompanyControl.nightVision)
            {
                LethalCompanyControl.playerRef.nightVision.color = UnityEngine.Color.green;
                LethalCompanyControl.playerRef.nightVision.intensity = 1000f;
                LethalCompanyControl.playerRef.nightVision.range = 10000f;
            }
            else
            {
                LethalCompanyControl.playerRef.nightVision.color = LethalCompanyControl.nightVisionColor;
                LethalCompanyControl.playerRef.nightVision.intensity = LethalCompanyControl.nightVisionIntensity;
                LethalCompanyControl.playerRef.nightVision.range = LethalCompanyControl.nightVisionRange;
            }

            // should always be on
            LethalCompanyControl.playerRef.nightVision.enabled = true;
        }
        
        [HarmonyPatch("AllowPlayerDeath")]
        [HarmonyPrefix]
        static bool OverrideDeath()
        {
            if (!LethalCompanyControl.isHost) { return true; }
            return !LethalCompanyControl.enableGod;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void InfiniteSprint(ref float ___sprintMeter)
        {
            if (LethalCompanyControl.infSprint && LethalCompanyControl.isHost) { Mathf.Clamp(___sprintMeter += 0.02f, 0f, 1f); }
        }
    }
}