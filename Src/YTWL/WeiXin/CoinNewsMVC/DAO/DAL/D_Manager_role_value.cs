﻿using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using System.Linq.Expressions;
using NHibernate.Criterion;
using DAO.IDAL;
using Common;
namespace DAO.DAL
{
	/// <summary>
	/// 接口层D_Manager_role_value
	/// </summary>
	public class D_Manager_role_value:IManager_role_value
	{
	    private ISessionFactory sessionFactory = ManagerPage.SessionFactory;
		#region  成员方法
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public virtual bool Exists(object id)
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                return Get(id) != null;
            }
		}
		/// <summary>
		/// 增加一条数据
		/// </summary>
		public virtual object  Save(Manager_role_value model)
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                var id = session.Save(model);
                session.Flush();
                return id;
            }
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public virtual void Update(Manager_role_value model)
		{
			using (var session = sessionFactory.OpenSession())
            {
                session.SaveOrUpdate(model);
                session.Flush();
            }
		}
		/// <summary>
		/// 删除数据
		/// </summary>
		public virtual void Delete(object id)
		{
			using (var session = sessionFactory.OpenSession())
            {
                var model = session.Load<Manager_role_value>(id);
                session.Delete(model);
                session.Flush();
            }
		}

		/// <summary>
		/// 删除数据
		/// </summary>
		public virtual void Delete(Manager_role_value model)
		{
			using (var session = sessionFactory.OpenSession())
            {
                session.Delete(model);

                session.Flush();
            }
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public virtual Manager_role_value Get(object id)
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                return session.Get<Manager_role_value>(id);
            }
		}
		
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public virtual IList<Manager_role_value> LoadAll()
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                return session.QueryOver<Manager_role_value>().List();
            }
		}
		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public virtual IList<Manager_role_value>  GetList(List<SearchTemplate> st, List<Order> order)
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria crit = session.CreateCriteria(typeof(Manager_role_value));
                IList<Manager_role_value> customers = ManagerPage.GetCrit<Manager_role_value>(st, order, crit);
                return customers;
            }
		}
		
		/// <summary>
        /// 获取总行数
        /// </summary>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        public virtual int GetCount(List<SearchTemplate> st)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria crit = session.CreateCriteria(typeof(Manager_role_value));
                int count = ManagerPage.GetCrit<Navigation>(st, crit);
                return count;
            }
        }
		#endregion  成员方法
	} 
}