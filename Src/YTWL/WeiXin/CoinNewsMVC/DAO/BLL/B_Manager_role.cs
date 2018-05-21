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
	public partial class B_Manager_role
	{
		private readonly IManager_role dal;
		public B_Manager_role()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_Manager_role();
            }
            else
            {
                dal = new D_Manager_role();
            }
        }
		
		#region  Method
        public Domain.Manager_role Get(int id) 
        {
            return dal.Get(id);
        }

        public IList<Domain.Manager_role> LoadAll() 
        {
            return dal.LoadAll();
        }
        public IList<Domain.Manager_role> GetList(List<SearchTemplate> st, List<Order> order)
        {
            return dal.GetList(st,order);
        }

        public int GetCount(List<SearchTemplate> st)
        {
            return dal.GetCount(st);
        }

        public int Save(Domain.Manager_role model) 
        {
            return Convert.ToInt32(dal.Save(model));
        }

        public void Update(Domain.Manager_role model) 
        {
            dal.Update(model);
        }

        public void Delete(int id) 
        {
            dal.Delete(id);
        }
		#endregion
   
	}
}