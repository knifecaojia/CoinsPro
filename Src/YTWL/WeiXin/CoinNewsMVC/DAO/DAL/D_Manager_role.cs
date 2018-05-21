using NHibernate;
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
	/// 接口层D_Manager_role
	/// </summary>
	public class D_Manager_role:IManager_role
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
		public virtual object  Save(Manager_role model)
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
		public virtual void Update(Manager_role model)
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
                var model = session.Load<Manager_role>(id);
                session.Delete(model);
                session.Flush();
            }
		}

		/// <summary>
		/// 删除数据
		/// </summary>
		public virtual void Delete(Manager_role model)
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
		public virtual Manager_role Get(object id)
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                return session.Get<Manager_role>(id);
            }
		}
		
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public virtual IList<Manager_role> LoadAll()
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                return session.QueryOver<Manager_role>().List();
            }
		}
		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public virtual IList<Manager_role>  GetList(List<SearchTemplate> st, List<Order> order)
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria crit = session.CreateCriteria(typeof(Manager_role));
                IList<Manager_role> customers = ManagerPage.GetCrit<Manager_role>(st, order, crit);
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
                ICriteria crit = session.CreateCriteria(typeof(Manager_role));
                int count = ManagerPage.GetCrit<Navigation>(st, crit);
                return count;
            }
        }
		#endregion  成员方法
	} 
}