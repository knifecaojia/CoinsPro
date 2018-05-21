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
	/// 接口层D_Manager_role
	/// </summary>
	public interface IManager_role
	{
		#region  成员方法
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		bool Exists(object id);
		/// <summary>
		/// 增加一条数据
		/// </summary>
		object  Save(Manager_role model);
		/// <summary>
		/// 更新一条数据
		/// </summary>
		void Update(Manager_role model);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(object id);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(Manager_role model);
		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		Manager_role Get(object id);
		/// <summary>
		/// 获得数据列表
		/// </summary>
		IList<Manager_role> LoadAll();
		/// <summary>
		/// 获得前几行数据
		/// </summary>
        IList<Manager_role> GetList(List<SearchTemplate> st, List<Order> order);

        int GetCount(List<SearchTemplate> st); 
		#endregion  成员方法
	} 
}