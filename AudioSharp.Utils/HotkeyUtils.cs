using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using GlobalHotKey;

namespace AudioSharp.Utils
{
    public static class HotkeyUtils
    {
        private static readonly HotKeyManager _hotKeyManager = new HotKeyManager();
        private static Dictionary<HotKey, HotkeyType> _hotkeys = new Dictionary<HotKey, HotkeyType>();

        public static readonly List<Key> IllegalHotkeys = new List<Key>
        {
            Key.Enter,
            Key.NumLock,
            Key.Capital,
            Key.Scroll,
            Key.LeftAlt,
            Key.LeftCtrl,
            Key.LeftShift,
            Key.RightAlt,
            Key.RightCtrl,
            Key.RightShift
        };

        public delegate void GlobalHotkeyPressedDelegate(HotkeyType hotkey);
        public static event GlobalHotkeyPressedDelegate GlobalHoykeyPressed;

        public static bool RegisterAllHotkeys(IntPtr handle, Dictionary<HotkeyType, Tuple<Key, ModifierKeys>> globalHotkeys)
        {
            bool isSuccess = true;
            _hotKeyManager.KeyPressed += HotKeyManager_KeyPressed;

            foreach (KeyValuePair<HotkeyType, Tuple<Key, ModifierKeys>> globalHotkey in globalHotkeys)
            {
                try
                {
                    HotKey hotkey = _hotKeyManager.Register(globalHotkey.Value.Item1, globalHotkey.Value.Item2);
                    _hotkeys.Add(hotkey, globalHotkey.Key);
                }
                catch (Win32Exception)
                {
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        public static void UnregisterAllHotkeys(IntPtr handle)
        {
            _hotKeyManager.KeyPressed -= HotKeyManager_KeyPressed;

            foreach (HotKey hotkey in _hotkeys.Keys)
                _hotKeyManager.Unregister(hotkey);

            _hotkeys.Clear();
        }

        private static void HotKeyManager_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            GlobalHoykeyPressed?.Invoke(_hotkeys[e.HotKey]);
        }
    }
}
