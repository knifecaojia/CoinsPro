using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAO.BLL;
using System.Text;
using NHibernate.Criterion;
using Common;
using Domain;

namespace Web.helper
{
    /// <summary>
    /// 左侧菜单
    /// </summary>
    public static class HtmlMenu
    {
        public static string Menu(this HtmlHelper helper,int role_id)
        {
            B_Navigation b_nav = new B_Navigation();
            //先查询出所有的一级菜单
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            var list_nav = b_nav.LoadAll(order, 0);
            StringBuilder sb = new StringBuilder();
            B_Manager_role_value b_mrv = new B_Manager_role_value();
            
            foreach (var nav in list_nav)
            {
                List<SearchTemplate> st = new List<SearchTemplate>()
                    {
                        new SearchTemplate(){key="role_id",value=role_id,searchType=Common.EnumBase.SearchType.Eq},
                        new SearchTemplate(){key="nav_id",value=nav.id,searchType=Common.EnumBase.SearchType.Eq}
                    };
                IList<Domain.Manager_role_value> list = b_mrv.GetList(st,null);
                if (list.Count == 0) continue;
                if (!list[0].action_type.Contains("查看")) continue;
                sb.Append("<li>");
                sb.Append("<a href=\"" + nav.link_url + "\">");
                sb.Append(nav.icon_url);
                sb.Append("<span class=\"nav-label\">" + nav.title + "</span>");
                sb.Append("<span class=\"fa arrow\"></span>");
                sb.Append("</a>");
                sb.Append("<ul class=\"nav nav-second-level\">");
                //查询二级菜单
                st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="parent_id",value=nav.id,searchType=Common.EnumBase.SearchType.Eq}
                };
                var list_sub_nav = b_nav.GetList(st,order);
                foreach (var sub_nav in list_sub_nav)
                {
                    st = new List<SearchTemplate>()
                    {
                        new SearchTemplate(){key="role_id",value=role_id,searchType=Common.EnumBase.SearchType.Eq},
                        new SearchTemplate(){key="nav_id",value=sub_nav.id,searchType=Common.EnumBase.SearchType.Eq}
                    };
                    list = b_mrv.GetList(st,null);
                    if (list.Count == 0) continue;
                    if (!list[0].action_type.Contains("查看")) continue;
                    sb.Append("<li>");
                    sb.Append("<a class=\"J_menuItem\" href=" + sub_nav.link_url + " data-index=" + sub_nav.sort_id + ">" + sub_nav.icon_url + "</i>" + sub_nav.title + "</a>");
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
                sb.Append("</li>");

            }
            return sb.ToString();
        }
    }
}