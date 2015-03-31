using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Storage
{
    public static class GuidExtensions
    {
        public static string ToUrn(this Guid guid)
        {
            return guid.ToString("D");
        }

        public static string ToDeviceChannelTableName(this Guid guid)
        {
            return "DeviceX" + guid.ToString("D").Replace("-", "X");
        }

        public static string ToChannelTableName(this Guid guid)
        {
            return "ChannelX" + guid.ToString("D").Replace("-", "X");
        }

    }
}
