using System;

//Nhibernate Code Generation Template 1.0
//author:MythXin
//blog:www.cnblogs.com/MythXin
//Entity Code Generation Template
namespace Domain{
	 	//manager_role_value
		public class Manager_role_value
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
		/// role_id
        /// </summary>
        public virtual int? role_id
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
		/// <summary>
		/// action_type
        /// </summary>
        public virtual string action_type
        {
            get; 
            set; 
        }        
		   
	}
}