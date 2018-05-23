using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public static class ExchangeNameConvert
    {
        public static string GetLongExchangeName(string _shortName)
        {
            return ConvertName(_shortName);
        }
        /// <summary>
        /// 获取交易所短名字，长度为2
        /// </summary>
        /// <param name="_longName"></param>
        /// <returns>返回交易所短名字，长度固定为2</returns>
        public static string GetShortExchangeName(string _longName)
        {
            return ConvertName(_longName);
        }
        static string ConvertName(string _input)
        {
            if (_input.Length == 2)
            {
                switch (_input)
                {
                    case "BN":
                        return "Binance";
                    default:
                        return "";
                }
            }
            else
            {
                switch (_input)
                {
                    case "Binance":
                        return "BN";
                    default:
                        return "";
                }
            }
        }
    }
}
