#if RIMWORLD_1_7
using System;
using System.Runtime.InteropServices;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace GenesisPatches
{
    // Letter Stack Cleaner's fourth and fifth mouse button keys never fire on 1.7, so these keys also read the buttons from Windows
    public static class LetterStackCleanerMouseButtons
    {
        private const int VkXButton1 = 0x05;
        private const int VkXButton2 = 0x06;

        private static readonly bool[] wasDown = new bool[2];
        private static readonly bool[] pressed = new bool[2];
        private static int lastFrame = -1;

        public static void Apply(Harmony harmony)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                return;
            }
            var keyDown = AccessTools.Method("LetterStackCleaner.CleanManager:KeyDown");
            if (keyDown == null)
            {
                return;
            }
            harmony.Patch(keyDown, postfix: new HarmonyMethod(typeof(LetterStackCleanerMouseButtons), nameof(Postfix)));
        }

        public static void Postfix(KeyCode key, ref bool __result)
        {
            if (__result || (key != KeyCode.Mouse3 && key != KeyCode.Mouse4))
            {
                return;
            }
            Poll();
            __result = pressed[key == KeyCode.Mouse3 ? 0 : 1];
        }

        // Reads both buttons once per frame, so every GUI event in the frame sees the same press
        private static void Poll()
        {
            if (Time.frameCount == lastFrame)
            {
                return;
            }
            lastFrame = Time.frameCount;
            bool focused = GameHasFocus();
            Update(0, focused && IsDown(VkXButton1));
            Update(1, focused && IsDown(VkXButton2));
        }

        private static void Update(int index, bool down)
        {
            pressed[index] = down && !wasDown[index];
            wasDown[index] = down;
        }

        private static bool IsDown(int virtualKey) => (GetAsyncKeyState(virtualKey) & 0x8000) != 0;

        private static bool GameHasFocus()
        {
            IntPtr window = GetForegroundWindow();
            if (window == IntPtr.Zero)
            {
                return false;
            }
            GetWindowThreadProcessId(window, out uint processId);
            return processId == (uint)Environment.ProcessId;
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    }
}
#endif
