using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public static class Utility
    {
        public static string GetHttpContent(string url,string method="GET",string postdata="", Proxy p = null)
        {
            if (method == "GET")
            {
                HttpResult hr = new HttpResult();
                HttpHelper hh = new HttpHelper();
                HttpItem hi = new HttpItem();

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
    }
}
