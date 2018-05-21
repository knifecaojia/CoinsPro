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
	public partial class B_Manager
	{
		private readonly IManager dal;
		public B_Manager()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_Manager();
            }
            else
            {
                dal = new D_Manager();
            }
        }
		
		#region  Method

        public IList<Domain.Manager> GetList(List<SearchTemplate> st, List<Order> order) 
        {
            return dal.GetList(st, order);
        }
        public int GetCount(List<SearchTemplate> st) 
        {
            return dal.GetCount(st);
        }
        public int Save(Domain.Manager model) 
        {
            return Convert.ToInt32(dal.Save(model));
        }

        public Domain.Manager Get(object id) 
        {
            return dal.Get(id);
        }

        public void Update(Domain.Manager model) 
        {
             dal.Update(model);
        }
        public void Delete(int id) 
        {
             dal.Delete(id);
        }

        /// <summary>
        /// 获取管理员角色
        /// </summary>
        /// <returns></returns>
        public string[] GetRoles(string id)
        {
            B_Manager_role b_mr = new B_Manager_role();
            var model = Get(Convert.ToInt32(id));
            if (model == null) 
            {
                return new string[] {""};
            }
            return new string[] { model.manager_role.role_name };
        }

        public IList<Domain.Manager> LoadAll() 
        {
            return dal.LoadAll();
        }

		#endregion
   
	}
}