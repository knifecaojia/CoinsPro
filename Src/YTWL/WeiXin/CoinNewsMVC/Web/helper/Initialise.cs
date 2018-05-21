using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.helper
{
    public class Initialise
    {

        /// <summary>
        /// 初始化权限
        /// </summary>
        public void Initializing() 
        {
            DAO.BLL.B_Navigation b_nav = new DAO.BLL.B_Navigation();
            DAO.BLL.B_Manager_role b_mr = new DAO.BLL.B_Manager_role();
            DAO.BLL.B_Manager_role_value b_mrv = new DAO.BLL.B_Manager_role_value();
            DAO.BLL.B_Manager b_manager = new DAO.BLL.B_Manager();
            DAO.BLL.B_Manager_log b_log = new DAO.BLL.B_Manager_log();


            var list_log = b_log.LoadAll();
            foreach (var item in list_log)
            {
                b_log.Delete(item.id);
            }

            //1.删除所有管理员
            var list_manager = b_manager.LoadAll();
            foreach (var item in list_manager)
            {
                b_manager.Delete(item.id);
            }
            //2.删除所有角色权限
            var list_mrv = b_mrv.LoadAll();
            foreach (var item in list_mrv)
            {
                b_mrv.Delete(item.id);
            }

            //3.删除所有的角色
            var list_mr = b_mr.LoadAll();
            foreach (var item in list_mr)
            {
                b_mr.Delete(item.id);
            }

            //4.删除所有的权限
            var list_nva = b_nav.LoadAll();
            foreach (var item in list_nva)
            {
                b_nav.Delete(item.id);
            }
            //5.添加权限
            #region 权限
            Domain.Navigation model = new Domain.Navigation();
            model.icon_url = "<i class=\"fa fa-home\"></i>";
            model.title = "主页";
            model.link_url = "#";
            model.sort_id = 1;
            model.is_lock = "√";
            model.parent_id = 0;
            model.action_type = "查看";
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            var res = b_nav.Save(model);

            model = new Domain.Navigation();
            model.icon_url = "<i class=\"fa fa-user\"></i>";
            model.title = "会员列表";
            model.link_url = "user";
            model.sort_id = 1;
            model.is_lock = "√";
            model.parent_id = res;
            model.action_type = "查看,添加,修改,删除,审核";
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            b_nav.Save(model);


            //常用示例
            model = new Domain.Navigation();
            model.icon_url = "<i class=\"fa fa-tags\"></i>";
            model.title = "常用示例";
            model.link_url = "#";
            model.sort_id = 1;
            model.is_lock = "√";
            model.parent_id = 0;
            model.action_type = "查看";
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            res = b_nav.Save(model);

            model = new Domain.Navigation();
            model.icon_url = "<i class=\"fa fa-envelope\"></i>";
            model.title = "短信工具";
            model.link_url = "send_sms";
            model.sort_id = 1;
            model.is_lock = "√";
            model.parent_id = res;
            model.action_type = "查看,添加,删除";
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            b_nav.Save(model);


            //系统安全
            model = new Domain.Navigation();
            model.icon_url = "<i class=\"fa fa-desktop\"></i>";
            model.title = "系统安全";
            model.link_url = "#";
            model.sort_id = 1;
            model.is_lock = "√";
            model.parent_id = 0;
            model.action_type = "查看";
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            res = b_nav.Save(model);

            model = new Domain.Navigation();
            model.icon_url = "<i class=\"fa fa-star-o\"></i>";
            model.title = "数据备份";
            model.link_url = "backups";
            model.sort_id = 1;
            model.is_lock = "√";
            model.parent_id = res;
            model.action_type = "查看,添加,删除,下载";
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            b_nav.Save(model);

            model = new Domain.Navigation();
            model.icon_url = "<i class=\"fa fa-book\"></i>";
            model.title = "操作日志";
            model.link_url = "manager_log";
            model.sort_id = 4;
            model.is_lock = "√";
            model.parent_id = res;
            model.action_type = "查看,删除";
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            b_nav.Save(model);
            //-----------------------------------------






            model = new Domain.Navigation();
            model.icon_url = "<i class=\"fa fa-cogs\"></i>";
            model.title = "系统管理";
            model.link_url = "#";
            model.sort_id = 99;
            model.is_lock = "√";
            model.parent_id = 0;
            model.action_type = "查看";
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            res = b_nav.Save(model);


            model = new Domain.Navigation();
            model.icon_url = "<i class=\"fa fa-cog\"></i>";
            model.title = "权限列表";
            model.link_url = "navigation";
            model.sort_id = 1;
            model.is_lock = "√";
            model.parent_id = res;
            model.action_type = "查看,添加,修改,删除";
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            b_nav.Save(model);


            model = new Domain.Navigation();
            model.icon_url = "<i class=\"fa fa-group\"></i>";
            model.title = "角色列表";
            model.link_url = "manager_role";
            model.sort_id = 2;
            model.is_lock = "√";
            model.parent_id = res;
            model.action_type = "查看,添加,修改,删除,审核";
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            b_nav.Save(model);

            model = new Domain.Navigation();
            model.icon_url = "<i class=\"fa fa-user-secret\"></i>";
            model.title = "用户列表";
            model.link_url = "manager";
            model.sort_id = 3;
            model.is_lock = "√";
            model.parent_id = res;
            model.action_type = "查看,添加,修改,删除,审核";
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            b_nav.Save(model);

           

          

           
            #endregion

            //6.添加角色和角色权限
            Domain.Manager_role m_mr = new Domain.Manager_role();
            m_mr.role_name = "管理员";
            m_mr.is_sys = 1;
            res = b_mr.Save(m_mr);
            list_nva = b_nav.LoadAll();
            foreach (var item in list_nva)
            {
                Domain.Manager_role_value m_mrv = new Domain.Manager_role_value();
                m_mrv.role_id = res;
                m_mrv.nav_id = item.id;
                m_mrv.action_type = item.action_type;
                b_mrv.Update(m_mrv);
            }

            //添加管理员
            Domain.Manager m_manager = new Domain.Manager();
            m_manager.user_name = "admin";
            m_manager.real_name = "提伯斯";
            m_manager.mobile = "15019400599";
            m_manager.email = "505613913@qq.com";
            m_manager.password = Common.Encrypt.md5("123456");
            m_manager.is_lock = "√";
            m_manager.add_time = DateTime.Now;
            m_manager.manager_role = m_mr;
            b_manager.Save(m_manager);
        }
    }
}