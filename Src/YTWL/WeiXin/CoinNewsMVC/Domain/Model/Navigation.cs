using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
namespace Domain
{
    //系统导航菜单

    [Serializable]
    public class Navigation
    {

        /// <summary>
        /// 自增ID
        /// </summary>		
        private int _id;
        public int id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// 所属父导航ID
        /// </summary>		
        private int _parent_id;
        public int parent_id
        {
            get { return _parent_id; }
            set { _parent_id = value; }
        }
        /// <summary>
        /// 菜单等级
        /// </summary>		
        private int _channel_id;
        public int channel_id
        {
            get { return _channel_id; }
            set { _channel_id = value; }
        }
        /// <summary>
        /// 导航类别
        /// </summary>		
        private string _nav_type;
        public string nav_type
        {
            get { return _nav_type; }
            set { _nav_type = value; }
        }
        /// <summary>
        /// 导航ID
        /// </summary>		
        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 标题
        /// </summary>		
        private string _title;
        public string title
        {
            get { return _title; }
            set { _title = value; }
        }
        /// <summary>
        /// 副标题
        /// </summary>		
        private string _sub_title;
        public string sub_title
        {
            get { return _sub_title; }
            set { _sub_title = value; }
        }
        /// <summary>
        /// 图标地址
        /// </summary>		
        private string _icon_url;
        public string icon_url
        {
            get { return _icon_url; }
            set { _icon_url = value; }
        }
        /// <summary>
        /// 链接地址
        /// </summary>		
        private string _link_url;
        public string link_url
        {
            get { return _link_url; }
            set { _link_url = value; }
        }
        /// <summary>
        /// 排序数字
        /// </summary>		
        private int _sort_id;
        public int sort_id
        {
            get { return _sort_id; }
            set { _sort_id = value; }
        }
        /// <summary>
        /// 是否隐藏0显示1隐藏
        /// </summary>		
        private string _is_lock;
        public string is_lock
        {
            get { return _is_lock; }
            set { _is_lock = value; }
        }
        /// <summary>
        /// 备注说明
        /// </summary>		
        private string _remark;
        public string remark
        {
            get { return _remark; }
            set { _remark = value; }
        }
        /// <summary>
        /// 权限资源
        /// </summary>		
        private string _action_type;
        public string action_type
        {
            get { return _action_type; }
            set { _action_type = value; }
        }
        /// <summary>
        /// 系统默认
        /// </summary>		
        private int _is_sys;
        public int is_sys
        {
            get { return _is_sys; }
            set { _is_sys = value; }
        }
        public string controllerName { get; set; }
    }
}

