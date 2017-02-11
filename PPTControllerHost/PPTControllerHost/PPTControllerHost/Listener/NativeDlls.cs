using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PPTControllerHost.Listener
{
    public class NativeDlls
    {
        public delegate bool CallBack(int hwnd, int lParam);

        [DllImport("user32")]
        public static extern int EnumWindows(CallBack x, int y);

        [DllImport("user32")]
        public static extern int GetWindowText(int hwnd, StringBuilder lptrstring, int nmaxcount);

        [DllImport("user32")]
        public static extern int GetParent(int hwnd);

        [DllImport("user32")]
        public static extern int SetForegroundWindow(int hwnd);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(int hwnd, int msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte key, byte scan, int flags, int extraInfo);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr GetForegroundWindow();
    }
}
