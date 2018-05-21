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
	public partial class B_News
	{
		private readonly INews dal;
		public B_News()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_News();
            }
            else
            {
                dal = new D_News();
            }
        }
		
		#region  Method
        public IList<Domain.News> GetList(List<SearchTemplate> st, List<Order> order)
        {
            return dal.GetList(st, order);
        }

        public int GetCount(List<SearchTemplate> st)
        {
            return dal.GetCount(st);
        }

        public int Save(Domain.News model)
        {
            return Convert.ToInt32(dal.Save(model));
        }

        public Domain.News Get(object id)
        {
            return dal.Get(id);
        }

        public void Delete(object id)
        {
            dal.Delete(id);
        }

        public void Update(Domain.News model)
        {
            dal.Update(model);
        }
		#endregion
   
	}
}