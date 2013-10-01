using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using ServiceStack.Text;
using System.Configuration;

namespace mydigitalspace
{
    public class Util
    {
        public static string GetAppConfigSetting(string SettingName)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(SettingName))
            {
                return ConfigurationManager.AppSettings[SettingName];
            }
            else
            {
                return string.Empty;
            }
        }

        public static string SerializeObject<T>(T objectToSerialize)
        {
            string json = JsonSerializer.SerializeToString<T>(objectToSerialize);

            return json;
        }

        public static object DeSerializeObject<T>(string data)
        {
            T fromjson = (T)JsonSerializer.DeserializeFromString<T>(data);

            return fromjson;
        }

        public static string DictionaryToParameters(Dictionary<string, object> dict)
        {
            StringBuilder sb = new StringBuilder();
            string param_string = string.Empty;

            foreach (KeyValuePair<string, object> kv in dict)
            {
                sb.Append(string.Format("&{0}={1}", kv.Key, kv.Value.ToString()));
            }
            param_string = sb.ToString();

            if (sb.ToString().StartsWith("&"))
            {
                param_string = param_string.Remove(0, 1);
            }

            return param_string;
        }

        public static float GetEpochTime()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        public static string GetSubstringByString(string start, string end, string value)
        {
            return value.Substring((value.IndexOf(start) + start.Length), (value.IndexOf(end) - value.IndexOf(start) - start.Length));
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            //byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static Dictionary<string, object> ProcessLogin(string username, string password)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("method", "LOGON");
            data.Add("logon", username);
            data.Add("passwordhash", Util.CalculateMD5Hash(username + password));
            data.Add("casesensitive", "Y");

            string requestURI = "https://au.mydigitalstructure.com/rpc/logon/";
            string datastring = Util.DictionaryToParameters(data);

            return HTMLHelper.POST(requestURI, datastring);
        }
    }
}
