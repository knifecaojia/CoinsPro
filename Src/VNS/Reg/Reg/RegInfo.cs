using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Reg
{
    public class RegInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string PhoneSmsCode { get; set; }
        /// <summary>
        /// 图片验证码
        /// </summary>
        public string PicVCode { get; set; }
        /// <summary>
        /// 上级邀请码
        /// </summary>
        public string InviteCodeFrom { get; set; }
        /// <summary>
        /// 本用户获得的新邀请码
        /// </summary>
        public string InviteCode { get; set; }
        public int JiFen { get; set; }
        public int JiangLi { get; set; }
        public CookieContainer CC { get; set; }
        public Proxy proxy { get; set; }
        public SmsInterface Sms{ get; set; }

        public RegInfo(List<SmsInterface> Smss)
        { 

            Random r = new Random();
            if (Smss != null)
            {
                int index = r.Next(0, Smss.Count);
                Sms = Smss[index];
            }
        }
        public void UpdateDB()
        {
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "insert into vns(username,password,invfrom,invcode,jifen,jiangli) values('" + Phone + "','" + Password + "','" + InviteCodeFrom + "','" + InviteCode + "'," + JiFen + "," + JiangLi + ")");

                SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "update vns set invcount=invcount+1 where invcode='" + InviteCodeFrom + "'");
            }

            catch (Exception e)
            {
                Console.WriteLine("更新数据库失败！" + e.Message);
            }
        }
        public void UpdateDB1()
        {
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "insert into vns(username,password,invfrom,invcode,jifen,jiangli) values('" + Phone + "','" + Password + "','" + InviteCodeFrom + "','" + InviteCode + "'," + JiFen + "," + JiangLi + ")");

                //SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "update vns set invcount=invcount+1 where invcode='" + InviteCodeFrom + "'");
            }

            catch (Exception e)
            {
                Console.WriteLine("更新数据库失败！" + e.Message);
            }
        }

    }
}
