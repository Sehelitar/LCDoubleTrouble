using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Netcode;
using UnityEngine;

namespace DoubleTrouble.Behaviors;

[RequireComponent(typeof(Terminal))]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal class DoubleTroubleService : NetworkBehaviour
{
    public bool IsActive { get; private set; } = false;
    public bool IsOverall { get; private set; } = true;
    public bool ProbabilityOverride { get; set; } = false;
    public int SoldAmount { get; internal set; } = 0;

    public static DoubleTroubleService Instance { get; private set; } = null;
    
    private void Awake()
    {
        Instance = this;
    }

    internal void BeginEvent()
    {
        // Launch a Double Trouble event only when all conditions are met
        if (!IsServer ||
            !Plugin.GameConfig.DoubleTroubleEnable.Value ||
            RoundManager.Instance.currentLevel.planetHasTime ||
            (!ProbabilityOverride && !ShouldLaunchEvent())) return;
        
        // DOUBLE TROUBLE !!!
        IsActive = true;
        IsOverall = Plugin.GameConfig.DoubleTroubleOverall.Value;
        ProbabilityOverride = false;
        SoldAmount = 0;
        AnnounceEventClientRpc(IsOverall);
    }

    private static bool ShouldLaunchEvent()
    {
        var threshold = Plugin.GameConfig.DoubleTroubleProbability.Value;
        var luck = UnityEngine.Random.RandomRangeInt(0, 100);
        return luck <= threshold;
    }

    internal void EndEvent()
    {
        if (!IsServer || !IsActive) return;
        IsActive = false;

        const double threshold = 0.5d;
        var luck = UnityEngine.Random.Range(0f, 1f);
        var winner = luck >= threshold;
        var terminal = FindObjectOfType<Terminal>();
        var isOverall = Plugin.GameConfig.DoubleTroubleOverall.Value;
        var before = terminal.groupCredits;
        
        if (isOverall)
        {
            if (winner)
                terminal.groupCredits *= 2;
            else
                terminal.groupCredits /= 2;
        }
        else
        {
            if (winner)
                terminal.groupCredits += SoldAmount;
            else
                terminal.groupCredits -= Convert.ToInt32(SoldAmount / 2f);
        }

        AnnounceResultClientRpc(before, terminal.groupCredits);
    }

    [ClientRpc]
    private void AnnounceEventClientRpc(bool isOverall)
    {
        HUDManager.Instance?.DisplayTip(
            Locale.Title,
            isOverall
                ? Locale.LandingInfoOverall
                : Locale.LandingInfoDiff
            );
    }

    [ClientRpc]
    private void AnnounceResultClientRpc(int amountBefore, int amountAfter)
    {
        GetComponent<Terminal>().groupCredits = amountAfter;
        HUDManager.Instance?.DisplayTip(
            Locale.Title,
            amountBefore > amountAfter
                ? string.Format(Locale.ResultLose, amountAfter - amountBefore)
                : string.Format(Locale.ResultWin, amountAfter - amountBefore)
            );
        HUDManager.Instance?.DisplaySpectatorTip(amountBefore > amountAfter
            ? string.Format(Locale.SpectateResultLose, amountAfter - amountBefore)
            : string.Format(Locale.SpectateResultWin,  amountAfter - amountBefore));
    }
}