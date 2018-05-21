using System;

//Nhibernate Code Generation Template 1.0
//author:MythXin
//blog:www.cnblogs.com/MythXin
//Entity Code Generation Template
namespace Domain{
	 	//Wiki
		public class Wiki
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
		/// title
        /// </summary>
        public virtual string title
        {
            get; 
            set; 
        }        
		/// <summary>
		/// tags
        /// </summary>
        public virtual string tags
        {
            get; 
            set; 
        }        
		/// <summary>
		/// synopsis
        /// </summary>
        public virtual string synopsis
        {
            get; 
            set; 
        }        
		/// <summary>
		/// wike_id
        /// </summary>
        public virtual string content
        {
            get; 
            set; 
        }        
		/// <summary>
		/// add_time
        /// </summary>
        public virtual DateTime? add_time
        {
            get; 
            set; 
        }        
		/// <summary>
		/// manager_id
        /// </summary>
        public virtual int? manager_id
        {
            get; 
            set; 
        }

        public virtual Manager manager
        {
            get;
            set;
        }
		/// <summary>
		/// parent_id
        /// </summary>
        public virtual int? parent_id
        {
            get; 
            set; 
        }        
		/// <summary>
		/// parent_ids
        /// </summary>
        public virtual string parent_ids
        {
            get; 
            set; 
        }


        public int levels { get; set; }

        public int status { get; set; }

        public int sort_id { get; set; }
	}
}