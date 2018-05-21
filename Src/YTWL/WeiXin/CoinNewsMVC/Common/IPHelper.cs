
namespace Hui.Utils
{
    using System;
    using System.IO;
    using System.Linq;
    
    /// <summary>
    /// 作者：惠
    /// 日期：2017-01-04
    /// 说明：
    /// 根据本地ip地址库获取ip地址所对应的地区信息
    /// </summary>
    public class IPHelper
    {
        static IPHelper()
        {
            const string fileName = "App_Data/ipdb.dat";
            string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            IPHelper.EnableFileWatch = true;
            IPHelper.Load(configFile);
        }
        /// <summary>
        /// 获取IP地址所在地
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="separator">分割符，默认-</param>
        /// <returns>如："中国-湖北-武汉"</returns>
        public static string GetFullName(string ip, string separator = "-")
        {
            string fullname = "";
            string[] ipstrarr = IPHelper.Find(ip);
            ipstrarr = ipstrarr.Where(a => !string.IsNullOrWhiteSpace(a)).Distinct().ToArray();
            fullname = string.Join(separator, ipstrarr);
            return fullname;
        }

        private static bool EnableFileWatch = false;

        private static int offset;
        private static uint[] index = new uint[256];
        private static byte[] dataBuffer;
        private static byte[] indexBuffer;
        private static long lastModifyTime = 0L;
        private static string ipFile;
        private static readonly object @lock = new object();

        private static void Load(string filename)
        {
            ipFile = new FileInfo(filename).FullName;
            Load();
            if (EnableFileWatch)
            {
                Watch();
            }
        }
        /// <summary>
        /// 根据IP地址返回数组
        /// </summary>
        /// <param name="ip">ip:111.122.72.124</param>
        /// <returns>如["中国","湖北","武汉",""]</returns>
        public static string[] Find(string ip)
        {
            lock (@lock)
            {
                var ips = ip.Split('.');
                var ip_prefix_value = int.Parse(ips[0]);
                long ip2long_value = BytesToLong(byte.Parse(ips[0]), byte.Parse(ips[1]), byte.Parse(ips[2]),
                    byte.Parse(ips[3]));
                var start = index[ip_prefix_value];
                var max_comp_len = offset - 1028;
                long index_offset = -1;
                var index_length = -1;
                byte b = 0;
                for (start = start * 8 + 1024; start < max_comp_len; start += 8)
                {
                    if (
                        BytesToLong(indexBuffer[start + 0], indexBuffer[start + 1], indexBuffer[start + 2],
                            indexBuffer[start + 3]) >= ip2long_value)
                    {
                        index_offset = BytesToLong(b, indexBuffer[start + 6], indexBuffer[start + 5],
                            indexBuffer[start + 4]);
                        index_length = 0xFF & indexBuffer[start + 7];
                        break;
                    }
                }
                var areaBytes = new byte[index_length];
                Array.Copy(dataBuffer, offset + (int)index_offset - 1024, areaBytes, 0, index_length);
                return System.Text.Encoding.UTF8.GetString(areaBytes).Split('\t');
            }
        }

        private static void Watch()
        {
            var file = new FileInfo(ipFile);
            if (file.DirectoryName == null) return;
            var watcher = new FileSystemWatcher(file.DirectoryName, file.Name) { NotifyFilter = NotifyFilters.LastWrite };
            watcher.Changed += (s, e) =>
            {
                var time = File.GetLastWriteTime(ipFile).Ticks;
                if (time > lastModifyTime)
                {
                    Load();
                }
            };
            watcher.EnableRaisingEvents = true;
        }

        private static void Load()
        {
            lock (@lock)
            {
                var file = new FileInfo(ipFile);
                lastModifyTime = file.LastWriteTime.Ticks;
                try
                {
                    dataBuffer = new byte[file.Length];
                    using (var fin = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        fin.Read(dataBuffer, 0, dataBuffer.Length);
                    }

                    var indexLength = BytesToLong(dataBuffer[0], dataBuffer[1], dataBuffer[2], dataBuffer[3]);
                    indexBuffer = new byte[indexLength];
                    Array.Copy(dataBuffer, 4, indexBuffer, 0, indexLength);
                    offset = (int)indexLength;

                    for (var loop = 0; loop < 256; loop++)
                    {
                        index[loop] = BytesToLong(indexBuffer[loop * 4 + 3], indexBuffer[loop * 4 + 2],
                            indexBuffer[loop * 4 + 1],
                            indexBuffer[loop * 4]);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private static uint BytesToLong(byte a, byte b, byte c, byte d)
        {
            return ((uint)a << 24) | ((uint)b << 16) | ((uint)c << 8) | d;
        }

        

    }
   
}

