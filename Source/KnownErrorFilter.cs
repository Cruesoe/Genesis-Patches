using System;
using Verse;

namespace GenesisPatches
{
    // Turns known, harmless errors and warnings from other mods into info lines; everything else passes through unchanged
    public static class KnownErrorFilter
    {
        // Each entry: every fragment must appear in the message. Remove an entry once its mod is fixed.
        private static readonly string[][] KnownErrors =
        {
            // Better Architect Menu gates its Vehicle Map Framework folder with IfModActiveAny, which RimWorld ignores (reported to ferny, 261116)
            new[] { "Better Architect Menu", "DesignationCategoryDef", "VMF_VehicleMap" },
            // TD Find Lib - Continued looks for a Float Sub-Menus compatibility assembly that it doesn't ship (261115)
            new[] { "Float Sub-Menu is installed, but TD Compatibility for it isn't" },
            // Nuff's Auto-Patcher ships a 1.6 Vehicle Framework add-on built against a newer core, so every vehicle it patches throws (261118)
            new[] { "nuff.AutoPatcherCombatExtended.DefDataHolder.StartNewLogEntry" },
        };

        // Set only while this call logs its info line, so a re-entered Log.Error passes through; reset in finally
        [ThreadStatic] private static bool inFilter;

        public static bool Prefix(string text)
        {
            if (inFilter || text == null) return true;
            var match = Match(text);
            if (match == null) return true;
            inFilter = true;
            try
            {
                Log.Message("[Genesis Patches] Known error hidden: " + LineWith(text, match[0]));
            }
            finally
            {
                inFilter = false;
            }
            return false;
        }

        // The first entry whose fragments all appear in the message, or null
        private static string[]? Match(string text)
        {
            foreach (var fragments in KnownErrors)
            {
                var all = true;
                foreach (var fragment in fragments)
                {
                    if (text.IndexOf(fragment, StringComparison.Ordinal) < 0) { all = false; break; }
                }
                if (all) return fragments;
            }
            return null;
        }

        // The line of the message that contains the fragment (messages can start with a header line)
        private static string LineWith(string text, string fragment)
        {
            var at = text.IndexOf(fragment, StringComparison.Ordinal);
            var start = text.LastIndexOf('\n', at) + 1;
            var end = text.IndexOf('\n', at);
            return (end < 0 ? text.Substring(start) : text.Substring(start, end - start)).Trim();
        }
    }
}
