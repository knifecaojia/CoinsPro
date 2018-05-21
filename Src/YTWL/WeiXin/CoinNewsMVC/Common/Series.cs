using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class xAxis 
    {
        public string type { get; set; }

        public bool boundaryGap { get; set; }

        public List<object> data { get; set; }
    }

    public class Series
    {
        /// <summary>  
        /// series序列组名称  
        /// </summary>  
        public string name
        {
            get;
            set;
        }

        /// <summary>  
        /// series序列组呈现图表类型(line、column、bar等)  
        /// </summary>  
        public string type
        {
            get;
            set;
        }
        /// <summary>  
        /// series序列组的数据为数据类型数组  
        /// </summary>  
        public List<object> data
        {
            get;
            set;
        }
        public itemStyle areaStyle
        {
            get;
            set;
        }
        public string stack { get; set; }
    }

    public class itemStyle
    {
        /// <summary>  
        /// normal  
        /// </summary>  
        public object normal
        {
            get;
            set;
        }
    }
}
