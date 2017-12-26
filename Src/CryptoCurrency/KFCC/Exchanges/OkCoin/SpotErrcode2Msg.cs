using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFCC.EOkCoin
{
    static public class SpotErrcode2Msg
    {
        static public string Prase(string code)
        {
            if (code == "10000") return "必选参数不能为空";
            if (code == "10001") return "用户请求频率过快，超过该接口允许的限额";
            if (code == "10002") return "系统错误";
            if (code == "10004") return "请求失败";
            if (code == "10005") return "SecretKey不存在";
            if (code == "10006") return "Api_key不存在";
            if (code == "10007") return "签名不匹配";
            if (code == "10008") return "非法参数";
            if (code == "10009") return "订单不存在";
            if (code == "10010") return "余额不足";
            if (code == "10011") return "买卖的数量小于BTC/LTC最小买卖额度";
            if (code == "10012") return "当前网站暂时只支持btc_usd ltc_usd";
            if (code == "10013") return "此接口只支持https请求";
            if (code == "10014") return "下单价格不得≤0或≥1000000";
            if (code == "10015") return "下单价格与最新成交价偏差过大";
            if (code == "10016") return "币数量不足";
            if (code == "10017") return "API鉴权失败";
            if (code == "10018") return "借入不能小于最低限额[usd:100,btc:0.1,ltc:1]";
            if (code == "10019") return "页面没有同意借贷协议";
            if (code == "10020") return "费率不能大于1%";
            if (code == "10021") return "费率不能小于0.01%";
            if (code == "10023") return "获取最新成交价错误";
            if (code == "10024") return "可借金额不足";
            if (code == "10025") return "额度已满，暂时无法借款";
            if (code == "10026") return "借款(含预约借款)及保证金部分不能提出";
            if (code == "10027") return "修改敏感提币验证信息，24小时内不允许提现";
            if (code == "10028") return "提币金额已超过今日提币限额";
            if (code == "10029") return "账户有借款，请撤消借款或者还清借款后提币";
            if (code == "10031") return "存在BTC/LTC充值，该部分等值金额需6个网络确认后方能提出";
            if (code == "10032") return "未绑定手机或谷歌验证";
            if (code == "10033") return "服务费大于最大网络手续费";
            if (code == "10034") return "服务费小于最低网络手续费";
            if (code == "10035") return "可用BTC/LTC不足";
            if (code == "10036") return "提币数量小于最小提币数量";
            if (code == "10037") return "交易密码未设置";
            if (code == "10040") return "取消提币失败";
            if (code == "10041") return "提币地址不存在或未认证";
            if (code == "10042") return "交易密码错误";
            if (code == "10043") return "合约权益错误，提币失败";
            if (code == "10044") return "取消借款失败";
            if (code == "10047") return "当前为子账户，此功能未开放";
            if (code == "10048") return "提币信息不存在";
            if (code == "10049") return "小额委托（<0.15BTC)的未成交委托数量不得大于50个";
            if (code == "10050") return "重复撤单";
            if (code == "10052") return "提币受限";
            if (code == "10064") return "美元充值后的48小时内，该部分资产不能提出";
            if (code == "10100") return "账户被冻结";
            if (code == "10101") return "订单类型错误";
            if (code == "10102") return "不是本用户的订单";
            if (code == "10103") return "私密订单密钥错误";
            if (code == "10216") return "非开放API";
            if (code == "1002") return "交易金额大于余额";
            if (code == "1003") return "交易金额小于最小交易值";
            if (code == "1004") return "交易金额小于0";
            if (code == "1007") return "没有交易市场信息";
            if (code == "1008") return "没有最新行情信息";
            if (code == "1009") return "没有订单";
            if (code == "1010") return "撤销订单与原订单用户不一致";
            if (code == "1011") return "没有查询到该用户";
            if (code == "1013") return "没有订单类型";
            if (code == "1014") return "没有登录";
            if (code == "1015") return "没有获取到行情深度信息";
            if (code == "1017") return "日期参数错误";
            if (code == "1018") return "下单失败";
            if (code == "1019") return "撤销订单失败";
            if (code == "1024") return "币种不存在";
            if (code == "1025") return "没有K线类型";
            if (code == "1026") return "没有基准币数量";
            if (code == "1027") return "参数不合法可能超出限制";
            if (code == "1028") return "保留小数位失败";
            if (code == "1029") return "正在准备中";
            if (code == "1030") return "有融资融币无法进行交易";
            if (code == "1031") return "转账余额不足";
            if (code == "1032") return "该币种不能转账";
            if (code == "1035") return "密码不合法";
            if (code == "1036") return "谷歌验证码不合法";
            if (code == "1037") return "谷歌验证码不正确";
            if (code == "1038") return "谷歌验证码重复使用";
            if (code == "1039") return "短信验证码输错限制";
            if (code == "1040") return "短信验证码不合法";
            if (code == "1041") return "短信验证码不正确";
            if (code == "1042") return "谷歌验证码输错限制";
            if (code == "1043") return "登陆密码不允许与交易密码一致";
            if (code == "1044") return "原密码错误";
            if (code == "1045") return "未设置二次验证";
            if (code == "1046") return "原密码未输入";
            if (code == "1048") return "用户被冻结";
            if (code == "1201") return "账号零时删除";
            if (code == "1202") return "账号不存在";
            if (code == "1203") return "转账金额大于余额";
            if (code == "1204") return "不同种币种不能转账";
            if (code == "1205") return "账号不存在主从关系";
            if (code == "1206") return "提现用户被冻结";
            if (code == "1207") return "不支持转账";
            if (code == "1208") return "没有该转账用户";
            if (code == "1209") return "当前api不可用";
            if (code == "1216") return "市价交易暂停，请选择限价交易";
            if (code == "1217") return "您的委托价格超过最新成交价的±5%，存在风险，请重新下单";
            if (code == "1218") return "下单失败，请稍后再试";
            return "unknow";

        }
    }
}
