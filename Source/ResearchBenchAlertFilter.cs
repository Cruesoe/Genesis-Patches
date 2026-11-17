using System.Collections.Generic;
using RimWorld;
using Verse;

namespace GenesisPatches
{
    // Hides the "Need research bench" alert until the colony has unlocked a research bench it could build for the current project
    public static class ResearchBenchAlertFilter
    {
        // Player-buildable research benches; Defs are fixed after load, so this lives for the session
        private static List<ThingDef>? benches;

        public static void Postfix(ref AlertReport __result)
        {
            if (!__result.active)
            {
                return;
            }
            ResearchProjectDef? project = Find.ResearchManager.GetProject();
            if (project == null)
            {
                return;
            }
            if (project.requiredResearchBuilding != null)
            {
                if (!project.requiredResearchBuilding.IsResearchFinished)
                {
                    __result = false;
                }
                return;
            }
            if (!AnyBenchUnlocked())
            {
                __result = false;
            }
        }

        private static bool AnyBenchUnlocked()
        {
            benches ??= FindBenches();
            for (int i = 0; i < benches.Count; i++)
            {
                if (benches[i].IsResearchFinished)
                {
                    return true;
                }
            }
            return false;
        }

        private static List<ThingDef> FindBenches()
        {
            var list = new List<ThingDef>();
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (def.BuildableByPlayer && def.thingClass != null && typeof(Building_ResearchBench).IsAssignableFrom(def.thingClass))
                {
                    list.Add(def);
                }
            }
            return list;
        }
    }
}
