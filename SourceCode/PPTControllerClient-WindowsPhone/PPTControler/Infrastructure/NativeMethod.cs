using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PPTController.Infrastructure
{
    public class NativeMethod
    {
        public delegate int HookKeyProc(int code, IntPtr wParam, IntPtr lParam);
        
        [DllImport("coredll.dll")]
        public static extern int SetWindowsHookEx(int type, HookKeyProc HookKeyProc, IntPtr hInstance, int m);
        //private static extern int SetWindowsHookEx(int type, HookMouseProc HookMouseProc, IntPtr hInstance, int m);


        [DllImport("coredll.dll")]

        public static extern IntPtr GetModuleHandle(string mod);

        [DllImport("coredll.dll")]

        public static extern int CallNextHookEx(

                HookKeyProc hhk,

                int nCode,

                IntPtr wParam,

                IntPtr lParam

                );

        [DllImport("coredll.dll")]

        public static extern int GetCurrentThreadId();

        [DllImport("coredll.dll", SetLastError = true)]

        public static extern int UnhookWindowsHookEx(int idHook);

        public struct KBDLLHOOKSTRUCT
        {

            public int vkCode;

            public int scanCode;

            public int flags;

            public int time;

            public IntPtr dwExtraInfo;

        }

        public class KeyBoardInfo
        {

            public int vkCode;

            public int scanCode;

            public int flags;

            public int time;

        }
    }
}
