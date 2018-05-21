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
	public partial class B_Organization_type
	{
		private readonly IOrganization_type dal;
		public B_Organization_type()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_Organization_type();
            }
            else
            {
                dal = new D_Organization_type();
            }
        }
		
		#region  Method
        public IList<Domain.Organization_type> GetList(List<SearchTemplate> st, List<Order> order) 
        {
            return dal.GetList(st,order);
        }

        public int GetCount(List<SearchTemplate> st) 
        {
            return dal.GetCount(st);
        }
        
        public int Save(Domain.Organization_type model) 
        {
            return Convert.ToInt32(dal.Save(model));
        }
        
        public Domain.Organization_type Get(object id) 
        {
            return dal.Get(id);
        }
        
        public void Delete(object id) 
        {
             dal.Delete(id);
        }
        
        public void Update(Domain.Organization_type model) 
        {
            dal.Update(model);
        }
		#endregion
   
	}
}