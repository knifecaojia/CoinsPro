using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Top.Api;
using Top.Api.Domain;
using Top.Api.Request;
using Top.Api.Response;

namespace Common
{
    public class Sms
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="appkey"></param>
        /// <param name="secret"></param>
        /// <param name="SmsFreeSignName">短信签名，传入的短信签名必须是在阿里大于“管理中心-短信签名管理”中的可用签名。如“阿里大于”已在短信签名管理中通过审核，则可传入”阿里大于“（传参时去掉引号）作为短信签名。短信效果示例：【阿里大于】欢迎使用阿里大于服务。</param>
        /// <param name="SmsParam">短信模板变量，传参规则{"key":"value"}，key的名字须和申请模板中的变量名一致，多个变量之间以逗号隔开。示例：针对模板“验证码${code}，您正在进行${product}身份验证，打死不要告诉别人哦！”，传参时需传入{"code":"1234","product":"alidayu"}</param>
        /// <param name="RecNum">短信接收号码。支持单个或多个手机号码，传入号码为11位手机号码，不能加0或+86。群发短信需传入多个号码，以英文逗号分隔，一次调用最多传入200个号码。示例：18600000000,13911111111,13322222222</param>
        /// <param name="SmsTemplateCode">短信模板ID，传入的模板必须是在阿里大于“管理中心-短信模板管理”中的可用模板。示例：SMS_585014</param>
        /// <param name="Extend">公共回传参数，在“消息返回”中会透传回该参数；举例：用户可以传入自己下级的会员ID，在消息返回时，该会员ID会包含在内，用户可以根据该会员ID识别是哪位会员使用了你的应用</param>
        /// <returns></returns>
        public BizResult Send(string appkey, string secret, string SmsFreeSignName, string SmsParam, string RecNum, string SmsTemplateCode, string Extend = "", string url = "https://eco.taobao.com/router/rest") 
        {
            ITopClient client = new DefaultTopClient(url, appkey, secret,"json");
            AlibabaAliqinFcSmsNumSendRequest req = new AlibabaAliqinFcSmsNumSendRequest();
            req.Extend = Extend;
            req.SmsType = "normal";
            req.SmsFreeSignName = SmsFreeSignName;
            req.SmsParam = SmsParam;
            req.RecNum = RecNum;
            req.SmsTemplateCode = SmsTemplateCode;
            AlibabaAliqinFcSmsNumSendResponse rsp = client.Execute(req);
            return rsp.Result;
        }
    }
}
