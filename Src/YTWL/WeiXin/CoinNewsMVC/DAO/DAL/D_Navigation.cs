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
	/// 接口层D_Navigation
	/// </summary>
	public class D_Navigation:INavigation
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
		public virtual object  Save(Navigation model)
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
		public virtual void Update(Navigation model)
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
                var model = session.Load<Navigation>(id);
                session.Delete(model);
                session.Flush();
            }
		}

		/// <summary>
		/// 删除数据
		/// </summary>
		public virtual void Delete(Navigation model)
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
		public virtual Navigation Get(object id)
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                return session.Get<Navigation>(id);
            }
		}
		
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public virtual IList<Navigation> LoadAll()
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                return session.QueryOver<Navigation>().List();
            }
		}

        public IList<Navigation> LoadAll(List<Order> order = null, int parent_id = 0, string is_lock = "√")
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria crit = session.CreateCriteria(typeof(Navigation));
                if (order == null)
                {
                    crit.AddOrder(Order.Desc("id"));
                }
                else
                {
                    foreach (var item in order)
                    {
                        crit.AddOrder(item);
                    }
                }
                if (parent_id != -1)
                {
                    crit.Add(Restrictions.Eq("parent_id", parent_id));
                }
                IList<Navigation> customers = crit.List<Navigation>();
                return customers;
            }
        }

		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public virtual IList<Navigation>  GetList(List<SearchTemplate> st, List<Order> order)
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria crit = session.CreateCriteria(typeof(Navigation));
                IList<Navigation> customers = ManagerPage.GetCrit<Navigation>(st, order, crit);
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
                ICriteria crit = session.CreateCriteria(typeof(Navigation));
                int count = ManagerPage.GetCrit<Navigation>(st, crit);
                return count;
            }
        }
		#endregion  成员方法
	} 
}