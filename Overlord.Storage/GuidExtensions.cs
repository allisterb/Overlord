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
    }
}
