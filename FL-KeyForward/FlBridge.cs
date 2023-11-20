using System;
using System.Runtime.InteropServices;

namespace FL_KeyForward
{
    public class FlBridge
    {

        const int INPUT_KEYBOARD = 1;

        const uint KEYEVENTF_SCANCODE = 0x0008;

        const uint KEYEVENTF_KEYUP = 0x0002;

        const int WM_KEYDOWN = 0x0100;

        const int WM_KEYUP = 0x0101;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;

            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        public static void SendKeyDown(IntPtr hWnd, char key) {
            SetForegroundWindow(hWnd);
            INPUT[] inputs = new INPUT[1];
            inputs[0] = new INPUT();
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].u.ki.wScan = (ushort)MapVirtualKey(key, 0);
            inputs[0].u.ki.dwFlags = KEYEVENTF_SCANCODE;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void SendKeyUp(IntPtr hWnd, char key) {
            SetForegroundWindow(hWnd);
            INPUT[] inputs = new INPUT[1];
            inputs[0] = new INPUT();
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].u.ki.wScan = (ushort)MapVirtualKey(key, 0);
            inputs[0].u.ki.dwFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
