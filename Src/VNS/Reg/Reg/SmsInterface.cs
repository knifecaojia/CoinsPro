using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reg
{
    public interface SmsInterface
    {
        string ServiceName { get; }
        string UserName { get; set; }
        string Password { get; set; }
        string Pid { get; set; }
        bool Login();
        string GetPhone();
        string GetMsg(string phone);
        /// <summary>
        /// 拉黑
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="pid"></param>
        bool AddIngore(string phone);
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="pid"></param>
        bool Release(string phone);
        bool ReleaseAll();
    }
}
