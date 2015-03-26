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
        public string SensorName;
        public DateTime Time { get; set; } 
        public object Reading { get; set; }
        #endregion

    }


}
