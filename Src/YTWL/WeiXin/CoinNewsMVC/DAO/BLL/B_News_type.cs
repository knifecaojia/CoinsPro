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
	public partial class B_News_type
	{
		private readonly INews_type dal;
		public B_News_type()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_News_type();
            }
            else
            {
                dal = new D_News_type();
            }
        }
		
		#region  Method
        public IList<Domain.News_type> GetList(List<SearchTemplate> st, List<Order> order) 
        {
            return dal.GetList(st,order);
        }

        public int GetCount(List<SearchTemplate> st) 
        {
            return dal.GetCount(st);
        }

        public int Save(Domain.News_type model) 
        {
            return Convert.ToInt32(dal.Save(model));
        }

        public Domain.News_type Get(object id) 
        {
            return dal.Get(id);
        }

        public void Delete(object id) 
        {
             dal.Delete(id);
        }

        public void Update(Domain.News_type model) 
        {
            dal.Update(model);
        }
		#endregion
   
	}
}