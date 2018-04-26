using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Reg
{
    class Config
    {
        private static string baseUrl = "http://api.eobzz.com/api/do.php?action=";
        public static string loginIn = baseUrl + "loginIn&";
        public static string getUserInfos = baseUrl + "getUserInfos&";
        public static string getPhone = baseUrl + "getPhone&";
        public static string addBlacklist = baseUrl + "addBlacklist&";
        public static string getMessage = baseUrl + "getMessage&";
        public static string cancelRecv = baseUrl + "cancelRecv&";
        public static string cancelAllRecv = baseUrl + "cancelAllRecv&";
    }
    public class HttpHelper
    {
        public static string GetHtml(string getUrl)
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {

                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(getUrl);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ServicePoint.ConnectionLimit = 300;
                httpWebRequest.Referer = getUrl;
                httpWebRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, */*";
                httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
                httpWebRequest.Method = "POST";
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch
            {
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                return string.Empty;
            }
        }
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
    }
    public class SuMaSmsService : SmsInterface
    {
        public static string Token = "";//通信令牌，可登录网站查看，也可通过调用登录接口获取

        public static string username = "knifeandcj";//帐号
        public static string password = "twgdhbtzhy2010...";//密码

        public int itemid = 37555;//项目编号，必须参数
        public string UserName { get { return username; } set { username = value; } }

        public string Password { get { return password; } set { password = value; } }

        public string Pid { get { return itemid.ToString(); } set { itemid = Convert.ToInt32(itemid); } }

        public string ServiceName { get { return "速码"; } }

        public SuMaSmsService(string pid)
        {
            Pid = pid;
        }
        public bool Login()
        {
            string result = login(username, password);
            if (result.StartsWith("1|"))
            {
              
                string[] array = result.Split('|');
                if (array.Length == 2)
                {
                    Token = array[1];

                }
                Console.WriteLine("Sms code receive sys logined!");
                return true;
            }
            else
            {
                Console.WriteLine("Sms code receive sys fucked!");
            }
            return false;
        }

        public string GetPhone()
        {
            string result = getPhone(Token, itemid.ToString(), "", "", "", "", "", "");
            if (result.StartsWith("1|"))
            {
                return result.Split('|')[1];

            }
            else
            {
                return "";
            }
           
        }

        public string GetMsg(string phone)
        {
            string result = getMessage(Token, itemid.ToString(), phone, username);
            string rawmsg = "";
            if (result.StartsWith("1|"))
            {
                rawmsg = result.Split('|')[1];
               
            }
             return rawmsg;
        }

        public bool AddIngore(string phone)
        {
            string result = addBlacklist(Token, itemid.ToString(), phone);
            if (result.StartsWith("1|"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Release(string phone)
        {
            string result = cancelRecv(Token, itemid.ToString(), phone);
            if (result.StartsWith("1|"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ReleaseAll()
        {
            string result = cancelAllRecv(Token);
            if (result.StartsWith("1|"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string login(string userName, string password)
        {
            try
            {
                string para = string.Format("name={0}&password={1}", userName.Trim(), password.Trim());
                string url = Config.loginIn + para;
                return HttpHelper.GetHtml(url);
            }
            catch (Exception ex)
            {
                return "yzmError" + ex.Message;
            }
        }
        public string getUserInfo(string userName, string token)
        {
            try
            {
                string para = string.Format("uid={0}&token={1}", userName, token);
                string url = Config.getUserInfos + para;
                return HttpHelper.GetHtml(url);
            }
            catch (Exception ex)
            {
                return "yzmError" + ex.Message;
            }
        }
        public string getPhone(string token, string sid, string phone, string phoneType, string vno, string locationMatching, string locationLevel, string location)
        {
            try
            {
                string para = string.Format("token={0}&sid={1}&phone={2}&phoneType={3}&vno={4}&locationMatching={5}&locationLevel={6}&location={7}", token, sid, phone, phoneType, vno, locationMatching, locationLevel, HttpHelper.UrlEncode(location));
                string url = Config.getPhone + para;
                return HttpHelper.GetHtml(url);
            }
            catch (Exception ex)
            {
                return "yzmError" + ex.Message;
            }
        }
        public string addBlacklist(string token, string sid, string phone)
        {
            try
            {
                string para = string.Format("token={0}&phone={1}&sid={2}", token, phone, sid);
                string url = Config.addBlacklist + para;
                return HttpHelper.GetHtml(url);
            }
            catch (Exception ex)
            {
                return "yzmError" + ex.Message;
            }
        }
        public string getMessage(string token, string sid, string phone, string author)
        {
            try
            {
                string para = string.Format("token={0}&sid={1}&phone={2}&author={3}", token, sid, phone, author);
                string url = Config.getMessage + para;
                return HttpHelper.GetHtml(url);
            }
            catch (Exception ex)
            {
                return "yzmError" + ex.Message;
            }
        }
        public string cancelRecv(string token, string sid, string phone)
        {
            try
            {
                string para = string.Format("token={0}&sid={1}&phone={2}", token, sid, phone);
                string url = Config.cancelRecv + para;
                return HttpHelper.GetHtml(url);
            }
            catch (Exception ex)
            {
                return "yzmError" + ex.Message;
            }
        }

        public string cancelAllRecv(string token)
        {
            try
            {
                string para = string.Format("token={0}", token);
                string url = Config.cancelAllRecv + para;
                return HttpHelper.GetHtml(url);
            }
            catch (Exception ex)
            {
                return "yzmError" + ex.Message;
            }
        }

       
    }
}
