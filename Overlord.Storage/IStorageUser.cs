using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Storage
{
    public class IStorageUser
    {
        #region Public properties
        public Guid Id { get; set; }
        public string Version { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public  byte[] SerializedGeoIp { get; set; }
        public IList<IStorageDevice> Devices { get; set; }

        #endregion
    }
}
