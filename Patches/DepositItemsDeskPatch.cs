using HarmonyLib;

namespace DoubleTrouble.Patches;

[HarmonyPatch(typeof(DepositItemsDesk))]
public class DepositItemsDeskPatch
{
    [HarmonyPatch("SellAndDisplayItemProfits")]
    [HarmonyPostfix]
    private static void SellAndDisplayItemProfits(int profit)
    {
        Behaviors.DoubleTroubleService.Instance.SoldAmount += profit;
    }
}