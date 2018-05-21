using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NHibernate.Criterion;
using System.Linq.Expressions;
using Common;
namespace DAO.IDAL {
	/// <summary>
	/// 接口层D_Manager
	/// </summary>
	public interface IManager
	{
		#region  成员方法
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		bool Exists(object id);
		/// <summary>
		/// 增加一条数据
		/// </summary>
		object  Save(Manager model);
		/// <summary>
		/// 更新一条数据
		/// </summary>
		void Update(Manager model);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(object id);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(Manager model);
		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		Manager Get(object id);
		/// <summary>
		/// 获得数据列表
		/// </summary>
		IList<Manager> LoadAll();
		/// <summary>
		/// 获得前几行数据
		/// </summary>
        IList<Domain.Manager> GetList(List<SearchTemplate> st, List<Order> order);

        int GetCount(List<SearchTemplate> st);
		#endregion  成员方法
	} 
}