using System;
namespace Domain{
	 	//News_type
		public class News_type
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
		/// 类别标题
        /// </summary>
        public virtual string title
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 父类别ID
        /// </summary>
        public virtual int? parent_id
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 排序数字
        /// </summary>
        public virtual int? sort_id
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 备注说明
        /// </summary>
        public virtual string content
        {
            get; 
            set; 
        }        
		/// <summary>
		/// SEO标题
        /// </summary>
        public virtual string seo_title
        {
            get; 
            set; 
        }        
		/// <summary>
		/// SEO关健字
        /// </summary>
        public virtual string seo_keywords
        {
            get; 
            set; 
        }        
		/// <summary>
		/// SEO描述
        /// </summary>
        public virtual string seo_description
        {
            get; 
            set; 
        }        
		   
	}
}