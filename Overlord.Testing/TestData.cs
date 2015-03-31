using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using Overlord.Security;
using Overlord.Security.Claims;
using Overlord.Security.ClaimTypes;
using Overlord.Storage;


namespace Overlord.Testing
{
    internal static class TestData
    {
        public static string user_01_id = "urn:uuid:d155074f-4e85-4cb5-a597-8bfecb0dfc04";
        public static string user_01_token = "xUnit_TestUser_01_Token";
        public static string user_01_name = "xUnit_TestUser_01_Name";

        public static string user_02_id = "urn:uuid:db4d2be1-a495-4ee0-9f49-03aa72a93f6b";
        public static string user_02_token = "xUnit_TestUser_02_Token";
        public static string user_02_name = "xUnit_TestUser_02_Name";

        public static string user_03_id = "urn:uuid:fba0c871-41d6-4ae1-83d6-f024a84adeb4";
        public static string user_03_token = "xUnit_TestUser_03_Token";
        public static string user_03_name = "xUnit_TestUser_03_Name";

        public static string device_01_id = "urn:uuid:9ac31883-f0e3-4666-a05f-6add31beb8f4";
        public static string device_01_token = "xUnit_TestDevice_01_Token";
        public static string device_01_name = "xUnit_TestDevice_01_Name";

        public static string device_02_id = "urn:uuid:618e7ae4-7ea8-4405-b229-e2fca57fd817";
        public static string device_02_token = "xUnit_TestDevice_02_Token";
        public static string device_02_name = "xUnit_TestDevice_02_Name";

        public static string device_03_id = "urn:uuid:84512c8b-b531-4067-8f42-e15656edb0a3";
        public static string device_03_token = "xUnit_TestDevice_03_Token";
        public static string device_03_name = "xUnit_TestDevice_03_Name";

        public static string device_04_id = "urn:uuid:9094b12e-e92c-44fc-bf64-54628e896e20";
        public static string device_04_token = "xUnit_TestDevice_04_Token";
        public static string device_04_name = "xUnit_TestDevice_04_Name";

        public static string device_05_id = "urn:uuid:3753de29-d028-4b0b-b4f4-2e4d8e31538b";
        public static string device_05_token = "xUnit_TestDevice_05_Token";
        public static string device_05_name = "xUnit_TestDevice_05_Name";

        public static string sensor_01_name = "S1";
        public static object sensor_01_reading_01 = "some string sensor data";
        public static object sensor_01_reading_02 = "some more string sensor data";
        public static object sensor_01_reading_03 = "even more string sensor data";
        public static string sensor_01_unit = "Unstructured Text";

        public static string sensor_02_name = "N1";
        public static object sensor_02_reading_01 = 15.7D;
        public static object sensor_02_reading_02 = 25.0D;
        public static string sensor_02_unit = "Degrees";

        public static string sensor_03_name = "D1";
        public static object sensor_03_reading_01 = DateTime.Now.AddHours(1);
        public static object sensor_03_reading_02 = DateTime.Now.AddHours(-1);
        public static string sensor_03_unit = "Degrees";

        public static string sensor_04_name = "L1";
        public static object sensor_04_reading_01 = 15.7D;
        public static string sensor_04_unit = "On/Off";

        public static Random rng = new Random();
        /// <summary>
        /// Genarate a random string of characters. Original code by Dan Rigby: 
        /// http://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c
        /// </summary>
        /// <param name="length">Length of string to return.</param>
        /// <returns></returns>
        public static string GenerateRandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];            
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[rng.Next(chars.Length)];
            }

            return new String(stringChars);
        }
        
        public static DateTime GenerateRandomTime(int? year, int? month, int? day, int? hour)
        {
            int y = year.HasValue ? year.Value : DateTime.Now.Year;
            int m = month.HasValue ? month.Value : DateTime.Now.Month;
            int d = day.HasValue ? day.Value : DateTime.Now.Day;
            int h = hour.HasValue ? hour.Value : rng.Next(24);            
            return new DateTime(y, m, d, h, rng.Next(60), rng.Next(60));
        }

        public static int GenerateRandomInteger(int min, int max)
        {
            return TestData.rng.Next(min, max);
        }
                
        public static IDictionary<string, object> GenerateRandomSensorData(int num_sensors)
        {
            string[] sensors = { "S", "I", "N", "L", "D", "B" };
            IDictionary<string, object> sensor_values = new Dictionary<string, object>();
            for (int i = 1; i <= num_sensors; i++)
            {
                string sensor_name = sensors[rng.Next(0, 5)] + rng.Next(num_sensors).ToString();
                if (sensor_values.Keys.Contains(sensor_name)) continue;
                if (sensor_name.ToSensorType() == typeof(string))
                    sensor_values.Add(new KeyValuePair<string, object>(sensor_name, GenerateRandomString(20)));
                else if (sensor_name.ToSensorType() == typeof(DateTime))
                    sensor_values.Add(new KeyValuePair<string, object>(sensor_name, GenerateRandomTime(null, null, 
                        null, null)));
                else if (sensor_name.ToSensorType() == typeof(int))
                    sensor_values.Add(new KeyValuePair<string, object>(sensor_name, rng.Next(5, 1000)));
                else if (sensor_name.ToSensorType() == typeof(double))
                    sensor_values.Add(new KeyValuePair<string, object>(sensor_name, rng.NextDouble()));
                else if (sensor_name.ToSensorType() == typeof(bool))
                    sensor_values.Add(new KeyValuePair<string, object>(sensor_name, rng.NextDouble() > 0.5));
                else
                {
                    byte[] b = new byte[100];
                    rng.NextBytes(b);
                    sensor_values.Add(new KeyValuePair<string, object>(sensor_name, b));
                }
            }
            return sensor_values;
        }

        public static IDictionary<string, object> GenerateRandomSensorData(IDictionary<string, object> 
            incoming_sensor_values)
        {
            string[] sensors = { "S", "I", "N", "L", "D", "B" };
            IDictionary<string, object> sensor_values = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> sv in incoming_sensor_values)
            {                
                if (sv.Key.ToSensorType() == typeof(string))
                    sensor_values.Add(sv.Key, GenerateRandomString(20));
                else if (sv.Key.ToSensorType() == typeof(DateTime))
                    sensor_values.Add(new KeyValuePair<string, object>(sv.Key, GenerateRandomTime(null, null,
                        null, null)));
                else if (sv.Key.ToSensorType() == typeof(int))
                    sensor_values.Add(new KeyValuePair<string, object>(sv.Key, rng.Next(5, 1000)));
                else if (sv.Key.ToSensorType() == typeof(double))
                    sensor_values.Add(new KeyValuePair<string, object>(sv.Key, rng.NextDouble()));
                else if (sv.Key.ToSensorType() == typeof(bool))
                    sensor_values.Add(new KeyValuePair<string, object>(sv.Key, rng.NextDouble() > 0.5));
                else
                {
                    byte[] b = new byte[100];
                    rng.NextBytes(b);
                    sensor_values.Add(new KeyValuePair<string, object>(sv.Key, b));
                }
            }
            return sensor_values;
        }    
    }
}
