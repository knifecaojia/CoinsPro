using System;
namespace Domain{
	 	//News
		public class News
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
		/// 类别ID
        /// </summary>
        public virtual int type_id
        {
            get; 
            set; 
        }

        public virtual News_type news_type
        {
            get;
            set;
        }

		/// <summary>
		/// 标题
        /// </summary>
        public virtual string title
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 外部链接
        /// </summary>
        public virtual string link_url
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 图片地址
        /// </summary>
        public virtual string img_url
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
		/// <summary>
		/// TAG标签逗号分隔
        /// </summary>
        public virtual string tags
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 内容摘要
        /// </summary>
        public virtual string summary
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 详细内容
        /// </summary>
        public virtual string content
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 排序
        /// </summary>
        public virtual int? sort_id
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 浏览次数
        /// </summary>
        public virtual int? click
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 状态0未审核1成功2失败
        /// </summary>
        public virtual int? status
        {
            get; 
            set; 
        }
        public virtual string is_lock
        {
            get;
            set;
        }

		/// <summary>
		/// 是否允许评论
        /// </summary>
        public virtual int? is_msg
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 是否置顶
        /// </summary>
        public virtual int? is_top
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 是否推荐
        /// </summary>
        public virtual int? is_red
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 是否热门
        /// </summary>
        public virtual int? is_hot
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 是否幻灯片
        /// </summary>
        public virtual int? is_slide
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 用户名
        /// </summary>
        public virtual string manager_id
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
		/// 创建时间
        /// </summary>
        public virtual DateTime? add_time
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 发布时间
        /// </summary>
        public virtual DateTime? start_time
        {
            get; 
            set; 
        }
        /// <summary>
        /// 修改时间
        /// </summary>
        public virtual DateTime? update_time
        {
            get;
            set;
        }

        /// <summary>
        /// 来源
        /// </summary>
        public virtual string source { get; set; }

            /// <summary>
            /// 作者
            /// </summary>
        public virtual string author { get; set; }
		   
	}
}