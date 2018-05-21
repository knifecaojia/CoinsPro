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
    public static class HtmlSelect
    {
        /// <summary>
        /// 后台导航下拉框
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static string select_nav(this HtmlHelper helper)
        {
            B_Navigation b_nav = new B_Navigation();
            //先查询出所有的一级菜单
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            var list_nav = b_nav.LoadAll(order, 0);
            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"txt_parent_id\" name=\"txt_parent_id\" class=\"selectpicker show-tick form-control\" data-live-search=\"true\">");
            sb.Append("<optgroup label=\"一级菜单\">");
            sb.Append("<option data-subtext=\"一级菜单\" selected = \"true\">0</option>");
            sb.Append("</optgroup>");
            foreach (var nav in list_nav)
            {
                sb.Append("<optgroup label=" + nav.title + ">");
                sb.Append("<option data-subtext=" + nav.title + ">" + nav.id + "</option>"); //一级菜单放第一个
                //查询二级菜单
                var list_sub_nav = b_nav.LoadAll(order, nav.id);
                foreach (var sub_nav in list_sub_nav)
                {
                    sb.Append("<option data-subtext=" + sub_nav.title + ">" + sub_nav.id + "</option>"); //二级菜单
                }
                sb.Append("</optgroup>");
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        public static string select_role(this HtmlHelper helper)
        {
            B_Manager_role b_role = new B_Manager_role();
            //先查询出所有的一级菜单
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            var list_role = b_role.LoadAll();
            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"txt_role_id\" name=\"txt_role_id\" class=\"selectpicker show-tick form-control\" data-live-search=\"true\">");
            sb.Append("<optgroup label=\"角色列表\">");
            foreach (var role in list_role)
            {
                sb.Append("<option data-subtext=" + role.role_name + ">" + role.id + "</option>");
            }
            sb.Append("</optgroup>");
            sb.Append("</select>");
            return sb.ToString();
        }

        public static string select_auth(this HtmlHelper helper, string manager_id, string controllerName)
        {
            B_Navigation b_nav = new B_Navigation();
            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="controllerName",value=controllerName,searchType=Common.EnumBase.SearchType.Eq}
            };
            var list_nav = b_nav.GetList(st, null);
            int nav_id = 0;
            if (list_nav.Count > 0)
            {
                nav_id = list_nav[0].id;
            }
            B_Manager_role_value b_mrv = new B_Manager_role_value();
            B_Manager b_manager = new B_Manager();
            var m_manager = b_manager.Get(Convert.ToInt32(manager_id));
            st = new List<SearchTemplate>()
                    {
                        new SearchTemplate(){key="role_id",value=m_manager.manager_role.id,searchType=Common.EnumBase.SearchType.Eq},
                        new SearchTemplate(){key="nav_id",value=nav_id,searchType=Common.EnumBase.SearchType.Eq}
                    };
            var list_mrv = b_mrv.GetList(st, null);
            if (list_mrv.Count == 0) return "";
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id=\"toolbar\" class=\"btn-group\">");
            if (list_mrv[0].action_type.Contains(EnumBase.Authorize.添加.Description()))
            {
                sb.Append("<button id=\"btn_add\" type=\"button\" class=\"btn btn-blue\">");
                sb.Append("<span class=\"glyphicon glyphicon-plus\" aria-hidden=\"true\"></span>" + EnumBase.Authorize.添加.Description());
                sb.Append("</button>");
            }
            if (list_mrv[0].action_type.Contains(EnumBase.Authorize.修改.Description()))
            {
                sb.Append("<button id=\"btn_edit\" type=\"button\" class=\"btn btn-warning\">");
                sb.Append("<span class=\"glyphicon glyphicon-pencil\" aria-hidden=\"true\"></span>" + EnumBase.Authorize.修改.Description());
                sb.Append("</button>");
            }
            if (list_mrv[0].action_type.Contains(EnumBase.Authorize.删除.Description()))
            {
                sb.Append("<button id=\"btn_delete\" type=\"button\" class=\"btn btn-danger\">");
                sb.Append("<span class=\"glyphicon glyphicon-remove\" aria-hidden=\"true\"></span>" + EnumBase.Authorize.删除.Description());
                sb.Append("</button>");
            }
            if (list_mrv[0].action_type.Contains(EnumBase.Authorize.审核.Description()))
            {
                sb.Append("<button id=\"btn_exam\" type=\"button\" class=\"btn btn-info\">");
                sb.Append("<span class=\"glyphicon glyphicon-check\" aria-hidden=\"true\"></span>" + EnumBase.Authorize.审核.Description());
                sb.Append("</button>");
            }
            if (list_mrv[0].action_type.Contains(EnumBase.Authorize.下载.Description()))
            {
                sb.Append("<button id=\"btn_download\" type=\"button\" class=\"btn btn-success\">");
                sb.Append("<span class=\"fa fa-cloud-download\" aria-hidden=\"true\"></span>" + EnumBase.Authorize.下载.Description());
                sb.Append("</button>");
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        public static string selet_backup(this HtmlHelper helper)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"txt_backupType\" name=\"txt_backupType\" class=\"selectpicker show-tick form-control\" data-live-search=\"true\">");


            sb.Append("<optgroup label=\"备份类型\">");

            sb.Append("<option data-subtext=" + EnumBase.BackupType.完整备份.Description() + ">" + Convert.ToInt32(EnumBase.BackupType.完整备份) + "</option>");
            sb.Append("<option data-subtext=" + EnumBase.BackupType.差异备份.Description() + ">" + Convert.ToInt32(EnumBase.BackupType.差异备份) + "</option>");
            sb.Append("</optgroup>");
            sb.Append("</select>");
            return sb.ToString();
        }


        public static string select_new_type(this HtmlHelper helper)
        {
            B_News_type b_nt = new B_News_type();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            List<SearchTemplate> st = new List<SearchTemplate>() 
            {
                new SearchTemplate(){key="parent_id",value=0,searchType=Common.EnumBase.SearchType.Eq}
            };
            var list_nt = b_nt.GetList(st, order);

            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"txt_parent_id\" name=\"txt_parent_id\" class=\"selectpicker show-tick form-control\" data-live-search=\"true\">");
            sb.Append("<optgroup label=\"一级菜单\">");
            sb.Append("<option data-subtext=\"一级菜单\" selected = \"true\">0</option>");
            sb.Append("</optgroup>");
            foreach (var item in list_nt)
            {
                sb.Append("<optgroup label=" + item.title + ">");
                sb.Append("<option data-subtext=" + item.title + ">" + item.id + "</option>"); //一级菜单放第一个
                st = new List<SearchTemplate>() 
                {
                    new SearchTemplate(){key="parent_id",value=item.id,searchType=Common.EnumBase.SearchType.Eq}
                };
                //var list_subnt = b_nt.GetList(st, order);
                //foreach (var sub in list_subnt)
                //{
                //    sb.Append("<option data-subtext=" + sub.title + ">" + sub.id + "</option>"); //二级菜单
                //}
                sb.Append("</optgroup>");
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        public static string select_new(this HtmlHelper helper)
        {
            B_News_type b_nt = new B_News_type();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            List<SearchTemplate> st = new List<SearchTemplate>() 
            {
                new SearchTemplate(){key="parent_id",value=0,searchType=Common.EnumBase.SearchType.Eq}
            };
            var list_nt = b_nt.GetList(st, order);

            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"txt_parent_id\" name=\"txt_parent_id\" class=\"selectpicker show-tick form-control\" data-live-search=\"true\">");
            foreach (var item in list_nt)
            {
                sb.Append("<optgroup label=" + item.title + ">");
                sb.Append("<option data-subtext=" + item.title + ">" + item.id + "</option>"); //一级菜单放第一个
                st = new List<SearchTemplate>() 
                {
                    new SearchTemplate(){key="parent_id",value=item.id,searchType=Common.EnumBase.SearchType.Eq}
                };
                var list_subnt = b_nt.GetList(st, order);
                foreach (var sub in list_subnt)
                {
                    sb.Append("<option data-subtext=" + sub.title + ">" + sub.id + "</option>"); //二级菜单
                }
                sb.Append("</optgroup>");
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        /// <summary>
        /// 这里只遍历两层
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static string select_org(this HtmlHelper helper)
        {
            B_Organization b_org = new B_Organization();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            List<SearchTemplate> st = new List<SearchTemplate>() 
            {
                new SearchTemplate(){key="parent_id",value=0,searchType=Common.EnumBase.SearchType.Eq}
            };
            IList<Domain.Organization> list_org = b_org.GetList(st, order);
            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"txt_parent_id\" name=\"txt_parent_id\" class=\"selectpicker show-tick form-control\" data-live-search=\"true\">");
            sb.Append("<optgroup label=\"一级菜单\">");
            sb.Append("<option data-subtext=\"一级菜单\" selected = \"true\">0</option>");
            sb.Append("</optgroup>");
            foreach (var item in list_org)
            {
                sb.Append("<optgroup label=" + item.name + ">");
                sb.Append("<option data-subtext=" + item.name + ">" + item.id + "</option>"); //一级菜单放第一个
                st = new List<SearchTemplate>() 
                {
                    new SearchTemplate(){key="parent_id",value=item.id,searchType=Common.EnumBase.SearchType.Eq}
                };
                var list_suborg = b_org.GetList(st, order);
                foreach (var it in list_suborg)
                {
                    sb.Append("<option data-subtext=" + it.name + ">" + it.id + "</option>"); //二级菜单
                }
                sb.Append("</optgroup>");
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        public static string select_org_type(this HtmlHelper helper)
        {
            B_Organization_type b_orgtype = new B_Organization_type();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            var list_orgtype = b_orgtype.GetList(null, order);
            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"txt_orgtype_id\" name=\"txt_orgtype_id\" class=\"selectpicker show-tick form-control\" data-live-search=\"true\">");
            for (int i = 0; i < list_orgtype.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append("<option selected = \"selected\" data-subtext=" + list_orgtype[i].orgtype_name + ">" + list_orgtype[i].id + "</option>");
                }
                else
                {
                    sb.Append("<option data-subtext=" + list_orgtype[i].orgtype_name + ">" + list_orgtype[i].id + "</option>");
                }
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        public static string select_wiki(this HtmlHelper helper)
        {
            B_Wiki b_wiki = new B_Wiki();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            List<SearchTemplate> st = new List<SearchTemplate>() 
            {
                new SearchTemplate(){key="parent_id",value=0,searchType=Common.EnumBase.SearchType.Eq}
            };
            var list_wiki = b_wiki.GetList(st, order);
            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"txt_parent_id\" name=\"txt_parent_id\" class=\"selectpicker show-tick form-control\" data-live-search=\"true\">");
            sb.Append("<option selected = \"selected\" data-subtext=一级栏目>" + 0 + "</option>");
            for (int i = 0; i < list_wiki.Count; i++)
            {

                sb.Append("<option data-subtext=" + list_wiki[i].title + ">" + list_wiki[i].id + "</option>");

            }
            sb.Append("</select>");
            return sb.ToString();
        }
    }
}