using System;
using Verse;

namespace GenesisPatches
{
    // Turns known, harmless errors from other mods into info lines; every other error passes through unchanged
    public static class KnownErrorFilter
    {
        // Each entry: every fragment must appear in the message. Remove an entry once its mod is fixed.
        private static readonly string[][] KnownErrors =
        {
            // Better Architect Menu gates its Vehicle Map Framework folder with IfModActiveAny, which RimWorld ignores (reported to ferny, 261116)
            new[] { "DesignationCategoryDef", "VMF_VehicleMap" },
            // TD Find Lib - Continued looks for a Float Sub-Menus compatibility assembly that it doesn't ship (261115)
            new[] { "Float Sub-Menu is installed, but TD Compatibility for it isn't" },
        };

        [ThreadStatic] private static bool inFilter;

        public static bool Prefix(string text)
        {
            if (inFilter || text == null || !IsKnown(text)) return true;
            inFilter = true;
            try
            {
                Log.Message("[Genesis Patches] Known error hidden: " + FirstLine(text));
            }
            finally
            {
                inFilter = false;
            }
            return false;
        }

        private static bool IsKnown(string text)
        {
            foreach (var fragments in KnownErrors)
            {
                var all = true;
                foreach (var fragment in fragments)
                {
                    if (text.IndexOf(fragment, StringComparison.Ordinal) < 0) { all = false; break; }
                }
                if (all) return true;
            }
            return false;
        }

        private static string FirstLine(string text)
        {
            var end = text.IndexOf('\n');
            return end < 0 ? text : text.Substring(0, end);
        }
    }
}
