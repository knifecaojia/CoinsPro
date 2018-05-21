using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuiEF.Data.HuiEntity
{

    [Table("Hui_Login_Log")]
    public class Login_Log
    {
        public Login_Log()
        {
            create_time = DateTime.Now;
        }
        [Key]
        public int log_id { get; set; }
        public string ip { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string remarks { get; set; }

        public string member_id { get; set; }
        public string area { get; set; }

        public DateTime create_time { get; set; }

    }
}
