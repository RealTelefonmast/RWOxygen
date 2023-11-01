using HarmonyLib;
using RimWorld;
using TAC;
using TeleCore;
using TROxygen.Oxygen;
using Verse;

namespace TROxygen.Patches;

internal static class CompPatches
{
    [HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.HasFuel), MethodType.Getter)]
    private static class CompRefuelable_HasFuel_Patch
    {
        private static void Postfix(CompRefuelable __instance, ref bool __result)
        {
            if (!__result) return;
            if (__instance.parent is not ThingWithComps thing) return;
            if (OxygenUtility.IsFuelBurner(thing))
            {
                var conv = thing.Map.Atmosphere().Converters.ConverterFor<OxygenBurner>(thing);
                __result &= conv.IsActive;//.HasOxygen(thing);
            }
        }
    }
}