using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Storage
{
    class DeviceReadingStorage
    {
        public static bool AddDeviceReading(DateTime reading_time, Dictionary<string, object> readings)
        {
            AzureStorage storage = new AzureStorage();
            return true;
        }
    }
}
