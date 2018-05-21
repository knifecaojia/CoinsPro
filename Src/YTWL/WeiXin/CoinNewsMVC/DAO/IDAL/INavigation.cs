using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using System.Linq.Expressions;
using NHibernate.Criterion;
using Common;
namespace DAO.IDAL {
	/// <summary>
	/// 接口层D_Navigation
	/// </summary>
	public interface INavigation
	{
		#region  成员方法
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		bool Exists(object id);
		/// <summary>
		/// 增加一条数据
		/// </summary>
		object  Save(Navigation model);
		/// <summary>
		/// 更新一条数据
		/// </summary>
		void Update(Navigation model);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(object id);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(Navigation model);
		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		Navigation Get(object id);
		/// <summary>
		/// 获得数据列表
		/// </summary>
		IList<Navigation> LoadAll();

        IList<Navigation> LoadAll(List<Order> order = null, int parent_id = 0, string is_lock = "√");
		/// <summary>
		/// 获得前几行数据
		/// </summary>
        IList<Navigation> GetList(List<SearchTemplate> st, List<Order> order);

        /// <summary>
        /// 获取总行数
        /// </summary>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        int GetCount(List<SearchTemplate> st);
		#endregion  成员方法
	} 
}