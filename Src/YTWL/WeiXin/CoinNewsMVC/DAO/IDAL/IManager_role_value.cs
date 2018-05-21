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
	/// 接口层D_Manager_role_value
	/// </summary>
	public interface IManager_role_value
	{
		#region  成员方法
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		bool Exists(object id);
		/// <summary>
		/// 增加一条数据
		/// </summary>
		object  Save(Manager_role_value model);
		/// <summary>
		/// 更新一条数据
		/// </summary>
		void Update(Manager_role_value model);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(object id);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(Manager_role_value model);
		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		Manager_role_value Get(object id);
		/// <summary>
		/// 获得数据列表
		/// </summary>
		IList<Manager_role_value> LoadAll();
		/// <summary>
		/// 获得前几行数据
		/// </summary>
        IList<Manager_role_value> GetList(List<SearchTemplate> st,List<Order> order);

        int GetCount(List<SearchTemplate> st);
		#endregion  成员方法
	} 
}