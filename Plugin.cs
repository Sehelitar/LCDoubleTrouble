using System;
using System.Globalization;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace DoubleTrouble;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("Lethal Company.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    internal static PluginConfigStruct GameConfig { get; private set; }
    private static Harmony _globalHarmony = null!;

    private void Awake()
    {
        Log = Logger;

        // Plugin startup logic
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

        // Apply Evaisa's NetworkPatcher
        PatchNetwork();

        _globalHarmony = new Harmony(PluginInfo.PLUGIN_NAME);
        _globalHarmony.PatchAll();

        GameConfig = new PluginConfigStruct
        {
            DoubleTroubleEnable = Config.Bind("DoubleTrouble", "Enable", true, @"Active l'évènement du ""quitte ou double""."),
            DoubleTroubleProbability = Config.Bind("DoubleTrouble", "Probability", 20d, @"Pourcentage de change qu'un évènement ""quitte ou double"" se lance (entre 0 et 100)."),
            DoubleTroubleOverall = Config.Bind("DoubleTrouble", "Overall", true, "Si activé, le quitte ou double se basera sur le nombre total de crédits au lieu de ce qui vient d'être vendu."),
        };
    }

    private static void PatchNetwork()
    {
        try
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes =
                        method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length <= 0) continue;
                    Log.LogInfo("Initialize network patch for " + type.FullName);
                    method.Invoke(null, null);
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}

internal struct PluginConfigStruct
{
    public ConfigEntry<bool> DoubleTroubleEnable;
    public ConfigEntry<double> DoubleTroubleProbability;
    public ConfigEntry<bool> DoubleTroubleOverall;
}