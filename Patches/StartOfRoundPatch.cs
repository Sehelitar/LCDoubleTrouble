using HarmonyLib;

namespace DoubleTrouble.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRoundPatch
{
    [HarmonyPatch("StartGame")]
    [HarmonyPostfix]
    private static void StartOfRoundStartGame()
    {
        // Début de partie (landing)
        Behaviors.DoubleTroubleService.Instance?.BeginEvent();
    }
    
    [HarmonyPatch("ShipLeave")]
    [HarmonyPostfix]
    private static void StartOfRoundShipLeave()
    {
        // Fin de la partie
        Behaviors.DoubleTroubleService.Instance?.EndEvent();
    }

    [HarmonyPatch("ShipLeaveAutomatically")]
    [HarmonyPostfix]
    private static void StartOfRoundShipLeaveAutomatically()
    {
        // Fin de la partie (autoleave)
        Behaviors.DoubleTroubleService.Instance?.EndEvent();
    }
}