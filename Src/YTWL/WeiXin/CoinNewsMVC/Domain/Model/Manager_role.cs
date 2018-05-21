using System;
using System.Collections.Generic;

//Nhibernate Code Generation Template 1.0
//author:MythXin
//blog:www.cnblogs.com/MythXin
//Entity Code Generation Template
namespace Domain{
	 	//manager_role
		public class Manager_role
	{
	
      	/// <summary>
		/// id
        /// </summary>
        public virtual int id
        {
            get; 
            set; 
        }        
		/// <summary>
		/// role_name
        /// </summary>
        public virtual string role_name
        {
            get; 
            set; 
        }        
		/// <summary>
		/// role_type
        /// </summary>
        public virtual int? role_type
        {
            get; 
            set; 
        }        
		/// <summary>
		/// is_sys
        /// </summary>
        public virtual int? is_sys
        {
            get; 
            set; 
        }
	}
}