using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OKCoinAPI
{
    public class DataFilter
    {
        Queue<Ticker> tickerQueue = new Queue<Ticker>();
        List<Decimal> dataList = new List<decimal>();

        int queueMaxCount = 3600 * 2;
        public int QueueMaxCount
        {
            get { return queueMaxCount; }
            set { queueMaxCount = value; }
        }

        Decimal avgValue = 0;
        public Decimal AvgValue
        {
            get { return avgValue; }
        }

        HistoryFile historyFile;

        public DataFilter()
        {
            historyFile = new HistoryFile();
            LoadHistory();

        }

        void LoadHistory()
        {
            //加载历史数据
            List<Ticker> tickerList = historyFile.Load(DateTime.Now.AddDays(-1), DateTime.Now);
            if (tickerList.Count < 2) return;

            List<Decimal> valuelist = new List<decimal>();
            foreach (Ticker item in tickerList)
            {
                valuelist.Add(item.Last);
                tickerQueue.Enqueue(item);
            }

            Decimal avgValue = AvgFilter(valuelist);

            Console.WriteLine("History Avg: {0:N3}", avgValue);

        }

        Decimal  AvgFilter(List<Decimal> valuelist)
        {
            valuelist.Sort();
            valuelist.RemoveAt(0);
            valuelist.RemoveAt(valuelist.Count - 1);
            avgValue = (Decimal)(valuelist.Sum() / valuelist.Count);
            return avgValue;
        }

        public void Enqueue(Ticker value)
        {
            historyFile.Write(value);

            lock (tickerQueue)
            {
                tickerQueue.Enqueue(value);

                if (tickerQueue.Count < queueMaxCount) return;
                                   
                tickerQueue.Dequeue();

                
                dataList.Clear();
                foreach (Ticker item in tickerQueue)
                {
                    dataList.Add(item.Last);
                }
                
            }

               
            Filter();
            

        }

        void Filter()
        {
            //中位值平均滤波法（又称防脉冲干扰平均滤波法）
            //方法：
            //   相当于“中位值滤波法”+“算术平均滤波法”
            //   连续采样N个数据，去掉一个最大值和一个最小值
            //   然后计算N-2个数据的算术平均值
            //   N值的选取：3~14

            dataList.Sort();
            dataList.RemoveAt(0);
            dataList.RemoveAt(dataList.Count - 1);
            avgValue = (Decimal)(dataList.Sum() / dataList.Count);

            //Console.Write("\r\n{0:G}, ", DateTime.Now);

            //foreach (Decimal n in dataList)
            //{
            //    Console.Write(" {0:N3},", n);
            //}
            
            Console.Write(" Avg: {0:N3}", avgValue);

        }

    }


}
