using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    [Serializable]
    public class SearchTemplate
    {
        public string key { get; set; }
        public object value { get; set; }
        public Common.EnumBase.SearchType searchType { get; set; }

    }
}
