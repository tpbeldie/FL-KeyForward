using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FL_KeyForward
{
    public class GlobalKeyboardHook
    {

        private LowLevelKeyboardProc m_proc;

        private IntPtr m_hookID = IntPtr.Zero;

        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;

        private const int WM_KEYUP = 0x0101;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public event EventHandler<KeyUpEventArgs> KeyUp;

        public GlobalKeyboardHook() {
            m_proc = HookCallback;
        }

        ~GlobalKeyboardHook() {
            UnhookWindowsHookEx(m_hookID);
        }

        public void HookKeyboard() {
            m_hookID = SetWindowsHookEx(WH_KEYBOARD_LL, m_proc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
        }

        public void UnhookKeyboard() {
            UnhookWindowsHookEx(m_hookID);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) {
                int vkCode = Marshal.ReadInt32(lParam);
                if (KeyPressed != null) {
                    KeyPressed.Invoke(this, new KeyPressedEventArgs((Keys)vkCode));
                }
            }
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP) {
                int vkCode = Marshal.ReadInt32(lParam);
                KeyUp.Invoke(this, new KeyUpEventArgs((Keys)vkCode));
            }
            return CallNextHookEx(m_hookID, nCode, wParam, lParam);
        }

        public class KeyUpEventArgs : EventArgs
        {
            public Keys KeyUp { get; private set; }

            public KeyUpEventArgs(Keys key) {
                KeyUp = key;
            }
        }

        public class KeyPressedEventArgs : EventArgs
        {
            public Keys KeyPressed { get; private set; }

            public KeyPressedEventArgs(Keys key) {
                KeyPressed = key;
            }
        }
    }
}
