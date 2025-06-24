using HarmonyLib;

namespace XRL.World.Parts
{
    [HarmonyPatch(typeof(XRL.World.Parts.GivesRep))]
    class Cattlesquat_YouSpotALegendaryPatcher
    {
        [HarmonyPatch("Register")]
        static void Postfix(XRL.World.Parts.GivesRep __instance)
        {
            // Patch the GivesRep part (which every legendary has) to add our helper part
            __instance.ParentObject?.RequirePart<XRL.World.Parts.Cattlesquat_YouSpotALegendary>();
        }
    }
}
