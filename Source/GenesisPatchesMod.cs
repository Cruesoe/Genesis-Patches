using HarmonyLib;
using RimWorld;
using Verse;

namespace GenesisPatches
{
    // A Mod subclass so the filter is in place before XML patches are applied (CreateModClasses runs before ApplyPatches)
    public sealed class GenesisPatchesMod : Mod
    {
        public GenesisPatchesMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("cruesoe.genesispatches");
            harmony.Patch(
                AccessTools.Method(typeof(Log), nameof(Log.Error), new[] { typeof(string) }),
                prefix: new HarmonyMethod(typeof(KnownErrorFilter), nameof(KnownErrorFilter.Prefix)));
            harmony.Patch(
                AccessTools.Method(typeof(Log), nameof(Log.Warning), new[] { typeof(string) }),
                prefix: new HarmonyMethod(typeof(KnownErrorFilter), nameof(KnownErrorFilter.Prefix)));
            harmony.Patch(
                AccessTools.Method(typeof(Alert_NeedResearchBench), nameof(Alert_NeedResearchBench.GetReport)),
                postfix: new HarmonyMethod(typeof(ResearchBenchAlertFilter), nameof(ResearchBenchAlertFilter.Postfix)));
        }
    }
}
