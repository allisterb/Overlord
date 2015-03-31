using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string DeviceIdToTableName(this string s)
        {
            return s.Replace("-", "_");
        }

        public static string TableNameToId(this string s)
        {
            return s.Replace("_", "-");
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

        public static bool IsVaildSensorName(this string s)
        {
            return Regex.IsMatch(s, @"^[SsIiNnDdLlBb]{1}[0-9]+");

        }

        public static Type ToSensorType(this string s)
        {
            if (!s.IsVaildSensorName()) throw new ArgumentException("String is not a valid sensor name.");
            Match match = Regex.Match(s, @"^([SsIiNnDdLlBb]{1})([0-9]+)");
            string sensor_type = match.Groups[1].Value.ToUpperInvariant();
            if (sensor_type == "S")
            {
                return typeof(string);
            }
            else if (sensor_type == "I")
            {
                return typeof(int);
            }
            else if (sensor_type == "N")
            {
                return typeof(Double);
            }
            if (sensor_type == "D")
            {
                return typeof(DateTime);
            }

            else if (sensor_type == "L")
            {
                return typeof(bool);
            }
            else if (sensor_type == "B")
            {
                return typeof(byte[]);
            }

            else throw new ArgumentOutOfRangeException("Could not find the sensor type.");
            
        }

        public static string ToAzureResourceName(this string s)
        {
            //this.repositoryName = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName));
            //            this.repositoryName = new string(userName.Where(c => 
            //                (char.IsLetterOrDigit(c)
            //            )).ToArray()); 
            StringBuilder sb = new StringBuilder(63, 63);
            int i = 0;
            if (s != null)
            {
                foreach (char c in s)
                {
                    i++;
                    if (char.IsLetterOrDigit(c))
                        sb.Append(c);
                    else
                        sb.Append('-');

                    if (i == 63) break;
                }
                return sb.ToString();
            }
            else return null;
        }
    }
}
