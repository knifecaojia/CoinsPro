using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reg
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;

    /// <summary>
    /// 神话短信服务平台开放接口范例代码
    /// 语言版本：C#版
    /// 官方网址：www.51ym.me
    /// 技术支持：QQ2114927217
    /// 发布时间：217-12-11
    /// </summary>
    public class ShenhuaSmsService: SmsInterface
    {
        public static string API = "http://api.shjmpt.com:9002/pubApi/";//神话短信服务平台开放接口地址
        public static string Token = "";//通信令牌，可登录网站查看，也可通过调用登录接口获取
        public string ServiceName { get { return "神话"; } }
        public static string username = "knifeandcj";//帐号
        public static string password = "knifecaojia";//密码

        public int itemid = 185753;//项目编号，必须参数
        public int province = 0;//省编号，不指定请设置为0
        public int city = 0;//市编号，不指定请设置为0
        public int isp = 0;//运营商编号，不指定请设置为0
        public string excludeno = "";//要排除的号段，多个用“|”分割，不指定请留空
        public string AssignMobile = "";//要指定获取的电话号码，不指定请留空

        public string UserName { get { return username; } set { username = value; } }

        public string Password { get { return password; } set { password = value; } }

        public string Pid { get { return itemid.ToString(); } set { itemid = Convert.ToInt32(itemid); } }
        public ShenhuaSmsService(string pid)
        {
            Pid = pid;
        }
        /// <summary>
        /// 用户登录获取Token，如果已设置Token则可用不调用该功能
        /// </summary>
        /// <param name="msg">返回值</param>
        /// <returns></returns>
        public bool Login(out string msg)
        {
            msg = string.Empty;
            try
            {
                var result = DoGet(string.Format("uLogin?uName={0}&pWord={1}", username, password));
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (result.Html.Trim().ToLower().IndexOf("&") > 0)
                    {
                        Console.WriteLine("Sms code receive sys logined!");
                        Token = result.Html.Split('&')[0];
                        //Token = result.Html.Trim().Substring("success|".Length);
                        msg = Token;
                        return true;
                    }
                    msg = result.Html.Trim();
                    return false;
                }
                else
                {
                    msg = string.Format("HTTP请求失败，错误代码[{0}]", result.StatusCode.ToString());
                    Console.WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }
        public bool Login()
        {
            string raw;
            return Login( out raw);
        }


        public string GetMsg(string phone)
        {
            return GetMsg(phone, Pid);
        }

        public bool AddIngore(string phone)
        {
            string raw;
            bool flag = Mobile_addIgnore(phone,itemid,out raw);
            return flag;
        }

        public bool Release(string phone)
        {
            string raw;
            bool flag = Mobile_Release(phone, itemid, out raw);
            return flag;
        }
        public bool ReleaseAll()
        {
            string raw;
            bool flag = Mobile_ReleaseAll(out raw);
            return flag;
        }
        public string GetPhone()
        {
            string raw;
            bool flag = Mobile_Get(out raw);
            if (flag)
                return raw;
            else
                return "";
        }

        public string GetMsg(string phone,string pid)
        {
            string raw;
            bool flag = Sms_Get(phone, Convert.ToInt32(pid), out raw);
            if (flag)
                return raw;
            else
            {
                Console.WriteLine(raw);
                return "";
            }
        }

        /// <summary>
        /// 获取账户信息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool GetAccountInfo(out string msg)
        {
            msg = string.Empty;
            if (string.IsNullOrEmpty(Token))
            {
                if (!Login(out msg))
                    return false;
            }
            try
            {
                var result = DoGet("action=getaccountinfo");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (result.Html.Trim().ToLower().IndexOf("success|") == 0)
                    {
                        msg = result.Html.Trim().Substring("success|".Length);
                        return true;
                    }
                    else
                    {
                        msg = result.Html.Trim();
                    }
                }
                else
                {
                    msg = string.Format("HTTP请求失败，错误代码[{0}]", result.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 获取电话号码
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Mobile_Get(out string msg)
        {
            msg = string.Empty;
            if (string.IsNullOrEmpty(Token))
            {
                if (!Login(out msg))
                    return false;
            }
            string _url = string.Format("GetPhone?ItemId={0}", itemid);
            if (province > 0)
                _url += string.Format("&province={0}", province);
            if (city > 0)
                _url += string.Format("&city={0}", city);
            if (isp > 0)
                _url += string.Format("&isp={0}", isp);
            if (!string.IsNullOrEmpty(AssignMobile))
                _url += string.Format("&mobile={0}", AssignMobile);
            if (!string.IsNullOrEmpty(excludeno))
                _url += string.Format("&excludeno={0}", excludeno);

            try
            {
                var result = DoGet(_url);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (result.Html.Trim().ToLower().IndexOf(";") > 0)
                    {
                        msg = result.Html.Trim().Substring(0,11);
                        return true;
                    }
                    else
                    {
                        msg = result.Html.Trim();
                    }
                }
                else
                {
                    msg = string.Format("HTTP请求失败，错误代码[{0}]", result.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 拉黑号码
        /// </summary>
        /// <param name="mobileno">电话号码</param>
        /// <param name="itemid">项目编号</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Mobile_addIgnore(string mobileno, int itemid, out string msg)
        {
            msg = string.Empty;
            var result = DoGet(string.Format("AddBlack? & phoneList ={1}-{0};", mobileno, itemid));
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (result.Html.Trim().ToLower().Equals("success"))
                    return true;
                else
                    msg = result.Html.Trim();
            }
            else
            {
                msg = string.Format("HTTP请求失败，错误代码[{0}]", result.StatusCode.ToString());
            }
            return false;
        }
        /// <summary>
        /// 释放号码
        /// </summary>
        /// <param name="mobileno">电话号码</param>
        /// <param name="itemid">项目编号</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Mobile_Release(string mobileno, int itemid, out string msg)
        {
            msg = string.Empty;
            var result = DoGet(string.Format("ReleasePhone?phoneList={0}-{1};", mobileno, itemid));
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (result.Html.Trim().ToLower().Equals("success"))
                {
                    return true;
                }
                else
                {
                    msg = result.Html.Trim();
                }
            }
            else
            {
                msg = string.Format("HTTP请求失败，错误代码[{0}]", result.StatusCode.ToString());
            }
            return false;
        }

        /// <summary>
        /// 释放该用户名下的所有号码
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Mobile_ReleaseAll(out string msg)
        {
            msg = string.Empty;
            var result = DoGet("action=releaseall");
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (result.Html.Trim().ToLower().Equals("success"))
                {
                    return true;
                }
                else
                {
                    msg = result.Html.Trim();
                }
            }
            else
            {
                msg = string.Format("HTTP请求失败，错误代码[{0}]", result.StatusCode.ToString());
            }
            return false;
        }

        /// <summary>
        /// 获取短信
        /// </summary>
        /// <param name="mobileno">电话号码</param>
        /// <param name="itemid">项目编号</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Sms_Get(string mobileno, int itemid, out string msg)
        {
            msg = string.Empty;
            var result = DoGet(string.Format("GMessage?ItemId={0}&Phone={1}", itemid, mobileno));
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (result.Html.Trim().ToLower().IndexOf("msg") == 0)
                {
                    msg = result.Html.Trim().Split('&')[3];
                    return true;
                }
                else
                {
                    msg = result.Html.Trim();
                }
            }
            else
            {
                msg = string.Format("HTTP请求失败，错误代码[{0}]", result.StatusCode.ToString());
            }
            return false;
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobileno">电话号码</param>
        /// <param name="itemid">项目编号</param>
        /// <param name="sms">发送内容</param>
        /// <param name="msg">如果成功该值为发送订单号，用于查询发送状态</param>
        /// <returns></returns>
        public bool Sms_Send(string mobileno, int itemid, string sms, out string msg)
        {
            msg = string.Empty;
            var result = DoGet(string.Format("action=sendsms&itemid={0}&mobile={1}&sms={2}", itemid, mobileno, HttpUtility.UrlEncode(sms)));
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (result.Html.Trim().ToLower() == "success")
                {
                    return true;
                }
                else
                {
                    msg = result.Html.Trim();
                }
            }
            else
            {
                msg = string.Format("HTTP请求失败，错误代码[{0}]", result.StatusCode.ToString());
            }
            return false;
        }

        /// <summary>
        /// 获取短信发送状态
        /// </summary>
        /// <param name="mobileno">电话号码</param>
        /// <param name="itemid">项目编号</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendSms_GetState(string mobileno, int itemid, out string msg)
        {
            msg = string.Empty;
            var result = DoGet(string.Format("action=getsendsmsstate&itemid={0}&mobile={1}", itemid, mobileno));
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (result.Html.Trim().ToLower() == "success")
                {
                    return true;
                }
                else if (result.Html.Trim().ToLower().IndexOf("fail|") == 0)
                {
                    msg = result.Html.Trim().Substring("fail|".Length);
                    return true;
                }
                else
                {
                    msg = result.Html.Trim();
                }
            }
            else
            {
                msg = string.Format("HTTP请求失败，错误代码[{0}]", result.StatusCode.ToString());
            }
            return false;
        }

        private HttpResult DoGet(string param)
        {
            HttpResult result = new HttpResult();

            string _url = string.Format("{0}{1}&token={2}", API.Trim('?'), param.Trim('&'), Token);
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(_url);
            httpRequest.Timeout = 60000;
            httpRequest.Method = "GET";

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            result.StatusCode = httpResponse.StatusCode;
            result.StatusDescription = httpResponse.StatusDescription;
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                result.Html = sr.ReadToEnd();
                sr.Close();
            }
            httpResponse.Close();
            return result;
        }

       

        public class HttpResult
        {
            private string _html = string.Empty;
            /// <summary>
            /// 返回的String类型数据
            /// </summary>
            public string Html
            {
                get { return _html; }
                set { _html = value; }
            }
            /// <summary>
            /// 返回状态说明
            /// </summary>
            public string StatusDescription { get; set; }
            /// <summary>
            /// 返回状态码,默认为OK
            /// </summary>
            public HttpStatusCode StatusCode { get; set; }
        }
    }
   
}
