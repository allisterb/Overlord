using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Threading.Tasks;

namespace Overlord.Storage
{
    public class IStorageSensorReading
    {
        #region Public properties
        public Guid DeviceId { get; set; }
        public string SensorName;
        public DateTime Time { get; set; }
        public string ETag { get; set; }
        public object Reading { get; set; }
        #endregion

    }


}
