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
            if (urn.IndexOf(prefix) == 0)
            {
                return Guid.ParseExact(urn.Substring(prefix.Length), "D");
            }
            else
            {
                return Guid.ParseExact(urn, "D");
            }
        }

        public static string UrnToId(this string urn)
        {
            string prefix = "urn:uuid:";
            if (urn.IndexOf(prefix) == 0)
            {
                return urn.Substring(prefix.Length);
            }
            else return urn;
        }

        public static Guid ToGuid(this string s)
        {            
            return Guid.ParseExact(s, "D");
        }
    }
}
