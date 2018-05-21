using System;

//Nhibernate Code Generation Template 1.0
//author:MythXin
//blog:www.cnblogs.com/MythXin
//Entity Code Generation Template
namespace Domain{
	 	//管理员信息表
		public class Manager
	{
	
      	/// <summary>
		/// 自增ID
        /// </summary>
        public virtual int id
        {
            get; 
            set; 
        }        

        public virtual int? role_id
        {
            get;
            set;
        }

        public virtual Manager_role manager_role
        {
            get;
            set;
        }
		/// <summary>
		/// 管理员类型1超管2系管
        /// </summary>
        public virtual int? role_type
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
		/// 登录密码
        /// </summary>
        public virtual string password
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 
        /// </summary>
        public virtual string salt
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 用户昵称
        /// </summary>
        public virtual string real_name
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 联系电话
        /// </summary>
        public virtual string mobile
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 电子邮箱
        /// </summary>
        public virtual string email
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 是否锁定
        /// </summary>
        public virtual string is_lock
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 添加时间
        /// </summary>
        public virtual DateTime? add_time
        {
            get; 
            set; 
        }        
		   
	}
}