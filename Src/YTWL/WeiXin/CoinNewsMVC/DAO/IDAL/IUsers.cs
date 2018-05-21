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
	/// 接口层D_Users
	/// </summary>
	public interface IUsers
	{
		#region  成员方法
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		bool Exists(object id);

		/// <summary>
		/// 增加一条数据
		/// </summary>
		object  Save(Users model);
		/// <summary>
		/// 更新一条数据
		/// </summary>
		void Update(Users model);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(int id);
		/// <summary>
		/// 删除数据
		/// </summary>
		void Delete(Users model);
		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		Users Get(object id);
		/// <summary>
		/// 获得数据列表
		/// </summary>
		IList<Users> LoadAll();
		/// <summary>
		/// 获得前几行数据
		/// </summary>
		IList<Users>  GetList(int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="count"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user_name"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        IList<Users> GetList(List<SearchTemplate> st, List<Order> order);

        /// <summary>
        /// 获取总条数
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user_name"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        int GetCount(List<SearchTemplate> st);
        

		#endregion  成员方法
	} 
}