using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class NoticeMessage
    {
        /// <summary>
        /// 信息发送给谁
        /// </summary>
        public RegUser messageTo { get; set; }
        public DateTime timeStamp { get; set; }
        public MessageType messageType { get; set; }

    }
    public enum MessageType
    {
        SMS,
        WECHAT,
        PHONE,
        SMS_WECHAT,
        SMS_PHONE,
        PHONE_WECHAT,
        ALL
    }
}
