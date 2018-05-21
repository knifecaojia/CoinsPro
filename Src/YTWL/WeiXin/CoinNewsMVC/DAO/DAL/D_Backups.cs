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
using System.Data;
namespace DAO.DAL {
	/// <summary>
	/// 接口层D_Backups
	/// </summary>
	public class D_Backups:IBackups
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
		public virtual object  Save(Backups model)
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
		public virtual void Update(Backups model)
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
                var model = session.Load<Backups>(id);
                session.Delete(model);
                session.Flush();
            }
		}

		/// <summary>
		/// 删除数据
		/// </summary>
		public virtual void Delete(Backups model)
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
		public virtual Backups Get(object id)
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                return session.Get<Backups>(id);
            }
		}
		
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public virtual IList<Backups> LoadAll()
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                return session.QueryOver<Backups>().List();
            }
		}
		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public virtual IList<Backups>  GetList(List<SearchTemplate> st, List<Order> order)
		{
			using (ISession session = sessionFactory.OpenSession())
            {
                ICriteria crit = session.CreateCriteria(typeof(Backups));
                IList<Backups> customers = ManagerPage.GetCrit<Backups>(st, order, crit);
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
                ICriteria crit = session.CreateCriteria(typeof(Backups));
                int count = ManagerPage.GetCrit<Navigation>(st, crit);
                return count;
            }
        }
        public void BackupDB(string dbName, string filePath, int backType)
        {
            using (ISession session = sessionFactory.OpenSession())
            {

                IDbCommand command = session.Connection.CreateCommand();
                if (backType == 1)
                {
                    command.CommandText = string.Format("backup database {0} to disk ='{1}'", dbName, filePath);
                }
                else if (backType == 2)
                {
                    command.CommandText = string.Format("backup database {0} to disk ='{1}' WITH DIFFERENTIAL ;", dbName, filePath);
                }
                else
                {
                    command.CommandText = string.Format("backup database {0} to disk ='{1}'", dbName, filePath);
                }
                command.ExecuteNonQuery();
            }
        }


        public string GetDbName()
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                return session.Connection.Database;
            }
        }
		#endregion  成员方法
	} 
}