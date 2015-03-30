using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Overlord.Storage
{
    
    public class IStorageDigestMessage
    {
        #region Public properties        
        public IStorageDevice Device { get; set; }
        public DateTime Time { get; set; }
        public string ETag { get; set; }
        public IDictionary<string, object> SensorValues { get; set; }        
        #endregion
    }




}
