using System;

//Nhibernate Code Generation Template 1.0
//author:MythXin
//blog:www.cnblogs.com/MythXin
//Entity Code Generation Template
namespace Domain
{
    //管理日志表
    public class Manager_log
    {

        /// <summary>
        /// 自增ID
        /// </summary>
        public virtual int id
        {
            get;
            set;
        }
        /// <summary>
        /// 用户ID
        /// </summary>
        public virtual int? user_id
        {
            get;
            set;
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string user_name
        {
            get;
            set;
        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public virtual string action_type
        {
            get;
            set;
        }
        /// <summary>
        /// nav_id
        /// </summary>
        public virtual int? nav_id
        {
            get;
            set;
        }

        public virtual Navigation navigation
        {
            get;
            set;
        }

        /// <summary>
        /// table_id
        /// </summary>
        public virtual int? table_id
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string remark
        {
            get;
            set;
        }
        /// <summary>
        /// 用户IP
        /// </summary>
        public virtual string user_ip
        {
            get;
            set;
        }
        /// <summary>
        /// 操作时间
        /// </summary>
        public virtual DateTime? add_time
        {
            get;
            set;
        }

    }
}