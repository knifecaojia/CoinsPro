using System;

//Nhibernate 代码自动生成模板 1.0
//作者:liuliang
//blog:www.cnblogs.com/tibos 
namespace Domain{
	 	//Organization
		public class Organization
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
		/// name
        /// </summary>
        public virtual string name
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
		/// <summary>
		/// parent_id
        /// </summary>
        public virtual int? parent_id
        {
            get; 
            set; 
        }        
		/// <summary>
		/// orgtype_id
        /// </summary>
        public virtual int? orgtype_id
        {
            get; 
            set; 
        }
        public virtual Organization_type orgtype
        {
            get;
            set;
        }

		/// <summary>
		/// sort_id
        /// </summary>
        public virtual int? sort_id
        {
            get; 
            set; 
        }        
		/// <summary>
		/// add_manager_id
        /// </summary>
        public virtual int? add_manager_id
        {
            get; 
            set; 
        }
        public virtual Manager add_manager
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
		/// del_manager_id
        /// </summary>
        public virtual int? del_manager_id
        {
            get; 
            set; 
        }
        public virtual Manager del_manager
        {
            get;
            set;
        }
		/// <summary>
		/// del_time
        /// </summary>
        public virtual DateTime? del_time
        {
            get; 
            set; 
        }        
		/// <summary>
		/// status
        /// </summary>
        public virtual int? status
        {
            get; 
            set; 
        }
        public virtual int? levels
        {
            get;
            set;
        }
		   
	}
}