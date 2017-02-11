using Microsoft.Devices;
using System;

namespace PPTController.Tasks
{
    public class CameraButtonHookTask : IHookTask
    {
        public Action Action { get; set; }

        //安装钩子
        public void HookStart()
        {
            CameraButtons.ShutterKeyReleased += camera_ButtonReleased;

            CameraButtons.ShutterKeyHalfPressed += camera_ButtonHalfPress;

            //Event is fired when the button is fully pressed
            CameraButtons.ShutterKeyPressed += camera_ButtonFullPress;
        }

        //拆除钩子
        public void HookStop()
        {
            CameraButtons.ShutterKeyReleased -= camera_ButtonReleased;
        }

        private void camera_ButtonReleased(object sender, EventArgs e)
        {
            if (Action != null)
            {
                Action();
            }
        }

        private void camera_ButtonHalfPress(object sender, EventArgs e)
        {
        }

        private void camera_ButtonFullPress(object sender, EventArgs e)
        {
        }
    }
}
