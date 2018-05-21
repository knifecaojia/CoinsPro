using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAO.DAL;
using DAO.IDAL;
using Domain;
using NHibernate.Criterion;
using Common;
using DAO.Cache;
namespace DAO.BLL {
	public partial class B_Organization
	{
		private readonly IOrganization dal;
		public B_Organization()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_Organization();
            }
            else
            {
                dal = new D_Organization();
            }
        }
		
		#region  Method
        public IList<Domain.Organization> GetList(List<SearchTemplate> st, List<Order> order) 
        {
            return dal.GetList(st,order);
        }

        public int GetCount(List<SearchTemplate> st) 
        {
            return dal.GetCount(st);
        }
        
        public int Save(Domain.Organization model) 
        {
            return Convert.ToInt32(dal.Save(model));
        }
        
        public Domain.Organization Get(object id) 
        {
            return dal.Get(id);
        }
        
        public void Delete(object id) 
        {
             dal.Delete(id);
        }
        
        public void Update(Domain.Organization model) 
        {
            dal.Update(model);
        }
		#endregion
   
	}
}