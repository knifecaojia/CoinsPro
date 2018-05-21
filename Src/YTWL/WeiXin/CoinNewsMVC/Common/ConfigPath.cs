using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    /// <summary>
    /// 获取配置路径(好处,便于维护)
    /// </summary>
    public class ConfigPath
    {
        /// <summary>
        /// 获取子模板路径
        /// </summary>
        /// <param name="ActionName">控制器名字</param>
        /// <returns></returns>
        public static string GetTempletPaht(string ActionName) 
        {
            return "../Templet/" + ActionName;
        }
    }
}
