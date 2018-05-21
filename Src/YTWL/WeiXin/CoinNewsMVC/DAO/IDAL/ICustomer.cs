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
	/// 接口层D_Customer
	/// </summary>
	public interface ICustomer
	{
        #region  成员方法
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        bool Exists(object id);
        /// <summary>
        /// 增加一条数据
        /// </summary>
        object Save(Customer model);
        /// <summary>
        /// 更新一条数据
        /// </summary>
        void Update(Customer model);
        /// <summary>
        /// 删除数据
        /// </summary>
        void Delete(object id);
        /// <summary>
        /// 删除数据
        /// </summary>
        void Delete(Customer model);
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        Customer Get(object id);
        /// <summary>
        /// 获得数据列表
        /// </summary>
        IList<Customer> LoadAll();
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        IList<Domain.Customer> GetList(List<SearchTemplate> st, List<Order> order);

        int GetCount(List<SearchTemplate> st);
        #endregion  成员方法
	} 
}