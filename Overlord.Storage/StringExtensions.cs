using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Storage
{
    public static class StringExtensions
    {
        public static Guid UrnToGuid(this string urn)
        {
            string prefix = "urn:uuid:";
            return Guid.ParseExact(urn.Substring(prefix.Length), "D");
        }

        public static Guid ToGuid(this string s)
        {            
            return Guid.ParseExact(s, "D");
        }
    }
}
