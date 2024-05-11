﻿
using HarmonyLib;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using xTile.Dimensions;

namespace LineSprinklersRedux.Framework
{
    internal static class BaseGamePatches
    {
        internal static void Apply (Harmony harmony)
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.GetSprinklerTiles)),
                    prefix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_GetSprinklerTiles_Prefix)),
                    postfix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_GetSprinklerTiles_Postfix))
    
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.IsSprinkler)),
                    postfix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_IsSprinkler_Postfix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.IsInSprinklerRangeBroadphase)),
                    prefix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_IsInSprinklerRangeBroadphase_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.GetModifiedRadiusForSprinkler)),
                    prefix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_GetModifiedRadiusForSprinkler_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.ApplySprinklerAnimation)),
                    prefix: new HarmonyMethod(typeof(BaseGamePatches), nameof(Object_ApplySprinklerAnimation_Prefix))
                    );
            } catch (Exception e)
            {
                ModEntry.IMonitor!.Log($"Could not Patch LineSprinklers: \n{e}", LogLevel.Error);
            }
        }

        private static bool Object_GetSprinklerTiles_Prefix(SObject __instance, ref List<Vector2> __result)
        {
            if (!Sprinkler.IsLineSprinkler(__instance))
            {
                return true;
            }
            __result = Sprinkler.GetCoverage(__instance).ToList();
            return false;
        }

        private static void Object_GetSprinklerTiles_Postfix(SObject __instance, ref List<Vector2> __result)
        {
            if (!Sprinkler.IsLineSprinkler(__instance))
            {
                return;
            }
            __result = Sprinkler.GetCoverage(__instance).ToList();
        }

        private static void Object_IsSprinkler_Postfix(SObject __instance, ref bool __result)
        {
            __result = __result || Sprinkler.IsLineSprinkler(__instance);
        }

        private static bool Object_IsInSprinklerRangeBroadphase_Prefix(SObject __instance, Vector2 target, ref bool __result)
        {
            if (!Sprinkler.IsLineSprinkler(__instance))
            {
                return true;
            }
            __result = Sprinkler.GetCoverage(__instance).ToList().Contains(target);
            return false;
        }

        private static bool Object_GetModifiedRadiusForSprinkler_Prefix(SObject __instance, ref int __result)
        {
            if (!Sprinkler.IsLineSprinkler(__instance))
            {
                return true;
            }
            // In `DayUpdate(), the base game ensures that the modified radius for the object is at least 0
            // as -1 is returned for non-sprinkler objects. I'm not sure why, as it's also checking `IsSprinkler`.
            __result = 0;
            return false;
        }

        private static bool Object_ApplySprinklerAnimation_Prefix(SObject __instance) {
            if (Sprinkler.IsLineSprinkler(__instance)) {
                Sprinkler.ApplySprinkler(__instance);
                return false;
            }
            return true;
        }
    }
}
