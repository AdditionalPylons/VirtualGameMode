﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WM = VirtualGameMode.Native.WM;
using VK = VirtualGameMode.Native.VK;

namespace VirtualGameMode
{
    public static class GameModeHook
    {
        private static int _hook = 0;
        private static Native.HookProc hookfn = new Native.HookProc(KeyboardProc);
        public static void InstallHook()
        {
            _hook = Native.SetWindowsHookEx(Native.WH_KEYBOARD_LL, hookfn, IntPtr.Zero, 0);
        }

        private static bool _lAltPressed = false, _rAltPressed = false;
        private static int KeyboardProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode < 0 || nCode != Native.HC_ACTION)
                return Native.CallNextHookEx(0, nCode, new IntPtr(wParam), lParam);

            Native.KeyboardLowLevelHookStruct kb = Marshal.PtrToStructure<Native.KeyboardLowLevelHookStruct>(lParam);
            switch ((WM)wParam)
            {
                case WM.KEYDOWN:
                case WM.SYSKEYDOWN:
                    if (kb.vkCode == VK.LMENU)
                    {
                        _lAltPressed = true;
                    }
                    else if (kb.vkCode == VK.RMENU)
                    {
                        _rAltPressed = true;
                    }

                    break;
                case WM.KEYUP:
                case WM.SYSKEYUP:
                    if (kb.vkCode == VK.LMENU)
                    {
                        _lAltPressed = false;
                    }
                    else if (kb.vkCode == VK.RMENU)
                    {
                        _rAltPressed = false;
                    }

                    break;
            }
            bool alt = _lAltPressed || _rAltPressed || Keyboard.GetKeyStates(Key.LeftAlt) == KeyStates.Down ||
                       (Keyboard.GetKeyStates(Key.RightAlt) == KeyStates.Down);

            bool validScope;
            switch (Properties.Settings.Default.Scope)
            {
                case 0:
                    // applications
                    validScope = false;
                    break;
                case 1:
                    // fullscreen windows
                    validScope = IsForegroundWindowFullScreen();
                    break;
                case 2:
                    // global
                    validScope = true;
                    break;
            }


            if (Properties.Settings.Default.DisableAltF4)
            {
                if (kb.vkCode == VK.F4 && alt)
                {
                    return 1;
                }
            }

            if (Properties.Settings.Default.DisableAltTab)
            {
                if (kb.vkCode == VK.Tab && alt)
                {
                    return 1;
                }
            }

            if (Properties.Settings.Default.DisableWinKey)
            {
                if (kb.vkCode == VK.LWIN || kb.vkCode == VK.RWIN)
                {
                    return 1;
                }
            }

            return Native.CallNextHookEx(0, nCode, new IntPtr(wParam), lParam);
        }

        private static bool IsForegroundWindowFullScreen()
        {
            var hwnd = Native.GetForegroundWindow();
            var windowRect = new Native.RectStruct();
            Native.GetWindowRect(hwnd, out windowRect);
            var monitor = Native.MonitorFromWindow(hwnd, Native.MONITOR_DEFAULTTONEAREST);
            var monitorInfo = new Native.MonitorInfoEx();
            monitorInfo.Init();
            Native.GetMonitorInfo(monitor, ref monitorInfo);
            return (windowRect.Bottom == monitorInfo.Monitor.Bottom &&
                    windowRect.Left == monitorInfo.Monitor.Left &&
                    windowRect.Right == monitorInfo.Monitor.Right &&
                    windowRect.Top == monitorInfo.Monitor.Top
                );
        }

        public static void RemoveHook()
        {
            Native.UnhookWindowsHookEx(_hook);
        }
    }
}
