using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Storage
{
    public class IStorageAlert
    {
        public string SensorType { get; set; }
        public string StringValue { get; set; }
        public bool LogicalFlag { get; set; }
        public int IntMaxValue { get; set; }
        public int IntMinValue {get; set;}
        public double NumberMaxValue { get; set; }
        public double NumberMinValue { get; set; }
        public DateTime DateMaxValue { get; set; }
        public DateTime DateMinValue { get; set; }

    }
}
