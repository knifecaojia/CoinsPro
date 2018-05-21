using System;

//Nhibernate 代码自动生成模板 1.0
//作者:liuliang
//blog:www.cnblogs.com/tibos 
namespace Domain{
	 	//Organization_type
		public class Organization_type
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
		/// orgtype_name
        /// </summary>
        public virtual string orgtype_name
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
		/// remark
        /// </summary>
        public virtual string remark
        {
            get; 
            set; 
        }        
		   
	}
}