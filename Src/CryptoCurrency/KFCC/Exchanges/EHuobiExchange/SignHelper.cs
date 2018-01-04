using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KFCC.Exchanges.EHuobiExchange
{
    class SignHelper
    {
        private const string domain = "be.huobi.com";
        private const string SignatureMethod = "HmacSHA256";
        private const int SignatureVersion = 2;
        private string baseUrl = $"https://{domain}";
        private string GetDateTime() => DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        private string CreateSign(string method, string action, string secretKey, Dictionary<string, object> data)
        {
            var hashSource = $"{method}\n{domain.ToLower()}\n{action}\n";
            if (data != null)
            {
                hashSource += ConvertQueryString(data, true);
            }
            var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(hashSource)).ToArray();
            return Convert.ToBase64String(hash);
        }
        private string ConvertQueryString(Dictionary<string, object> data, bool urlencode = false)
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
