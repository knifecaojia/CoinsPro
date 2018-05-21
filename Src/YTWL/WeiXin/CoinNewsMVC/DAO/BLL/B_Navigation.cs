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
	public partial class B_Navigation
	{
		private readonly INavigation dal;
		public B_Navigation()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_Navigation();
            }
            else
            {
                dal = new D_Navigation();
            }
        }
		
		#region  Method

        public IList<Domain.Navigation> LoadAll() 
        {
            return dal.LoadAll();
        }
        public IList<Domain.Navigation> LoadAll(List<Order> order = null, int parent_id = 0,string is_lock = "√") 
        {
            return dal.LoadAll(order, parent_id, is_lock);
        }



        //public IList<Domain.Navigation> GetList(int pageIndex = 1, int pageSiez = 10, List<Order> order = null, int parent_id = -1,string link_url = "") 
        //{
        //    return dal.GetList(pageIndex, pageSiez, order, parent_id, link_url);
        //}

        public IList<Domain.Navigation> GetList(List<SearchTemplate> st, List<Order> order) 
        {
            return dal.GetList(st, order);
        }

        public int GetCount(List<SearchTemplate> st)
        {
            return dal.GetCount(st);
        }
        public int Save(Domain.Navigation model) 
        {
            return Convert.ToInt32(dal.Save(model));
        }

        public void Update(Domain.Navigation model) 
        {
             dal.Update(model);
        }

        public Domain.Navigation GetNav(int id) 
        {
            return dal.Get(id);
        }

        public void Delete(int id) 
        {
             dal.Delete(id);
        }
		#endregion
   
	}
}