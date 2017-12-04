using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HFTS_OKEX_Maker_Test
{
    /// <summary>
    /// 测试botvs 高频挂单策略，选取LTC BTC 交易对儿，测试是否有盈利可能
    /// </summary>
    class Program
    {
        static CommonLab.TradePair tp = new CommonLab.TradePair("ltc", "btc");
        static KFCC.ExchangeInterface.IExchanges exchange = null;
        static CommonLab.Account account;
        static CommonLab.Account sa = null;
        static double ba = 1;//卖单深度
        static double aa = 1;//买单深度
       
        static void Main(string[] args)
        {
            #region 交易所Okex现货测试

            exchange = new KFCC.EOkCoin.OkCoinExchange("a8716cf5-8e3d-4037-9a78-6ad59a66d6c4", "CF44F1C9F3BB23B148523B797B862D4C", "", "");
            exchange.Subscribe(tp, CommonLab.SubscribeTypes.RESTAPI);
            while (true)
            {
                try
                {
                    if (DateTime.Now.Hour == 14)
                    {
                        Console.WriteLine("时间到了，看看效果吧");
                        Console.ReadKey();
                        return;
                    }

                    string raw = "";
                    CommonLab.Depth d = exchange.GetDepth(exchange.GetLocalTradingPairString(tp), out raw);
                    //Console.WriteLine(d.ToString(10));
                    double bprice = d.CaculateDepth(CommonLab.OrderType.ORDER_TYPE_BUY, ba);
                    double sprice = d.CaculateDepth(CommonLab.OrderType.ORDER_TYPE_SELL, aa);
                    double diff = sprice - bprice;
                    //
                    if (diff > 0.00000005)
                    {
                        //Console.WriteLine("买单价格：{0}  卖单价格：{1}  差价：{2}", bprice, sprice, diff.ToString("F8"));
                    }
                    else
                    {
                        bprice -= 0.00000005;
                        sprice += 0.00000005;


                    }

                    if (exchange.CancelAllOrders())
                        continue;




                    account = exchange.GetAccount(out raw);
                    if (sa == null)
                        sa = account.Clone();
                    double cansellamount = account.Balances[tp.FromSymbol].available;
                    double canbuyamout = account.Balances[tp.ToSymbol].available / sprice;
                    canbuyamout = Math.Min(canbuyamout, ba);
                    cansellamount = Math.Min(cansellamount, aa);

                    Console.WriteLine("下单买：{0} 下单卖{1} ，差价：{2} 账户可卖：{3} 账户可买：{4}", bprice, sprice, diff.ToString("F8"), cansellamount, canbuyamout);
                    double sltc, nltc, cp;
                    sltc = sa.Balances[tp.FromSymbol].balance + sa.Balances[tp.ToSymbol].balance / d.Asks[0].Price;
                    nltc = account.Balances[tp.FromSymbol].balance + account.Balances[tp.ToSymbol].balance / d.Asks[0].Price;
                    cp = 100 * nltc / sltc;

                    double sbtc, nbtc, bcp;
                    sbtc = sa.Balances[tp.ToSymbol].balance + sa.Balances[tp.FromSymbol].balance * d.Bids[0].Price;
                    nbtc = account.Balances[tp.ToSymbol].balance + account.Balances[tp.FromSymbol].balance * d.Bids[0].Price;
                    bcp = 100 * nbtc / sbtc;
                    Console.WriteLine("初始LTC:{0} 当前LTC:{1} 变化:{2}%  初始BTC:{3} 当前BTC:{4}变化:{2}% ", sltc, nltc, cp.ToString("f4"), sbtc, nbtc, bcp.ToString("f4"));

                    if (Math.Min(canbuyamout, cansellamount) > 0.01)
                    {
                        exchange.Sell(tp.FromSymbol + "_" + tp.ToSymbol, sprice, cansellamount);
                        exchange.Buy(tp.FromSymbol + "_" + tp.ToSymbol, bprice, canbuyamout);
                        Thread.Sleep(3500);
                    }
                    else if (canbuyamout <= 0.01)
                    {
                        exchange.Sell(tp.FromSymbol + "_" + tp.ToSymbol, d.Asks[0].Price, cansellamount);
                        Thread.Sleep(10000);
                    }
                    else if (cansellamount <= 0.01)
                    {
                        exchange.Buy(tp.FromSymbol + "_" + tp.ToSymbol, d.Bids[0].Price, canbuyamout);
                        Thread.Sleep(10000);
                    }

                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Thread.Sleep(3000);
                }

             
             
              
            }
            //exchange.TickerEvent += Exchange_TickerEvent;
            //exchange.DepthEvent += Exchange_DepthEvent;
            #endregion
        }
    }
}
