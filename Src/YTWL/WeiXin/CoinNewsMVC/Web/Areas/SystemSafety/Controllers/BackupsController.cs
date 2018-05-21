using Common;
using DAO.BLL;
using Domain;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.helper;

namespace Web.Areas.SystemSafety.Controllers
{
    public class BackupsController : BaseController
    {
        //
        // GET: /SystemSafety/Backups/

        public ActionResult Index()
        {
            B_Backups b_backup = new B_Backups();
            string dbName = b_backup.GetDbName();
            ViewData["dbName"] = dbName;
            return View();
        }

        #region Backups


        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetBackupsList(int limit = 10, int offset = 1, string fileName = "", DateTime? start_time = null, DateTime? end_time = null)
        {
            B_Backups b_backup = new B_Backups();
            List<Order> order = new List<Order>() { Order.Desc("id") };
            List<SearchTemplate> st = new List<SearchTemplate>() 
            {
                new SearchTemplate(){key="fileName",value=fileName,searchType=EnumBase.SearchType.Eq},
                new SearchTemplate(){key="start_time",value=fileName,searchType=EnumBase.SearchType.Ge},
                new SearchTemplate(){key="end_time",value=fileName,searchType=EnumBase.SearchType.Le},
                new SearchTemplate(){key="",value=new int[]{offset,limit},searchType=EnumBase.SearchType.Paging}
            };
            var list_manager = b_backup.GetList(st, order);
            var total = b_backup.GetCount(st);
            return this.MyJson(new { total = total, rows = list_manager }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult AddBackups(string txt_dbName, string txt_fileName, string txt_backupType, string txt_remark)
        {
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            Common.Json json = new Common.Json();
            B_Backups b_backup = new B_Backups();
            B_Manager b_manager = new B_Manager();
            txt_fileName += ".bak";
            var filePath = Server.MapPath("~/Resource/DbBackup/" + txt_fileName);
            //把文件备份成功
            b_backup.BackupDB(txt_dbName, filePath, Convert.ToInt32(txt_backupType));
            Domain.Backups m_backup = new Domain.Backups();
            m_backup.dbName = txt_dbName;
            m_backup.fileName = txt_fileName;
            m_backup.filePath = "/Resource/DbBackup/" + txt_fileName;
            if (Convert.ToInt32(txt_backupType) == Convert.ToInt32(EnumBase.BackupType.完整备份))
            {
                m_backup.backupType = EnumBase.BackupType.完整备份.Description();
            }
            else if (Convert.ToInt32(txt_backupType) == Convert.ToInt32(EnumBase.BackupType.差异备份))
            {
                m_backup.backupType = EnumBase.BackupType.差异备份.Description();
            }
            else
            {
                m_backup.backupType = EnumBase.BackupType.完整备份.Description();
            }
            m_backup.fileSize = FileHelper.ToFileSize(FileHelper.GetFileSize(filePath));
            m_backup.addTime = DateTime.Now;
            m_backup.addManager = b_manager.Get(Convert.ToInt32(base.User.Identity.Name));
            m_backup.remark = txt_remark;
            m_backup.delManager = new Domain.Manager();//这里给个空对象,否则外键关联会出错的
            var res = b_backup.Save(m_backup);
            if (res > 0)
            {
                json.msg = "备份成功!";
            }
            else
            {
                json.msg = "备份失败!";
                json.status = -1;
            }
            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult DelBackups(string ids)
        {
            Common.Json json = new Common.Json();
            B_Backups b_backup = new B_Backups();
            B_Manager b_mananger = new B_Manager();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                var model = b_backup.Get(Convert.ToInt32(id));
                string filepath = Server.MapPath(model.filePath);
                if (FileDownHelper.FileExists(filepath))
                {
                    FileHelper.DeleteFile(filepath);
                }
                model.delManager = b_mananger.Get(Convert.ToInt32(base.User.Identity.Name));
                model.delTime = DateTime.Now;
                b_backup.Update(model);
            }
            json.msg = "成功删除" + ids.Split(new char[] { ',' }).Length + "个备份!";
            return Json(json);
        }


        [HttpPost]
        [AuthorizeFilter]
        public void DownLoadBackups(string id)
        {
            B_Backups b_backup = new B_Backups();

            var data = b_backup.Get(Convert.ToInt32(id));
            string filename = Server.UrlDecode(data.fileName);
            string filepath = Server.MapPath(data.filePath);
            if (FileDownHelper.FileExists(filepath))
            {
                FileDownHelper.DownLoadold(filepath, filename);
            }
        }
        #endregion
    }
}
