using DoubleTrouble.Behaviors;
using HarmonyLib;

namespace DoubleTrouble.Patches;

[HarmonyPatch(typeof(DepositItemsDesk))]
public class DepositItemsDeskPatch
{
    [HarmonyPatch("SellAndDisplayItemProfits")]
    [HarmonyPostfix]
    private static void SellAndDisplayItemProfits(int profit)
    {
        if(DoubleTroubleService.Instance?.IsActive == true)
            DoubleTroubleService.Instance.SoldAmount += profit;
    }
}