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
	public partial class B_Wiki
	{
		private readonly IWiki dal;
		public B_Wiki()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_Wiki();
            }
            else
            {
                dal = new D_Wiki();
            }
        }
		
		#region  Method
        public IList<Domain.Wiki> GetList(List<SearchTemplate> st, List<Order> order) 
        {
            return dal.GetList(st,order);
        }

        public int GetCount(List<SearchTemplate> st) 
        {
            return dal.GetCount(st);
        }
        
        public int Save(Domain.Wiki model) 
        {
            return Convert.ToInt32(dal.Save(model));
        }
        
        public Domain.Wiki Get(object id) 
        {
            return dal.Get(id);
        }
        
        public void Delete(object id) 
        {
             dal.Delete(id);
        }
        
        public void Update(Domain.Wiki model) 
        {
            dal.Update(model);
        }
		#endregion
   
	}
}