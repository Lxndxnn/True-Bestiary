using System;
using System.Reflection;
using HarmonyLib;
using Terraria.GameContent.Bestiary;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace TrueBestiary
{
    public class Mod : IMod
    {
        public string Id => "true-bestiary";
        public string Name => "True Bestiary";
        public string Version => "1.0.0";

        private ILogger _log;
        private TrueBestiaryConfig _config;
        private Harmony _harmony;

        private static bool RevealDrops;

        public void Initialize(ModContext context)
        {
            _log = context.Logger;
            _config = context.GetConfig<TrueBestiaryConfig>();

            RevealDrops = _config.RevealDropsAfterOneKill;

            _log.Info("Initializing True Bestiary...");

            try
            {
                _harmony = new Harmony(Id);

                MethodInfo target = AccessTools.Method(
                    typeof(CommonEnemyUICollectionInfoProvider),
                    "GetUnlockStateByKillCount",
                    new Type[]
                    {
                        typeof(int),
                        typeof(bool),
                        typeof(int)
                    }
                );

                if (target == null)
                {
                    _log.Error(
                        "FAILED: Could not find GetUnlockStateByKillCount."
                    );
                    return;
                }

                MethodInfo prefix = AccessTools.Method(
                    typeof(Mod),
                    nameof(GetUnlockStateByKillCount_Prefix)
                );

                _harmony.Patch(
                    target,
                    prefix: new HarmonyMethod(prefix)
                );

                _log.Info(
                    "SUCCESS: Bestiary unlock patch applied."
                );
            }
            catch (Exception ex)
            {
                _log.Error(
                    $"FAILED to patch Bestiary: {ex}"
                );
            }
        }

        private static bool GetUnlockStateByKillCount_Prefix(
            int killCount,
            bool quickUnlock,
            int fullKillCountNeeded,
            ref BestiaryEntryUnlockState __result)
        {
            if (!RevealDrops)
                return true;

            if (killCount >= 1)
            {
                __result =
                    BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;

                return false;
            }

            return true;
        }

        public void OnConfigChanged()
        {
            RevealDrops = _config.RevealDropsAfterOneKill;

            _log.Info(
                $"Reveal Drops After 1 Kill: {RevealDrops}"
            );
        }

        public void Unload()
        {
            _harmony?.UnpatchAll(Id);
            _harmony = null;

            _log?.Info("True Bestiary unloaded.");
        }
    }
}