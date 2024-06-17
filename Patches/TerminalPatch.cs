using HarmonyLib;

namespace DoubleTrouble.Patches;

[HarmonyPatch(typeof(Terminal))]
internal class TerminalPatch
{
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    private static void TerminalAwake(ref Terminal __instance)
    {
        // Ajout du Quitte ou double
        __instance.gameObject.AddComponent<Behaviors.DoubleTroubleService>();
    }
}