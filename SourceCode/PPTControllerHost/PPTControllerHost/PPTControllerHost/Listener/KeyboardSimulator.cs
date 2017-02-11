using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using PPTControllerHost.ViewModel;

namespace PPTControllerHost.Listener
{
    /// <summary>
    /// Standard Keyboard Shortcuts used by most applications
    /// Ref: https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent
    /// </summary>
    public enum KeyType
    {
        PageUp = 33,
        PageDown = 34,
        End = 35,
        Home = 36,
        Shift = 16,
        F5 = 116,
        Esc = 27
    }

    public enum Operation
    {
        PrePage = 33,
        NextPage = 34,
        EndPage = 35,
        FirstPage = 36,
        Esc = 27,
        FullScreen,
        Start,
    }

    /// <summary>
    /// Simulate keyboard key presses
    /// </summary>
    public static class KeyboardSimulator
    {

        #region Windows API Code

        const int KEYEVENTF_EXTENDEDKEY = 0x1;
        const int KEYEVENTF_KEYUP = 0x2;



        #endregion

        #region Methods

        public static void KeyDown(byte key)
        {
            NativeDlls.keybd_event(key, 0, 0, 0);
        }

        public static void KeyUp(byte key)
        {
            NativeDlls.keybd_event(key, 0, KEYEVENTF_KEYUP, 0);
        }

        public static void SimulateOperation(string cmd, ConnectionViewModel vm)
        {
            Operation curOperation;
            cmd = cmd.ToUpper();

            if (cmd.Contains("UP"))
            {
                curOperation = Operation.PrePage;
            }
            else if (cmd.Contains("DOWN"))
            {
                curOperation = Operation.NextPage;
            }
            else if (cmd.Contains("HOME"))
            {
                curOperation = Operation.FirstPage;
            }
            else if (cmd.Contains("END"))
            {
                curOperation = Operation.EndPage;
            }
            else if (cmd.Contains("FULL"))
            {
                curOperation = Operation.FullScreen;
            }
            else if (cmd.Contains("ESC"))
            {
                curOperation = Operation.Esc;
            }
            else if (cmd.Contains("START"))
            {
                curOperation = Operation.Start;
            }
            else
                return;

            if (curOperation == Operation.Start)
            {
                string filelocation = Properties.Settings.Default.PPTLocation;
                if (!String.IsNullOrEmpty(filelocation) && File.Exists(Properties.Settings.Default.PPTLocation))
                {
                    System.Diagnostics.Process.Start(filelocation);
                    vm.SendInfoMessage(String.Format("Successfully opened your PPT {0}.", Path.GetFileNameWithoutExtension(filelocation)), InfoType.Success);
                }
                else
                {
                    vm.SendInfoMessage("There is not exist a valid PPT location in your computer. Please specify one from your PPTControllerHost.", InfoType.Error);
                }
            }
            else
            {
                if (curOperation != Operation.FullScreen)
                {
                    KeyDown((byte)curOperation);
                    KeyUp((byte)curOperation);
                }
                else
                {
                    // full screen
                    KeyDown((byte)KeyType.Shift);
                    KeyDown((byte)KeyType.F5);

                    KeyUp((byte)KeyType.F5);
                    KeyUp((byte)KeyType.Shift);
                }
            }
        }
        #endregion

    }

}
