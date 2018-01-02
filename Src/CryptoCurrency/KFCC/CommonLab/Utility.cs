using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public static class Utility
    {
        public static double ToFixed(double d, int s)
        {
            double sp = Math.Pow(10, s);
            return Math.Truncate(d) + Math.Floor((d - Math.Truncate(d)) * sp) / sp;
        }
    public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
        public static string GetHttpContent(string url,string method="GET",string postdata="", Proxy p = null)
        {
            if (method == "GET")
            {
                HttpResult hr = new HttpResult();
                HttpHelper hh = new HttpHelper();
                HttpItem hi = new HttpItem();
                hi.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36";
                string _url = url;
                hi.URL = _url;
                if (p != null)
                {
                    hi.ProxyIp = p.IP + ":" + p.Port;
                }
                hr = hh.GetHtml(hi);

                return hr.Html;
            }
            else
            {
                HttpResult hr = new HttpResult();
                HttpHelper hh = new HttpHelper();
                HttpItem hi = new HttpItem();
                hi.Method = "POST";
                hi.Postdata = postdata;
                string _url = url;
                hi.URL = _url;
                if (p != null)
                {
                    hi.ProxyIp = p.IP + ":" + p.Port;
                }
                hr = hh.GetHtml(hi);

                return hr.Html;
            }
        }
    }
    public static class TokenGen
    {
        
        static public string CreateToken(string message, string secret,Encoding encoding)
        {
            secret = secret ?? "";
            //var encoding = Encoding.GetEncoding("UTF-8");
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return ToHexString(hashmessage).ToUpper();
                //string hex = BitConverter.ToString(hashmessage).Replace("-", string.Empty);
               // return hex;
            }
        }
        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
        public static string CreateSign_Huobi(string domain,string method, string action, string secretKey, Dictionary<string, object> data)
        {
            var hashSource = $"{method}\n{domain.ToLower()}\n{action}\n";
            if (data != null)
            {
                hashSource += ConvertQueryString_Huobi(data, true);
            }
            var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(hashSource)).ToArray();
            return Convert.ToBase64String(hash);
        }
        public static string CreateSign_ZB(string secretKey, Dictionary<string, object> data)
        {
            var hashSource ="";
            secretKey = digest(secretKey);
            if (data != null)
            {
                hashSource += ConvertQueryString_Huobi(data, true);
            }
            byte[] k_ipad = new byte[64];
            byte[] k_opad = new byte[64];
            byte[] keyb;
            byte[] value;
            Encoding coding = Encoding.GetEncoding("UTF-8");
            try
            {
                keyb = coding.GetBytes(secretKey);
                // aKey.getBytes(encodingCharset);
                value = coding.GetBytes(hashSource);
                // aValue.getBytes(encodingCharset);
            }
            catch (Exception e)
            {
                keyb = null;
                value = null;
                //throw;
            }
            for (int i = keyb.Length; i < 64; i++)
            {
                k_ipad[i] = (byte)54;
                k_opad[i] = (byte)92;
            }
            for (int i = 0; i < keyb.Length; i++)
            {
                k_ipad[i] = (byte)(keyb[i] ^ 0x36);
                k_opad[i] = (byte)(keyb[i] ^ 0x5c);
            }
            byte[] sMd5_1 = MakeMD5(k_ipad.Concat(value).ToArray());
            byte[] dg = MakeMD5(k_opad.Concat(sMd5_1).ToArray());
            return toHex(dg);
            
        }
        /**
    * SHA加密
    * @param aValue
    * @return
    */
        public static String digest(String aValue)
        {
            aValue = aValue.Trim();
            byte[] value;
            SHA1 sha = null;
            Encoding coding = Encoding.GetEncoding("UTF-8");
            try
            {
                value = coding.GetBytes(aValue);
                // aValue.getBytes(encodingCharset);
                HashAlgorithm ha = (HashAlgorithm)CryptoConfig.CreateFromName("SHA");
                value = ha.ComputeHash(value);
            }
            catch (Exception e)
            {
                //value = coding.GetBytes(aValue);
                throw;
            }
            return toHex(value);
        }
        public static String toHex(byte[] input)
        {
            if (input == null)
                return null;
            StringBuilder output = new StringBuilder(input.Length * 2);
            for (int i = 0; i < input.Length; i++)
            {
                int current = input[i] & 0xff;
                if (current < 16)
                    output.Append('0');
                output.Append(current.ToString("x"));
            }
            return output.ToString();
        }
        /// <summary>
        /// 生成MD5摘要
        /// </summary>
        /// <param name="original">数据源</param>
        /// <returns>摘要</returns>
        public static byte[] MakeMD5(byte[] original)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyhash = hashmd5.ComputeHash(original);
            hashmd5 = null;
            return keyhash;
        }
        public static string CreateSign_Binance(Dictionary<string, string> data, string secretKey)
        {
            string str = ConvertQueryString_Binance(data,true);
            var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(str));
            return BitConverter.ToString(hash).Replace("-", "");

        }
        public static string ConvertQueryString_Huobi(Dictionary<string, object> data, bool urlencode = false)
        {
            var stringbuilder = new StringBuilder();
            foreach (var item in data)
            {
                stringbuilder.AppendFormat("{0}={1}&", item.Key, urlencode ? Uri.EscapeDataString(item.Value.ToString()) : item.Value.ToString());
            }
            stringbuilder.Remove(stringbuilder.Length - 1, 1);
            return stringbuilder.ToString();
        }
        public static string ConvertQueryString_Binance(Dictionary<string, string> data, bool urlencode = false)
        {
            var stringbuilder = new StringBuilder();
            foreach (var item in data)
            {
                stringbuilder.AppendFormat("{0}={1}&", item.Key, urlencode ? Uri.EscapeDataString(item.Value.ToString()) : item.Value.ToString());
            }
            stringbuilder.Remove(stringbuilder.Length - 1, 1);
            return stringbuilder.ToString();
        }
    }
}
