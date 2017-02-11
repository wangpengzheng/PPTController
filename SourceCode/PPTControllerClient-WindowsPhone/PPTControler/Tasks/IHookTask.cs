using PPTController.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PPTController.Tasks
{
    public interface IHookTask
    {
        void HookStart();
        void HookStop();
        Action Action { get; set; }
    }
}
