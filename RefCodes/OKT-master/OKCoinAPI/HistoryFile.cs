using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OKCoinAPI
{
    class HistoryFile
    {
        FileStream fileStream = null;

        BinaryWriter binaryWriter = null;

        string fileName;

        long fileSize;

        static int REC_SIZE = 100;  //首记录为文件头记录


        public HistoryFile(string fileName = "History.dat", long filesize = (1 + 12*3600) * 100)
        {
            this.fileName = fileName;

            this.fileSize = filesize;

            OpenFile();

        }


        public void OpenFile()
        {
            try
            {
                fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
              
                binaryWriter = new BinaryWriter(fileStream);

                //读取最后记录的文件位置
                byte[] buffer = new byte[8];

                fileStream.Read(buffer, 0, 8);

                Int64 position = BitConverter.ToInt64(buffer, 0);

                if (position > 0)
                {
                    fileStream.Position = position;
                }
                else
                {
                    fileStream.Seek(REC_SIZE, SeekOrigin.Begin);
                }

            }
            catch
            {
            }

        }

        public void CloseFile()
        {
            try
            {
                if (binaryWriter != null)
                {
                    binaryWriter.Flush();
                    binaryWriter.Close();
                }

                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                }

            }
            catch
            {
            }

        }

        Int64 WritePosition()
        {
            Int64 position = fileStream.Position;

            fileStream.Seek(0, SeekOrigin.Begin);

            binaryWriter.Write(position);

            return position;

        }

        public void Write(Ticker value)
        {
            //每个记录100字节
            Int64 position = WritePosition();
            fileStream.Seek(position, SeekOrigin.Begin);

            binaryWriter.Write(UnixTime.ToInt32(value.Time));
            binaryWriter.Write(value.High);
            binaryWriter.Write(value.Low);
            binaryWriter.Write(value.Buy);
            binaryWriter.Write(value.Sell);
            binaryWriter.Write(value.Last);
            binaryWriter.Write(value.Volume);
            
            //如果数据长度达到指定的文件长度，则返回开始位置循环写入
            binaryWriter.Flush();
            if (fileStream.Position >= fileSize)
            {
                fileStream.Seek(REC_SIZE, SeekOrigin.Begin);
            }

        }

        public List<Ticker> Load(DateTime beginTime, DateTime endTime)
        {
            List<Ticker> tickerList = new List<Ticker>();

            int iTime = 0;

            DateTime dTime = DateTime.MinValue;

            Decimal high = 0,
                    low = 0,
                    buy = 0,
                    sell = 0,
                    last = 0,
                    volume = 0;

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (fs == null || fs.Length < 1) return tickerList;

                using (BinaryReader binaryReader = new BinaryReader(fs))
                {
                    while (true)
                    {
                        try
                        {
                            iTime = binaryReader.ReadInt32();
                            dTime = UnixTime.FromInt32(iTime);

                            high = binaryReader.ReadDecimal();
                            low = binaryReader.ReadDecimal();
                            buy = binaryReader.ReadDecimal();
                            sell = binaryReader.ReadDecimal();
                            last = binaryReader.ReadDecimal();
                            volume = binaryReader.ReadDecimal();


                            if (dTime > beginTime && dTime <= endTime)
                            {
                                Ticker ticker = new Ticker();
                                ticker.Time = dTime;
                                ticker.High = high;
                                ticker.Low = low;
                                ticker.Buy = buy;
                                ticker.Sell = sell;
                                ticker.Last = last;
                                ticker.Volume = volume;

                                tickerList.Add(ticker);
                            }

                        }
                        catch (EndOfStreamException ex)
                        {
                            break;
                        }
                    }

                }
            }
            
            List<Ticker> sortList = (from entry in tickerList orderby entry.Time ascending select entry).ToList();

            return sortList;

        }
    }
}
