using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAO.DAL;
using DAO.IDAL;
using NHibernate.Criterion;
using Domain;
using Common;
using DAO.Cache;
namespace DAO.BLL {
	public partial class B_Backups
	{
		private readonly IBackups dal;
		public B_Backups()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_Backups();
            }
            else
            {
                dal = new D_Backups();
            }
        }
		
		#region  Method
        public IList<Domain.Backups> GetList(List<SearchTemplate> st, List<Order> order)
        {
            return dal.GetList(st, order);
        }
        public int GetCount(List<SearchTemplate> st)
        {
            return dal.GetCount(st);
        }

        public void Delete(int id) 
        {
            dal.Delete(id);
        }

        public void BackupDB(string dbName, string filePath,int backType)
        {
            dal.BackupDB(dbName, filePath,backType);
        }

        public string GetDbName() 
        {
            return dal.GetDbName();
        }

        public int Save(Domain.Backups model) 
        {
            return Convert.ToInt32(dal.Save(model));
        }

        public Domain.Backups Get(int id) 
        {
            return dal.Get(id);
        }

        public void Update(Domain.Backups model) 
        {
            dal.Update(model);
        }
		#endregion
   
	}
}