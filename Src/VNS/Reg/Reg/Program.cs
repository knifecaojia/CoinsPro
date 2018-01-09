using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using YZMDLL;
using System.Threading;
using System.Text.RegularExpressions;

namespace Reg
{
    class Program
    {
        private static Yzm yzm = new Yzm();
        private static string SmsToken = "";
        private static Proxy proxy = new Proxy("127.0.0.1", 1080);
        private static string invPhone = "";
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   // 总是接受  
            return true;
        }
        private const string CodeBreak = "https://way.jd.com/showapi/checkcode_ys?typeId=34&convert_to_jpg=0&appkey=3b2ad84f1ed6b6456e1e9c5ce1bfcc20";
        private const string VCodeImgUrl = "http://www.vnscoin.com/api/user/create_code_img/";
        private const string Sms = "【VNScore】尊敬的用户，您本次的验证码为901693，请于3分钟内正确输入，切勿泄露他人。";
        static void Main(string[] args)
        {
            //Console.WriteLine(getyzm(Sms, 6));
            //Login("18648611298", "vn,.?1wyky..", new Proxy("127.0.0.1", 1080));
            //登陆短信接码平台

            //string ivc = GetInvCodeList();
            //string ps= GetRandomString(12, true, true, true);
            //SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "insert into vns(username,password,invfrom,invcode,jifen,jiangli) values('" + 13514727682 + "','" + ps + "','18686111298','DDDDDDDD',1,2)");

            //SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "update vns set invcount=invcount+1 where invcode='" + ivc + "'");

            //Console.ReadKey();



            Console.WriteLine("Vns Autoreg bot!");






            smslogin("knifeandcj", "twgdhbtzhy2010...");
            while (true)
            {
                Reg();
                Console.ReadKey();
            }
            //Console.ReadKey();
        }
        static void GetProxy()
        {

        }
        static void Reg()
        {
            

            Thread.Sleep(1000);
            string phone = getMobileNum("37555");
            if (phone.Length != 11)
                return;
            Console.WriteLine("获得手机号:" + phone);
            //破解验证码


            bool piccodeflag = false;
            CookieContainer cc = new CookieContainer();
            while (!piccodeflag)
            {
                Bitmap capimg = LoadImg(VCodeImgUrl, out cc, proxy);
                //capimg.Save("temp.png");
                string piccode = CrackImg(capimg);
                string path = Directory.GetCurrentDirectory() + @"\imgs\" + piccode + ".png";
                capimg.Save(path);
                Console.WriteLine("图片验证码:" + piccode);
                //判断验证码是否识别正确，根据返回值判断
                piccodeflag = true;
            }
            

            string msg=getMessage(phone, "37555");
            if (msg.Length > 10)
            {
                Console.WriteLine("Raw SmsMsg:" + msg);
                string msgcode = getyzm(msg, 6);
                Console.WriteLine("Sms Verficode:" + msgcode);
                //获取邀请码
                string invcode = GetInvCodeList();
                Console.WriteLine("返回随机邀请码：" + invcode);

                string password = GetRandomString(12, true, true, true);

                HttpWebRequest request = null;
                string url = "http://www.vnscoin.com/user/register/?code=" + invcode;   //登录页面
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                request.Accept = "*/*;";
                request.UserAgent = "Mozilla/5.0";
                request.ContentType = "application/x-www-form-urlencoded";
                request.AllowAutoRedirect = true;
                request.CookieContainer = cc;
                request.KeepAlive = true;
                /*
 * code:TOfUKS7u
mobile:18648611298
phone_code:901693
password:vn,.?1wyky..
pwd_again:vn,.?1wyky..
*/
                string postData = string.Format("code={0}&mobile={1}&phone_code={2}&password={3}&pwd_again={4}",
                    System.Web.HttpUtility.UrlEncode(invcode),
                    System.Web.HttpUtility.UrlEncode(phone),
                    System.Web.HttpUtility.UrlEncode(msgcode),
                    System.Web.HttpUtility.UrlEncode(password),
                    System.Web.HttpUtility.UrlEncode(password));  //这里按照前面FireBug中查到的POST字符串做相应修改。
                byte[] postdatabyte = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = postdatabyte.Length;

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postdatabyte, 0, postdatabyte.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                string strWebData = string.Empty;
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    strWebData = reader.ReadToEnd();
                }
                if (response.Headers.Get("Location") == "/user/my/")
                {
                    //登陆
                    //
                    //尝试获取邀请码
                    request = null;
                    url = "http://www.vnscoin.com/user/my";   //登录页面
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";

                    request.Accept = "*/*;";
                    request.UserAgent = "Mozilla/5.0";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.AllowAutoRedirect = true;
                    request.CookieContainer = cc;
                    request.KeepAlive = true;



                    response = (HttpWebResponse)request.GetResponse();

                    strWebData = string.Empty;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        strWebData = reader.ReadToEnd();
                    }

                    var doc = new HtmlDocument();
                    doc.LoadHtml(strWebData);
                    var yaoqing = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div[3]/div[3]/div[2]/table/tbody/tr[2]/td[2]/div").First().InnerText;
                    var jifen = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div[3]/div[3]/div[2]/table/tbody/tr[4]/td[2]").First().InnerText;
                    var jiangli = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div[3]/div[3]/div[2]/table/tbody/tr[5]/td[2]").First().InnerText;

                    Console.WriteLine("新用户:{0}注册成功！！！,邀请:{1},积分:{2},奖励:{3}", phone, yaoqing, jifen, jiangli);

                    //更新数据库
                    SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "insert into vns(username,password,invfrom,invcode,jifen,jiangli) values('" + phone + "','" + password + "','" + invPhone + "','" + yaoqing + "'," + jifen + "," + jiangli + ")");

                    SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "update vns set invcount=invcount+1 where invcode='" + invcode + "'");

                }


            }
            else
            {
                Console.WriteLine("验证码短信读取错误");
            }
            string result = yzm.cancelAllRecv(SmsToken);
            Console.WriteLine("释放手机号:" + result);
        }
        /**  
* 从短信字符窜提取验证码  
* @param body 短信内容   
  * @param YZMLENGTH  验证码的长度 一般6位或者4位  
* @return 接取出来的验证码  
*/
        public static String getyzm(String body, int YZMLENGTH)
        {
            // 首先([a-zA-Z0-9]{YZMLENGTH})是得到一个连续的六位数字字母组合    
            // (?<![a-zA-Z0-9])负向断言([0-9]{YZMLENGTH})前面不能有数字    
            // (?![a-zA-Z0-9])断言([0-9]{YZMLENGTH})后面不能有数字出现    


            //  获得数字字母组合    
            //    Pattern p = Pattern   .compile("(?<![a-zA-Z0-9])([a-zA-Z0-9]{" + YZMLENGTH + "})(?![a-zA-Z0-9])");    

            //  获得纯数字  
            Regex p =new Regex("(?<![0-9])([0-9]{" + YZMLENGTH + "})(?![0-9])");

            Match m = p.Match(body);
            if (m.Success)
            {
               
                return m.Groups[0].ToString();
            }
            return null;
        }
        static private void smslogin(string username,string password)
        {
           
            string result = yzm.login(username, password);
           
            if (result.StartsWith("1|"))
            {
                Console.WriteLine("Sms code receive sys logined!");
                string[] array = result.Split('|');
                if (array.Length == 2)
                {
                    SmsToken = array[1];
                   
                }
                
            }
            else
            {
                Console.WriteLine("Sms code receive sys fucked!");
            }
        }
        private static string getMobileNum(string sid)
        {
           
            string result = yzm.getPhone(SmsToken, sid, "", "", "", "", "","");
            if (result.StartsWith("1|"))
            {
                return result.Split('|')[1];
             
            }
            else
            {
                return "";
            }
           
        }
        private static bool _exitGetYzm = false;
        private static string getMessage(string phone,string sid)
        {

            string rawmsg = "";
            try
            {
             


                int times = 0;
                while (true)
                {
                    if (times > 10)
                    {
                     
                        break;
                    }
                    if (_exitGetYzm)
                    {
                        
                        break;
                    }

                    string result = yzm.getMessage(SmsToken, sid, phone, "knifeandcj");
                    if (result.StartsWith("1|"))
                    {
                        rawmsg= result.Split('|')[1];
                        break;
                    }
                    else
                    {
                        times++;
                        Console.WriteLine(string.Format("正在第 {0} 次获取验证码。", times));
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
            return rawmsg;


        }
        static string GetInvCodeList()
        {
            //从数据库选择邀请数量小于5的用户
            DataTable dt = SqlHelper.ExecuteDataset(SqlHelper.GetConnSting(), CommandType.Text, "select * from vns where invcount<5 and id<100").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                Random r = new Random();
                int index = r.Next(dt.Rows.Count);
                invPhone= dt.Rows[index]["username"].ToString();
                return dt.Rows[index]["invcode"].ToString();
            }
            return "";
        }
        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe=false, string custom="")
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
        static string Bmp2Base64UrlEncode(Bitmap bmp)
        {
            
            string str = "";
            using (MemoryStream ms1 = new MemoryStream())
            {
                bmp.Save(ms1, System.Drawing.Imaging.ImageFormat.Png);

                byte[] arr1 = new byte[ms1.Length];
                ms1.Position = 0;
                ms1.Read(arr1, 0, (int)ms1.Length);
                ms1.Close();
                str = Convert.ToBase64String(arr1);
            }
            //return System.Web.HttpUtility.UrlEncode(str);
            return str;
        }
        static void Login(string username, string password, Proxy _proxy)
        {
            //
            RestClient rc = new RestClient("http://www.vnscoin.com/user/login/");
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            RestRequest rr = new RestRequest();
            var response =  rc.Execute(rr);


            string rawresponse = response.Content;
            var doc = new HtmlDocument();
            doc.LoadHtml(rawresponse);
            var value = doc.DocumentNode.SelectNodes("/html/body/div/form/input[1]").First().Attributes["value"].Value;
            CookieContainer cc = new CookieContainer();
            Bitmap capimg=LoadImg(VCodeImgUrl,out cc, _proxy);
            //capimg.Save("temp.png");
            string code = CrackImg(capimg);
            string path = Directory.GetCurrentDirectory() + @"\imgs\" + code + ".png";
            capimg.Save(path);
            Console.WriteLine("验证码:"+code);

            //准备登陆
           
            HttpWebRequest request = null;
            string url = "http://www.vnscoin.com/user/login/";   //登录页面
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            request.Accept = "*/*;";
            request.UserAgent = "Mozilla/5.0";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = true;
            request.CookieContainer = cc;
            request.KeepAlive = true;

            string postData = string.Format("csrfmiddlewaretoken={0}&mobile={1}&password={2}&check_code={3}", System.Web.HttpUtility.UrlEncode(value), System.Web.HttpUtility.UrlEncode(username), System.Web.HttpUtility.UrlEncode(password), System.Web.HttpUtility.UrlEncode(code));  //这里按照前面FireBug中查到的POST字符串做相应修改。
            byte[] postdatabyte = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = postdatabyte.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(postdatabyte, 0, postdatabyte.Length);
            }

            HttpWebResponse loginresponse = (HttpWebResponse)request.GetResponse();

            string strWebData = string.Empty;
            using (StreamReader reader = new StreamReader(loginresponse.GetResponseStream()))
            {
                strWebData = reader.ReadToEnd();
            }
            Console.WriteLine(strWebData);
            doc = new HtmlDocument();
            doc.LoadHtml(strWebData);
            var yaoqing = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div[3]/div[3]/div[2]/table/tbody/tr[2]/td[2]/div").First().InnerText;
            var jifen = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div[3]/div[3]/div[2]/table/tbody/tr[4]/td[2]").First().InnerText;
            var jiangli = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div[3]/div[3]/div[2]/table/tbody/tr[5]/td[2]").First().InnerText;
            Console.WriteLine("用户:{0},邀请:{1},积分:{2},奖励:{3}", username, yaoqing, jifen, jiangli);
        }
        public static string CrackImg(Bitmap bmp)
        {
            string text = "";
            RestRequest rr = new RestRequest(Method.POST);
            rr.AddParameter("body", "img_base64=" + Bmp2Base64UrlEncode(bmp));
            RestClient rc = new RestClient(CodeBreak);
            var response = rc.Execute(rr);


            string rawresponse = response.Content;
            JObject obj = JObject.Parse(rawresponse);
            if (obj["code"].ToString() == "10000")
            {
                text = obj["result"]["showapi_res_body"]["Result"].ToString();
            }
            return text;
        }
        /// <summary>
        /// 获取验证码方法
        /// </summary>
        public static Bitmap LoadImg(string ImageUrl, out CookieContainer Cookies, Proxy _proxy)
        {
            try
            {
                Cookies = new CookieContainer();
                HttpWebRequest wrq = null;
             
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                wrq = WebRequest.Create(ImageUrl) as HttpWebRequest;
                wrq.ProtocolVersion = HttpVersion.Version10;
                wrq = (HttpWebRequest)WebRequest.Create(ImageUrl);//请求的URL
                wrq.Method = "GET";
                wrq.Timeout = 5000;
                wrq.CookieContainer = new CookieContainer();
                wrq.ContentType = "application/x-www-form-urlencoded";
                if (_proxy != null)
                {
                    wrq.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
                }
                //获取返回资源
                HttpWebResponse response = (HttpWebResponse)wrq.GetResponse();
                //foreach (Cookie c in response.Cookies)
                //{
                //    Cookies.Add(c);
                //}
                //获取流
                Cookies = wrq.CookieContainer;//获取cookies
                Bitmap bt = Bitmap.FromStream(response.GetResponseStream()) as Bitmap;
               
                return bt;
            }
            catch(Exception e)
            {
                Cookies = null;
                return null;
            }
          
        }
    }
    public class RegResault
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class Proxy
    {
        public Proxy(string ip, int port)
        {
            this.IP = ip;
            this.Port = port;
        }
        public string IP;
        public int Port;
    }
}
