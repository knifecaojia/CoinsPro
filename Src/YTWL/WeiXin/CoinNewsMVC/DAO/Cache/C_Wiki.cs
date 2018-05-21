using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAO.DAL;
using DAO.IDAL;
using NHibernate.Criterion;
using Domain;
using Common;
using CacheRedis;
using System.Web.Configuration;
namespace DAO.Cache {
	/// <summary>
	/// 缓存层D_Wiki
	/// </summary>
	public class C_Wiki:D_Wiki
	{
        //类名 + 方法名 + 参数

        //是否启动主从
        private static bool IsSlave = Convert.ToBoolean(WebConfigurationManager.AppSettings["isslave"]);
        
        //类名
        private string className = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
        
        //redis对象
        private RedisHelper rh = new RedisHelper(IsSlave);
        
        
		#region  成员方法
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public override bool Exists(object id)
		{
			string key = "Wiki:" + id;
			var res = rh.Exist(key);
            if (res > 0)//存在
            {
                return true;
            }
            else 
            {
                var model = base.Get(id);
                if (model != null) 
                {
                    rh.Set<Wiki>(key, model);
                    return true;
                }
                return false;
            }
		}
		/// <summary>
		/// 增加一条数据
		/// </summary>
		public override object  Save(Wiki model)
		{
			var res = base.Save(model);
            if ((int)res > 0) 
            {
                string key = "Wiki:" + res;

                if (rh.Exist(key) <= 0) //不存在
                {
                    rh.Set<Wiki>(key, model);
                    //检查是否存在关联
                    rh.DelJoin("Wiki");
                }
            }
            return res;
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public override void Update(Wiki model)
		{
			string key = "Wiki:" + model.id;
            if (rh.Exist(key) > 0) //存在
            {
                //修改当前集合
                rh.Set<Wiki>(key, model);
                //检查是否存在关联
                rh.DelJoin("Wiki");
            }
            base.Update(model);
		}
		/// <summary>
		/// 删除数据
		/// </summary>
		public override void Delete(object id)
		{
			string key = "Wiki:" + id;
            if (rh.Exist(key) > 0) //存在
            {
                //修改当前集合
                rh.Remove(key);
                //检查是否存在关联
                rh.DelJoin("Wiki");
            }
            base.Delete(id);
		}

		/// <summary>
		/// 删除数据
		/// </summary>
		public override void Delete(Wiki model)
		{
			 string key = "Wiki:" + model.id;
            if (rh.Exist(key) > 0) //存在
            {
                rh.Remove(key);
                //检查是否存在关联
                rh.DelJoin("Wiki");
            }
            base.Delete(model);
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public override Wiki Get(object id)
		{
			string key = "Wiki:" + id;
            if (rh.Exist(key) > 0) //存在
            {
                return rh.Get<Wiki>(key);
            }
            else
            {
                var model = base.Get(id);
                rh.Set<Wiki>(key, model);
                return model;
            }
		}
		
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public override IList<Wiki> LoadAll()
		{
			var functionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            var key = className + ":" + functionName;
            //添加关联
            rh.AddJoin("Wiki", key);
            if (rh.Exist(key) > 0) //存在
            {
                return rh.GetList<Wiki>(key) as IList<Wiki>;
            }
            else 
            {
                IList<Wiki> list = base.LoadAll();
                //写入缓存
                rh.AddList<Wiki>(key, list);
                return list;
            }
		}

		public override IList<Wiki>  GetList(List<SearchTemplate> st, List<Order> order)
		{
			    var functionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
	            var key = className + ":" + functionName + ":" + rh.GetString<SearchTemplate>(st) + ":" + rh.GetString(order); 
	            //添加关联
	            rh.AddJoin("Wiki", key);
	            if (rh.Exist(key) > 0) //存在
	            {
	                return rh.GetList<Wiki>(key) as IList<Wiki>;
	            }
	            else
	            {
	                IList<Wiki> list = base.GetList(st, order);
	                //写入缓存
	                rh.AddList<Wiki>(key, list);
	                return list;
	            }
		}
		
		/// <summary>
        /// 获取总行数
        /// </summary>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        public override int GetCount(List<SearchTemplate> st)
        {
            var functionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            var key = className + ":" + functionName + ":" + rh.GetString<SearchTemplate>(st);
            //添加关联
            rh.AddJoin("Wiki", key);
            if (rh.Exist(key) > 0) //存在
            {
                return rh.Get<int>(key);
            }
            else
            {
                int count = base.GetCount(st);
                //写入缓存
                rh.Set<int>(key, count);
                return count;
            }
        }
		#endregion  成员方法
	} 
}