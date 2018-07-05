using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    /// <summary>
    /// 注册的用户基本信息
    /// </summary>
    public class RegUser
    {
        public string UID { get; set; }
        public string UserName { get; set; }
        public string MobilePhone { get; set; }
        /// <summary>
        /// 微信的openid
        /// </summary>
        public string OpenID { get; set; }
    }
}
