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
     
        static string proxyHost = "http://http-pro.abuyun.com";
        static string proxyPort = "9010";
        static Log log = new Log("log/log.txt");
        static Log NewUserLog= new Log("log/newuserlog.txt");
        private static Proxy localproxy = new Proxy("127.0.0.1", 1080);
        private static WebProxy webproxy = null;
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   // 总是接受  
            return true;
        }
        private const string CodeBreak = "https://way.jd.com/showapi/checkcode_ys?typeId=34&convert_to_jpg=0&appkey=3b2ad84f1ed6b6456e1e9c5ce1bfcc20";
        private const string VCodeImgUrl = "http://www.vnscoin.com/api/user/create_code_img/";
        //private const string Sms = "【VNScore】尊敬的用户，您本次的验证码为901693，请于3分钟内正确输入，切勿泄露他人。";
        //static SmsInterface Sms = null;
        static List<SmsInterface> Smss = new List<SmsInterface>();
        static bool Working = false;
        static void InitWebProxy()
        {
            string proxyUser = "H5OO4JWC15HLR46P";
            string proxyPass = "A730FAF6BE67E59B";

            var proxy = new WebProxy();
            proxy.Address = new Uri(string.Format("{0}:{1}", proxyHost, proxyPort));
            proxy.Credentials = new NetworkCredential(proxyUser, proxyPass);
            webproxy = proxy;
        }
        static void UpdateWebProxy()
        {
            //string targetUrl = "http://test.abuyun.com/proxy.php";
            string targetUrl = "http://proxy.abuyun.com/switch-ip";
            //string targetUrl = "http://proxy.abuyun.com/current-ip";
            var request = WebRequest.Create(targetUrl) as HttpWebRequest;
            request.AllowAutoRedirect = true;
            request.KeepAlive = true;
            request.Method = "GET";
            request.Proxy = webproxy;

            using (var response = request.GetResponse() as HttpWebResponse)
            using (var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string htmlStr = sr.ReadToEnd();
            }
        }
        static int ThreadCount = 0;
        static Thread threadsCheckThread = null;
        static RegDBQueue regqueue = null;
        static Thread threadUpdateDBThread = null;
        static void Main(string[] args)
        {
            //整理用户
            //Dictionary<string, RegInfo> newusers = new Dictionary<string, RegInfo>();
            // StreamReader sr = new StreamReader("newuserlog.txt");
            //string line= sr.ReadLine();
            //while (line!=null&&line.Length > 2)
            //{
            //    string phone = line.Substring(line.IndexOf(":") + 1, 11);
            //    if (!newusers.ContainsKey(phone))
            //    {
            //        RegInfo nu = new RegInfo(null);
            //        nu.Phone = phone;
            //        nu.UpdateDB1();
            //        newusers.Add(phone, nu);
            //    }
            //    line = sr.ReadLine();
            //}
            StreamReader sr = new StreamReader("ll.txt");
            string ll = sr.ReadToEnd();
            sr.Close();
            DataTable dt = SqlHelper.ExecuteDataset(SqlHelper.GetConnection(), CommandType.Text, "select * from vns where password=''").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string phone = dt.Rows[i]["username"].ToString().Trim();
                string password = ll.Substring(ll.IndexOf(phone + "&phone_code=") + 39, 12);
                SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "update vns set password='" + password + "' where username='" + phone + "'");
            }

            Console.ReadKey();











            //InitWebProxy();
            regqueue = new RegDBQueue();
            //测试 隧道故障
            //webproxy = new WebProxy(proxy.IP, proxy.Port);
            //log.Write("Vns Autoreg bot! Select SMS service 1=suma 2=yima");
            //Console.WriteLine("Vns Autoreg bot! Select SMS service 1=suma 2=yima");
            //string input = Console.ReadLine();
            //if (input == "2")
            //{
            SmsInterface Sms1 = new YiMaSmsService("13950");
            Console.WriteLine("尝试登陆易码SMS服务");
            bool islogined1 = Sms1.Login();
            if (!islogined1)
            {
                Console.WriteLine("易码SMS登陆失败...");


            }
            else
            {
                Smss.Add(Sms1);
            }
            //}
            //else if (input == "1")
            //{
            SmsInterface Sms2 = new SuMaSmsService("37555");
            Console.WriteLine("尝试登陆速码SMS服务");
            bool islogined2 = Sms2.Login();
            if (!islogined2)
            {
                Console.WriteLine("速码SMS登陆失败...");

            }
            else
            {
                Smss.Add(Sms2);
            }

            //SmsInterface Sms3 = new ShenhuaSmsService("185753");
            //Console.WriteLine("尝试登陆神话SMS服务");
            //bool islogined3 = Sms3.Login();
            //if (!islogined3)
            //{
            //    Console.WriteLine("神话SMS登陆失败...");

            //}
            //else
            //{
            //    Smss.Add(Sms3);
            //}
            if (Smss.Count == 0)
            {
                Console.WriteLine("SMS服务 全部登陆失败，退出...");
                return;
            }
            Console.WriteLine("SMS平台数{0}开始注册...",Smss.Count);
            //}

            //string p=Sms3.GetPhone();
            //Console.WriteLine(p);
            //while (true)
            //{
            //    Thread.Sleep(4000);
            //    string msg = Sms3.GetMsg(p);
            //    Console.WriteLine(msg);
            //    if (msg.Length > 10)
            //        break;

            //}
            //Console.ReadKey();




            Working = true;
            threadsCheckThread = new Thread(CheckThreads);
            threadsCheckThread.IsBackground = true;
            threadsCheckThread.Start();

            threadUpdateDBThread = new Thread(UpdateDB);
            threadUpdateDBThread.IsBackground = true;
            threadUpdateDBThread.Start();

            while (true)
            {
                // Reg();
                //Console.ReadKey();
                string inputstr = Console.ReadLine();
                if (inputstr == "q")
                {
                    Working = false;
                    Thread.Sleep(1000);
                    threadUpdateDBThread.Abort();
                    threadsCheckThread.Abort();

                }
               
            }
            //Console.ReadKey();
        }
        static void UpdateDB()
        {
            while (Working)
            {
                regqueue.UpdateDB();
                Thread.Sleep(1000);

            }
        }
        static void CheckThreads()
        {
            while (Working)
            {
                if (ThreadCount < 60)
                {
                    //启动新的注册线程
                    ThreadCount++;
                    Thread.Sleep(1000);
                    Thread regThread = new Thread(new ThreadStart(Reg));
                    regThread.IsBackground = true;
                    regThread.Start();

                }
                Thread.Sleep(500);

            }
        }
        static Proxy gloabproxy = null;
        static bool GetProxy(RegInfo user,string tid)
        {
            bool flag = false;
            if (gloabproxy != null && (DateTime.Now - gloabproxy.InitTime).Seconds < 2)
            {
                user.proxy = gloabproxy;
                flag = true;
                return flag;
            }
            try
            {
                String line = "http://webapi.http.zhimacangku.com/getip?num=1&type=2&pro=&city=0&yys=0&port=1&time=1&ts=0&ys=0&cs=0&lb=1&sb=0&pb=4&mr=1&regions=";
                try
                {
                    using (StreamReader sr = new StreamReader("Proxy.txt"))
                    {
                        // Read the stream to a string, and write the string to the console.
                        line = sr.ReadToEnd();
                        Console.WriteLine(line);
                    }
                }
                catch (Exception e)
                {

                }


                RestClient rc = new RestClient(line);
                RestRequest rr = new RestRequest(Method.GET);
                var response = rc.Execute(rr);
                string rawresponse = response.Content;
                if (rawresponse.Length > 0)
                {
                    JObject obj = JObject.Parse(rawresponse);
                    if (Convert.ToBoolean(obj["success"]))
                    {
                        JArray ips = JArray.Parse(obj["data"].ToString());
                        if (ips.Count > 0)
                        {
                            gloabproxy= new Proxy(ips[0]["ip"].ToString(), Convert.ToInt32(ips[0]["port"].ToString()));
                            user.proxy = gloabproxy;
                            flag = true;
                            Console.WriteLine(tid + "代理服务器{0}:{1}获取成功:" ,user.proxy.IP,user.proxy.Port);
                        }
                            
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(tid + "代理服务器获取失败:" + e.Message);
                log.WriteLine(tid + "代理服务器获取失败:" + e.Message);
            }
            return flag;
        }
        //Thread.CurrentThread.ManagedThreadId.ToString() 获取线程ID
        static void Reg()
        {
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            Console.WriteLine("线程:{0}启动...", tid);
            Thread.Sleep(1000);
            RegInfo newuser = new RegInfo(Smss);

            //获取代理服务器
            bool proxyflag = GetProxy(newuser, tid);
            int proxycount = 0;
            while (!proxyflag)
            {
                Thread.Sleep(300);
                proxyflag = GetProxy(newuser, tid);
                proxycount++;
                if (proxycount > 3)
                {
                    break;
                }
            }
            if (!proxyflag)
            {
                newuser.proxy = null;
            }

            newuser.Phone = newuser.Sms.GetPhone();
            if (newuser.Phone.Length != 11)
            {
                Console.WriteLine(tid + "手机号获取失败:"+newuser.Sms.ServiceName + newuser.Phone);
                log.WriteLine(tid + "手机号获取失败:" + newuser.Sms.ServiceName + newuser.Phone);
                ThreadCount--;
                return;
            }
            Console.WriteLine(tid + "手机号获取:" + newuser.Sms.ServiceName + newuser.Phone+" 成功");
            log.WriteLine(tid + "手机号获取:" + newuser.Sms.ServiceName + newuser.Phone + " 成功");
          
            
            //破解验证码
            int piccount = 1;
            while (!BreakImgCode(newuser, tid))
            {
                Console.WriteLine(tid + "尝试第{0}次破解图片验证码", (piccount + 1));
                piccount++;
                if (piccount > 5)
                    break;

            }
            if (newuser.PicVCode == "")
            {
                ThreadCount--;
                return;
            }
            try

            {
                SendSmsMsg(newuser.Phone, newuser);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ThreadCount--;
                return;
            }
            //获取短信内容
            int times = 0;
            string Smsresult = "";
            while (true)
            {
                if (times > 20)
                {

                    break;
                }
                if (!Working)
                {

                    break;
                }
                Smsresult = newuser.Sms.GetMsg(newuser.Phone);
                if (Smsresult.Length > 4)
                {

                    break;
                }
                else
                {
                    times++;
                    //Console.WriteLine(string.Format(tid  +newuser.Sms.ServiceName+"正在第 {0} 次获取验证码。", times));
                    Thread.Sleep(5000);
                }
            }
            if (Smsresult.Length < 4)
            {
                Console.WriteLine(tid + "获取短信内容失败" + newuser.Sms.ServiceName);
                log.WriteLine(tid + "获取短信内容失败" + newuser.Sms.ServiceName);
                newuser.Sms.AddIngore(newuser.Phone);
                newuser.Sms.Release(newuser.Phone);
                ThreadCount--;
                return;
            }
            //获取短信验证码 正则方式
            newuser.PhoneSmsCode = getyzm(Smsresult, 6);
            Console.WriteLine(tid + "Raw SmsMsg:" + Smsresult + newuser.Sms.ServiceName);
            Console.WriteLine(tid + "Sms Verficode:" + newuser.PhoneSmsCode + newuser.Sms.ServiceName);

            //获取邀请码
            newuser.InviteCodeFrom = GetInvCodeList();
            Console.WriteLine(tid + "返回随机邀请码：" + newuser.InviteCodeFrom);

            newuser.Password = GetRandomString(12, true, true, true);
            newuser.Password = newuser.Password.Substring(0, 11) + "0";
            Console.WriteLine(tid + "返回随机密码：" + newuser.Password);

            int regtime = 1;
            while (true)
            {
              
                Console.WriteLine(tid + "第{0}次尝试注册：", regtime);
                
                bool flag = RegNewUser(newuser, tid);
                if (flag)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(tid + "第{0}次尝试注册成功，更新数据库...", regtime);
                    Console.ForegroundColor = ConsoleColor.White;
                    log.WriteLine(tid + "第" + regtime + "次尝试注册成功，更新数据库...");
                    //更新数据库
                    regqueue.Put(newuser);
                    //newuser.UpdateDB();
                    break;
                }
               
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(tid + "第{0}次尝试注册失败，..", regtime);
                    Console.ForegroundColor = ConsoleColor.White;
                    log.WriteLine(tid + "第" + regtime + "次尝试注册失败...");
                    regtime++;
                }

                

                if (regtime >= 4)
                {
                    Console.WriteLine(tid + "超过注册次数限制，失败！");
                    log.WriteLine(tid + "超过注册次数限制，失败！");
                    break;
                }
                // 默认注册失败是由于 图片验证码破解失败造成破解验证码
                piccount = 1;
                while (!BreakImgCode(newuser, tid))
                {
                    Console.WriteLine(tid + "第{1}次尝试注册 尝试第{0}次破解图片验证码", (piccount + 1), regtime);
                    piccount++;
                    if (piccount > 5)
                        break;
                }
            }
            //该号码拉黑
            newuser.Sms.AddIngore(newuser.Phone);
            if (newuser.Sms.Release(newuser.Phone))
                Console.WriteLine("释放手机号:" + newuser.Phone + "成功！");
            else
            {
                Console.WriteLine("释放手机号:" + newuser.Phone + "失败！");
            }
            ThreadCount--;
            //UpdateWebProxy();
        }
        
        static bool RegNewUser(RegInfo user,string tid)
        {
            try
            {
                HttpWebRequest request = null;
                string url = "http://www.vnscoin.com/user/register/?code=" + user.InviteCodeFrom;   //登录页面
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                if (user.proxy != null)
                {
                    request.Proxy = new WebProxy(user.proxy.IP, user.proxy.Port);
                }
                else
                {
                    if (webproxy != null)
                    {
                        request.Proxy = webproxy;
                    }
                    else
                    {
                        request.Proxy = new WebProxy(localproxy.IP, localproxy.Port);
                    }
                }
                request.Accept = "*/*;";
                request.UserAgent = "Mozilla/5.0";
                request.ContentType = "application/x-www-form-urlencoded";
                request.AllowAutoRedirect = true;
                request.CookieContainer = user.CC;
                request.KeepAlive = true;
                /*
    * code:TOfUKS7u
    mobile:18648611298
    phone_code:901693
    password:vn,.?1wyky..
    pwd_again:vn,.?1wyky..
    */

               string  postData = string.Format("code={0}&mobile={1}&phone_code={2}&password={3}&pwd_again={4}&check_code={5}",
                   System.Web.HttpUtility.UrlEncode(user.InviteCodeFrom),
                   System.Web.HttpUtility.UrlEncode(user.Phone),
                   System.Web.HttpUtility.UrlEncode(user.PhoneSmsCode),
                   System.Web.HttpUtility.UrlEncode(user.Password),
                   System.Web.HttpUtility.UrlEncode(user.Password),
                    System.Web.HttpUtility.UrlEncode(user.PicVCode));
                log.WriteLine(postData);//这里按照前面FireBug中查到的POST字符串做相应修改。
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
                if (strWebData.IndexOf("奖") > 0)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(strWebData);
                    try
                    {
                        var yaoqing = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div[3]/div[3]/div[2]/table/tbody/tr[2]/td[2]/div").First().InnerText;
                        var jifen = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div[3]/div[3]/div[2]/table/tbody/tr[4]/td[2]").First().InnerText;
                        var jiangli = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div[3]/div[3]/div[2]/table/tbody/tr[5]/td[2]").First().InnerText;
                        user.InviteCode = yaoqing;
                        user.JiFen = Convert.ToInt32(jifen);
                        user.JiangLi = Convert.ToInt32(jiangli);
                    }
                    catch
                    { }

                    string info = string.Format(tid + "新用户:{0}注册成功！！！,邀请:{1},积分:{2},奖励:{3}", user.Phone, user.InviteCode, user.JiFen, user.JiangLi);
                    Console.WriteLine(info);
                    log.WriteLine(info);
                    NewUserLog.WriteLine(info);
                    return true;
                }
                else
                {
                    throw new Exception(strWebData);
                }
            }
            catch(Exception e)
            {
                string info = string.Format(tid + "新用户:{0}注册失败 Err{1}",user.Phone,e.Message);
                string info1 = string.Format(tid + "新用户:{0}注册失败 ", user.Phone);

                Console.WriteLine(info1);
                log.WriteLine(info);
            }
            return false;
        }
        static bool BreakImgCode(RegInfo user,string tid)
        {
            try
            {
                string piccode = "";
                bool piccodeflag = false;
                CookieContainer cc = new CookieContainer();
                while (!piccodeflag)
                {
                    Bitmap capimg = LoadImg(VCodeImgUrl, out cc, user.proxy);
                    int trycount= 0;
                    while (capimg == null)
                    {
                        trycount++;
                        Thread.Sleep(500);
                        capimg = LoadImg(VCodeImgUrl, out cc, user.proxy);
                        if (trycount > 6)
                            break;
                    }
                    if (capimg == null)
                        return false;
                    //capimg.Save("temp.png");
                    piccode = CrackImg(capimg);
                    string path = Directory.GetCurrentDirectory() + @"\imgs\" + piccode + ".png";
                    capimg.Save(path);
                    Console.WriteLine(tid + "图片验证码:" + piccode);
                    log.WriteLine(tid + "图片验证码:" + piccode);
                    //判断验证码是否识别正确，根据返回值判断
                    piccodeflag = true;
                }
                user.PicVCode = piccode;
                user.CC = cc;
                return true;
            }
            catch(Exception e)
            {
             
                Console.WriteLine(tid + "图片验证码破解失败:" + e.Message);
                log.WriteLine(tid + "图片验证码破解失败:" + e.Message);
                return false;
            }
        }
        static string SendSmsMsg(string phone,RegInfo user)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.vnscoin.com/api/user/getsms/");
            //http://2017.ip138.com/ic.asp
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://2017.ip138.com/ic.asp");
            request.Method = "POST";
            if (user.proxy != null)
            {
                request.Proxy = new WebProxy(user.proxy.IP, user.proxy.Port);
            }
            else
            {
                if (webproxy != null)
                {
                    request.Proxy = webproxy;
                }
                else
                {
                    request.Proxy = new WebProxy(localproxy.IP, localproxy.Port);
                }
            }
            request.Accept = "*/*;";
            request.UserAgent = "Mozilla/5.0";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = true;
            request.KeepAlive = true;
            string postData = string.Format("mobile={0}&res=W&code={1}", System.Web.HttpUtility.UrlEncode(phone),System.Web.HttpUtility.UrlEncode(user.PicVCode));
            log.WriteLine(postData);//这里按照前面FireBug中查到的POST字符串做相应修改。
            byte[] postdatabyte = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = postdatabyte.Length;
            request.CookieContainer = user.CC;
            string strWebData = string.Empty;
            try

            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postdatabyte, 0, postdatabyte.Length);
                }



                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

               
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    strWebData = reader.ReadToEnd();
                    Console.WriteLine(strWebData);
                }
            }
            catch (Exception e)
            {
            }

            return strWebData;
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
       
       
     
      
        static string GetInvCodeList()
        {
            //从数据库选择邀请数量小于5的用户
            string[] invs = { "	1Wb4s3UzMKhSQo7Z", "h1Nt7rjU", "db217u8mSRxPnYek", "hB4X3ANm", "oCmdPNQbGMXBAy83" };
            //DataTable dt = SqlHelper.ExecuteDataset(SqlHelper.GetConnSting(), CommandType.Text, "select * from vns where id<30").Tables[0];
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    Random r = new Random();
            //    int index = r.Next(dt.Rows.Count);
            //    //invPhone= dt.Rows[index]["username"].ToString();
            //    return dt.Rows[index]["invcode"].ToString();
            //}
            long tick = DateTime.Now.Ticks;
            Random r = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            //Random r = new Random(DateTime.Now.);
            int index = r.Next(0, 4);
            return invs[index];
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
                    wrq.Proxy = new WebProxy(_proxy.IP, _proxy.Port);
                }
                else
                {
                    if (webproxy != null)
                    {
                        wrq.Proxy = webproxy;
                    }
                    else
                    {
                        wrq.Proxy = new WebProxy(localproxy.IP, localproxy.Port);
                    }
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
            InitTime = DateTime.Now;
        }
        public string IP;
        public int Port;
        public DateTime InitTime;
    }
    public class Log
    {
        private string logFile;
        private StreamWriter writer;
        private FileStream fileStream = null;
        private string _fileName;
        private static Dictionary<long, long> lockDic = new Dictionary<long, long>();
        /// <summary>  
        /// 获取或设置文件名称  
        /// </summary>  
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        /// <summary>  
        /// 构造函数  
        /// </summary>  
        /// <param name="byteCount">每次开辟位数大小，这个直接影响到记录文件的效率</param>  
        /// <param name="fileName">文件全路径名</param>  

        /// <summary>  
        /// 创建文件  
        /// </summary>  
        /// <param name="fileName"></param>  
        public void Create(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(fileName))
                {
                    fs.Close();
                }
            }
        }
        /// <summary>  
        /// 写入文本  
        /// </summary>  
        /// <param name="content">文本内容</param>  
        private void Write(string content, string newLine)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                throw new Exception("FileName不能为空！");
            }
            using (System.IO.FileStream fs = new System.IO.FileStream(_fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, 8, System.IO.FileOptions.Asynchronous))
            {
                //Byte[] dataArray = System.Text.Encoding.ASCII.GetBytes(System.DateTime.Now.ToString() + content + "/r/n");  
                Byte[] dataArray = System.Text.Encoding.Default.GetBytes(content + newLine);
                bool flag = true;
                long slen = dataArray.Length;
                long len = 0;
                while (flag)
                {
                    try
                    {
                        if (len >= fs.Length)
                        {
                            fs.Lock(len, slen);
                            lockDic[len] = slen;
                            flag = false;
                        }
                        else
                        {
                            len = fs.Length;
                        }
                    }
                    catch (Exception ex)
                    {
                        while (!lockDic.ContainsKey(len))
                        {
                            len += lockDic[len];
                        }
                    }
                }
                fs.Seek(len, System.IO.SeekOrigin.Begin);
                fs.Write(dataArray, 0, dataArray.Length);
                fs.Close();
            }
        }
        /// <summary>  
        /// 写入文件内容  
        /// </summary>  
        /// <param name="content"></param>  
        public void WriteLine(string content)
        {
            this.Write(content, System.Environment.NewLine);
        }
        /// <summary>  
        /// 写入文件  
        /// </summary>  
        /// <param name="content"></param>  
        public void Write(string content)
        {
            this.Write(content, "");
        }
        public Log(string fileName)
        {
            logFile = fileName;
            CreateDirectory(logFile);
            _fileName = fileName;
        }
        //使用
        //Log log = new Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
        //log.log(basePath);
        public void log(string info)
        {
            try
            {

                WriteLine(DateTime.Now + ": " + info);
                WriteLine("----------------------------------");
            }
            finally
            {

            }
        }
        public void RawLog(string info)
        {
            try
            {

                WriteLine(info);

            }
            finally
            {

            }
        }
        public void CreateDirectory(string infoPath)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(infoPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }
    }
}
