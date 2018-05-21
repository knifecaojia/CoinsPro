using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using System.Linq.Expressions;
using NHibernate.Criterion;
using DAO.IDAL;
using System.Web;
using Common;
namespace DAO.DAL {
	/// <summary>
	/// 接口层D_Manager_log
	/// </summary>
	public class D_Manager_log:IManager_log
	{
        //因为sqlite添加的时候,会导致连接丢失,这里使用session保存连接信息
        private ISessionFactory sessionFactory = HttpContext.Current.Session["cfg"] as ISessionFactory;
        public D_Manager_log() 
        {
            if (sessionFactory == null)
            {
                string dbtype = "/bin/" + System.Configuration.ConfigurationManager.AppSettings["dbtype"];
                var path = HttpContext.Current.Server.MapPath(dbtype);
                var cfg = new NHibernate.Cfg.Configuration().Configure(path);
                sessionFactory = cfg.BuildSessionFactory();
                HttpContext.Current.Session["cfg"] = sessionFactory;
            }
        }
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
        public virtual object Save(Manager_log model)
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
        public virtual void Update(Manager_log model)
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
                var model = session.Load<Manager_log>(id);
                session.Delete(model);
                session.Flush();
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public virtual void Delete(Manager_log model)
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
        public virtual Manager_log Get(object id)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                return session.Get<Manager_log>(id);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public virtual IList<Manager_log> LoadAll()
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                return session.QueryOver<Manager_log>().List();
            }
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public virtual IList<Manager_log> GetList(List<SearchTemplate> st, List<Order> order)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria crit = session.CreateCriteria(typeof(Manager_log));
                IList<Manager_log> customers = ManagerPage.GetCrit<Manager_log>(st, order, crit);
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
                ICriteria crit = session.CreateCriteria(typeof(Manager_log));
                int count = ManagerPage.GetCrit<Navigation>(st, crit);
                return count;
            }
        }
        #endregion  成员方法
	} 
}