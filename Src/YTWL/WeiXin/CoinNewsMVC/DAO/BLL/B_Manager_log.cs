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
	public partial class B_Manager_log
	{
		private readonly IManager_log dal;
		public B_Manager_log()
		{
            if (CacheRedis.RedisHelper.Ping())
            {
                dal = new C_Manager_log();
            }
            else
            {
                dal = new D_Manager_log();
            }
        }
		
		#region  Method
        public int Save(Domain.Manager_log model) 
        {
            return Convert.ToInt32(dal.Save(model));
        }

        public int Add(int manager_id, string actionType, string controllerName, string remark, string user_ip)
        {
            DAO.BLL.B_Manager b_manager = new DAO.BLL.B_Manager();
            DAO.BLL.B_Navigation b_nav = new DAO.BLL.B_Navigation();
            Domain.Manager_log model = new Domain.Manager_log();
            model.user_id =manager_id;
            var m_manager = b_manager.Get(manager_id);
            model.user_name = m_manager.user_name;
            model.action_type = actionType;

            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="controllerName",value=controllerName,searchType=Common.EnumBase.SearchType.Eq}
            };
            model.navigation = b_nav.GetList(st,null)[0];
            model.remark = remark;
            model.user_ip = user_ip;
            model.add_time = DateTime.Now;
            var res = Save(model);
            return res;
        }

        public IList<Domain.Manager_log> GetList(List<SearchTemplate> st, List<Order> order) 
        {
            return dal.GetList(st,order);
        }

        public int GetCount(List<SearchTemplate> st) 
        {
            return dal.GetCount(st);
        }

        public void Delete(int id) 
        {
             dal.Delete(id);
        }

        public IList<Domain.Manager_log> LoadAll() 
        {
           return dal.LoadAll();
        }
		#endregion

        #region Work
        public Dictionary<string, object[]> GetNavClick(int numDay) 
        {
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            Dictionary<string, object[]> dic_nav = new Dictionary<string, object[]>();

            //模块最近7天的访问量
            B_Navigation b_nav = new B_Navigation();
            List<SearchTemplate> st = new List<SearchTemplate>() 
            {
                new SearchTemplate(){key="parent_id",value=0,searchType=EnumBase.SearchType.Gt}
            };
            IList<Domain.Navigation> list_nav = b_nav.GetList(st, null);
            foreach (var nav in list_nav)
            {
                object[] count = new object[numDay];
                for (int i = 0; i < numDay; i++)
                {

                    st = new List<SearchTemplate>();
                    if (i == numDay - 1)
                    {
                        st.Add(new SearchTemplate() { key = "add_time", value = DateTime.Now.AddDays(i - numDay + 1), searchType = EnumBase.SearchType.Le });
                        st.Add(new SearchTemplate() { key = "add_time", value = DateTime.Parse(DateTime.Now.AddDays(i - numDay + 1).ToString("yyyy-MM-dd")), searchType = EnumBase.SearchType.Ge });
                    }
                    else
                    {
                        st.Add(new SearchTemplate() { key = "add_time", value = DateTime.Parse(DateTime.Now.AddDays(i - numDay + 2).ToString("yyyy-MM-dd")), searchType = EnumBase.SearchType.Le });
                        st.Add(new SearchTemplate() { key = "add_time", value = DateTime.Parse(DateTime.Now.AddDays(i - numDay + 1).ToString("yyyy-MM-dd")), searchType = EnumBase.SearchType.Ge });
                    }
                    st.Add(new SearchTemplate() { key = "navigation", value = nav, searchType = EnumBase.SearchType.Eq });


                    count[i] = GetCount(st);
                }
                dic_nav.Add(nav.title, count);
            }
            return dic_nav;
        }
        #endregion
    }
}