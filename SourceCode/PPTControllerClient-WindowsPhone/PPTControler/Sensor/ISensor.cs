using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPTController.Sensor
{
    public interface ISensor
    {
        void InitializeSensor();
        event Action ImplementNextPage;
        event Action ImplementPreviousPage;
    }
}
