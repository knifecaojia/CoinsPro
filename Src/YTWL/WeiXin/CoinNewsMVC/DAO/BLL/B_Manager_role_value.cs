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
	public partial class B_Manager_role_value
	{
		private readonly IManager_role_value dal;
		public B_Manager_role_value()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_Manager_role_value();
            }
            else
            {
                dal = new D_Manager_role_value();
            }
        }
		
		#region  Method

        public Domain.Manager_role_value Get(int id) 
        {
            return dal.Get(id);
        }
        public IList<Domain.Manager_role_value> GetList(List<SearchTemplate> st,List<Order> order)
        {
            return dal.GetList(st, order);
        }

        public int GetCount(List<SearchTemplate> st)
        {
            return dal.GetCount(st);
        }

        public void Update(Domain.Manager_role_value model)
        {
            dal.Update(model);
        }

        public void Delete(int id) 
        {
            dal.Delete(id);
        }

        public IList<Domain.Manager_role_value> LoadAll() 
        {
            return dal.LoadAll();
        }
		#endregion


        #region 业务
        public void Update(string txt_action_type,int role_id) 
        {
            //添角色户权限
            var navs = txt_action_type.Split(new char[] { ';' });
            foreach (var nav in navs)
            {
                var role_value = nav.Split(new char[] { ':' });
                if (role_value.Length > 1)
                {
                    List<SearchTemplate> st = new List<SearchTemplate>()
                    {
                        new SearchTemplate(){key="role_id",value=role_id,searchType=Common.EnumBase.SearchType.Eq},
                        new SearchTemplate(){key="nav_id",value=Convert.ToInt32(role_value[0]),searchType=Common.EnumBase.SearchType.Eq}
                    };
                    var list = GetList(st,null);
                    Domain.Manager_role_value m_mrv = new Domain.Manager_role_value();
                    if (list.Count > 0)
                    {
                        m_mrv = list[0];
                        m_mrv.action_type = role_value[1];
                    }
                    else
                    {
                        m_mrv.action_type = role_value[1];
                        m_mrv.nav_id = Convert.ToInt32(role_value[0]);
                        m_mrv.role_id = role_id;
                    }
                    Update(m_mrv);
                }
            }
        }
        #endregion
    }
}