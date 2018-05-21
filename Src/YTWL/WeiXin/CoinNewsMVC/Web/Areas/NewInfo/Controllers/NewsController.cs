using Common;
using DAO.BLL;
using Domain;
using NHibernate.Criterion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Web.helper;

namespace Web.Areas.NewInfo.Controllers
{
    public class NewsController : BaseController
    {
        


        // GET: /NewInfo/News/
        [AuthorizeFilter]
        public ActionResult Index()
        {
            string imagespath = System.Configuration.ConfigurationManager.AppSettings["imagespath"];
            ViewData["imagespath"] = imagespath;
            return View();
        }



        #region News
        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetNewsList(int limit = 10, int offset = 1)
        {
            B_News b_new = new B_News();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            List<SearchTemplate> st = new List<SearchTemplate>() 
            {
                new SearchTemplate(){key="",value=new int[]{offset,limit},searchType=EnumBase.SearchType.Paging}
            };
            var list_new = b_new.GetList(st, order);
            var list_new_count = b_new.GetCount(st);
            return Json(new { total = list_new_count, rows = list_new }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        [AuthorizeFilter]
        public JsonResult AddNews(int txt_parent_id, string txt_is_lock, string txt_action_type, string txt_title, int? txt_sort_id, int? txt_click, DateTime? start_time, string txt_source, string txt_author, string txt_summary, string txtContent, string txt_seo_title, string txt_seo_keywords, string txt_seo_description)
        {
            Common.Json json = new Common.Json();
            var fileName = "";
            var name = "";
            if (Request.Files.Count == 0)
            {
                //json.msg = "没有文件！";
                //json.status = -1;
                //return Json(json);
            }
            else
            {
                HttpPostedFileBase file = Request.Files[0];
                name = "news/" + DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
                fileName = Path.Combine(Request.MapPath("/") + "../Images", name);
                try
                {
                    file.SaveAs(fileName);
                }
                catch
                {
                    //json.msg = "上传异常！";
                    //json.status = -1;
                    //return Json(json);
                }
            }

            DAO.BLL.B_News b_new = new DAO.BLL.B_News();
            DAO.BLL.B_News_type b_nt = new B_News_type();
            DAO.BLL.B_Manager b_manager = new B_Manager();
            Domain.News m_new = new Domain.News();
            m_new.news_type = b_nt.Get(txt_parent_id);
            m_new.is_lock = txt_is_lock;
            if (!string.IsNullOrEmpty(txt_action_type))
            {
                if (txt_action_type.Contains(EnumBase.RecommendType.置顶.Description()))
                {
                    m_new.is_top = 1;
                }
                if (txt_action_type.Contains(EnumBase.RecommendType.热门.Description()))
                {
                    m_new.is_hot = 1;
                }
                if (txt_action_type.Contains(EnumBase.RecommendType.推荐.Description()))
                {
                    m_new.is_red = 1;
                }
                if (txt_action_type.Contains(EnumBase.RecommendType.允许评论.Description()))
                {
                    m_new.is_msg = 1;
                }
            }
            m_new.title = txt_title;
            m_new.img_url = name;
            m_new.sort_id = txt_sort_id;
            m_new.source = txt_source;
            m_new.author = txt_author;
            m_new.summary = txt_summary;
            m_new.content = txtContent;
            m_new.start_time = start_time;
            m_new.add_time = DateTime.Now;
            m_new.click = txt_click;
            m_new.manager = b_manager.Get(Convert.ToInt32(base.User.Identity.Name));
            m_new.seo_title = txt_seo_title;
            m_new.seo_keywords = txt_seo_keywords;
            m_new.seo_description = txt_seo_description;
            var res = b_new.Save(m_new);
            if (res > 0)
            {
                json.msg = "添加成功!";
            }
            else
            {
                json.msg = "添加失败!";
                json.status = -1;
            }
            return Json(json);
        }


        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetNews(int id)
        {
            B_News b_new = new B_News();
            var res = b_new.Get(id);
            return Json(res);
        }

        [HttpPost]
        [ValidateInput(false)]
        [AuthorizeFilter]
        public JsonResult EditNews(int id, int txt_parent_id, string txt_is_lock, string txt_action_type, string txt_title, int? txt_sort_id, int? txt_click, DateTime? start_time, string txt_source, string txt_author, string txt_summary, string txtContent, string txt_seo_title, string txt_seo_keywords, string txt_seo_description, string txt_img_url)
        {
            Common.Json json = new Common.Json();
            DAO.BLL.B_News b_new = new DAO.BLL.B_News();
            DAO.BLL.B_News_type b_nt = new B_News_type();
            DAO.BLL.B_Manager b_manager = new B_Manager();
            Domain.News m_new = b_new.Get(id);
            string pastImg = "";
            if (txt_img_url != m_new.img_url)//如果原图不等于现在的图片
            {
                var fileName = "";
                var name = "";
                if (Request.Files.Count == 0)
                {
                    //json.msg = "没有文件！";
                    //json.status = -1;
                    //return Json(json);
                }
                else
                {
                    pastImg = Path.Combine(Request.MapPath("/") + "../Images", m_new.img_url);//旧文件的物理路径
                    HttpPostedFileBase file = Request.Files[0];
                    name = "news/" + DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
                    fileName = Path.Combine(Request.MapPath("/") + "../Images", name);
                    try
                    {
                        file.SaveAs(fileName);
                        m_new.img_url = name;
                    }
                    catch
                    {
                        //json.msg = "上传异常！";
                        //json.status = -1;
                        //return Json(json);
                    }
                }
            }
            m_new.news_type = b_nt.Get(txt_parent_id);
            m_new.is_lock = txt_is_lock;
            if (!string.IsNullOrEmpty(txt_action_type))
            {
                m_new.is_top = 0;
                m_new.is_hot = 0;
                m_new.is_red = 0;
                m_new.is_msg = 0;
                if (txt_action_type.Contains(EnumBase.RecommendType.置顶.Description()))
                {
                    m_new.is_top = 1;
                }
                if (txt_action_type.Contains(EnumBase.RecommendType.热门.Description()))
                {
                    m_new.is_hot = 1;
                }
                if (txt_action_type.Contains(EnumBase.RecommendType.推荐.Description()))
                {
                    m_new.is_red = 1;
                }
                if (txt_action_type.Contains(EnumBase.RecommendType.允许评论.Description()))
                {
                    m_new.is_msg = 1;
                }
            }
            m_new.title = txt_title;
            m_new.sort_id = txt_sort_id;
            m_new.source = txt_source;
            m_new.author = txt_author;
            m_new.summary = txt_summary;
            m_new.content = txtContent;
            m_new.start_time = start_time;
            m_new.update_time = DateTime.Now;
            m_new.click = txt_click;
            m_new.manager = b_manager.Get(Convert.ToInt32(base.User.Identity.Name));
            m_new.seo_title = txt_seo_title;
            m_new.seo_keywords = txt_seo_keywords;
            m_new.seo_description = txt_seo_description;
            b_new.Update(m_new);
            json.msg = "修改成功!";
            Common.FileHelper.DeleteFile(pastImg);
            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult DelNews(string ids)
        {
            Common.Json json = new Common.Json();
            B_News b_new = new B_News();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                var m_new = b_new.Get(Convert.ToInt32(id));
                b_new.Delete(Convert.ToInt32(id));
                string pastImg = Path.Combine(Request.MapPath("/") + "../Images", m_new.img_url);//旧文件的物理路径
            }
            json.msg = "成功删除" + ids.Split(new char[] { ',' }).Length + "条记录!";
            return Json(json);
        }
        [HttpPost]
        [AuthorizeFilter]
        public JsonResult ExamNews(string ids, int? txt_status)
        {
            Common.Json json = new Common.Json();
            B_News b_new = new B_News();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                var m_new = b_new.Get(Convert.ToInt32(id));
                if (m_new.status != txt_status)
                {
                    m_new.status = txt_status;
                    b_new.Update(m_new);
                }
            }
            json.msg = "成功审核" + ids.Split(new char[] { ',' }).Length + "条记录!";
            return Json(json);
        }
        #endregion

        #region kindeditor富文本上传图片,预览图片

        [HttpPost]
        public JsonResult UploadImage()
        {
            string imagespath = System.Configuration.ConfigurationManager.AppSettings["imagespath"];
            string savePath = "/UploadImages/";
            string saveUrl = "/UploadImages/";
            string fileTypes = "gif,jpg,jpeg,png,bmp";
            int maxSize = 1000000;

            Hashtable hash = new Hashtable();

            HttpPostedFileBase file = Request.Files["imgFile"];
            if (file == null)
            {
                hash = new Hashtable();
                hash["error"] = 1;
                hash["message"] = "请选择文件";
                return Json(hash, "text/html;charset=UTF-8");
            }

            string dirPath = Server.MapPath("/") + "../Images/" + savePath;
            if (!Directory.Exists(dirPath))
            {
                hash = new Hashtable();
                hash["error"] = 1;
                hash["message"] = "上传目录不存在";
                return Json(hash, "text/html;charset=UTF-8");
            }

            string fileName = file.FileName;
            string fileExt = Path.GetExtension(fileName).ToLower();

            ArrayList fileTypeList = ArrayList.Adapter(fileTypes.Split(','));

            if (file.InputStream == null || file.InputStream.Length > maxSize)
            {
                hash = new Hashtable();
                hash["error"] = 1;
                hash["message"] = "上传文件大小超过限制";
                return Json(hash, "text/html;charset=UTF-8");
            }

            if (string.IsNullOrEmpty(fileExt) || Array.IndexOf(fileTypes.Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                hash = new Hashtable();
                hash["error"] = 1;
                hash["message"] = "上传文件扩展名是不允许的扩展名";
                return Json(hash, "text/html;charset=UTF-8");
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", System.Globalization.DateTimeFormatInfo.InvariantInfo) + fileExt;
            string filePath = dirPath + newFileName;
            file.SaveAs(filePath);
            string fileUrl = imagespath + saveUrl + newFileName;

            hash = new Hashtable();
            hash["error"] = 0;
            hash["url"] = fileUrl;

            return Json(hash, "text/html;charset=UTF-8");

        }


        //浏览方法
        [HttpGet]
        public ActionResult ProcessRequest()
        {
            string imagespath = System.Configuration.ConfigurationManager.AppSettings["imagespath"];
            //String aspxUrl = context.Request.Path.Substring(0, context.Request.Path.LastIndexOf("/") + 1);

            //根目录路径，相对路径
            String rootPath = "/UploadImages/";  //站点目录+上传目录
            //根目录URL，可以指定绝对路径，
            String rootUrl = "/UploadImages/";
            //图片显示的网络路径
            string networkUrl = imagespath + "UploadImages/";
            //图片扩展名
            String fileTypes = "gif,jpg,jpeg,png,bmp";

            String currentPath = "";
            String currentUrl = "";
            String currentDirPath = "";
            String moveupDirPath = "";

            //根据path参数，设置各路径和URL
            String path = Request.QueryString["path"];
            path = String.IsNullOrEmpty(path) ? "" : path;
            if (path == "")
            {
                currentPath = Server.MapPath("/") + "../Images/" + rootPath;
                currentUrl = Server.MapPath("/") + "../Images/" + rootUrl;
                currentDirPath = "";
                moveupDirPath = "";
            }
            else
            {
                currentPath = Server.MapPath(rootPath) + path;
                currentUrl = rootUrl + path;
                currentDirPath = path;
                moveupDirPath = Regex.Replace(currentDirPath, @"(.*?)[^\/]+\/$", "$1");
            }

            //排序形式，name or size or type
            String order = Request.QueryString["order"];
            order = String.IsNullOrEmpty(order) ? "" : order.ToLower();

            //不允许使用..移动到上一级目录
            if (Regex.IsMatch(path, @"\.\."))
            {
                Response.Write("Access is not allowed.");
                Response.End();
            }
            //最后一个字符不是/
            if (path != "" && !path.EndsWith("/"))
            {
                Response.Write("Parameter is not valid.");
                Response.End();
            }
            //目录不存在或不是目录
            if (!Directory.Exists(currentPath))
            {
                Response.Write("Directory does not exist.");
                Response.End();
            }

            //遍历目录取得文件信息
            string[] dirList = Directory.GetDirectories(currentPath);
            string[] fileList = Directory.GetFiles(currentPath);

            switch (order)
            {
                case "size":
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new SizeSorter());
                    break;
                case "type":
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new TypeSorter());
                    break;
                case "name":
                default:
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new NameSorter());
                    break;
            }

            Hashtable result = new Hashtable();
            result["moveup_dir_path"] = moveupDirPath;
            result["current_dir_path"] = currentDirPath;
            //result["current_url"] = currentUrl;
            result["current_url"] = networkUrl;
            result["total_count"] = dirList.Length + fileList.Length;
            List<Hashtable> dirFileList = new List<Hashtable>();
            result["file_list"] = dirFileList;
            for (int i = 0; i < dirList.Length; i++)
            {
                DirectoryInfo dir = new DirectoryInfo(dirList[i]);
                Hashtable hash = new Hashtable();
                hash["is_dir"] = true;
                hash["has_file"] = (dir.GetFileSystemInfos().Length > 0);
                hash["filesize"] = 0;
                hash["is_photo"] = false;
                hash["filetype"] = "";
                hash["filename"] = dir.Name;
                hash["datetime"] = dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                dirFileList.Add(hash);
            }
            for (int i = 0; i < fileList.Length; i++)
            {
                FileInfo file = new FileInfo(fileList[i]);
                Hashtable hash = new Hashtable();
                hash["is_dir"] = false;
                hash["has_file"] = false;
                hash["filesize"] = file.Length;
                hash["is_photo"] = (Array.IndexOf(fileTypes.Split(','), file.Extension.Substring(1).ToLower()) >= 0);
                hash["filetype"] = file.Extension.Substring(1);
                hash["filename"] = file.Name;
                hash["datetime"] = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                dirFileList.Add(hash);
            }
            //Response.AddHeader("Content-Type", "application/json; charset=UTF-8");
            //context.Response.Write(JsonMapper.ToJson(result));
            //context.Response.End();
            return Json(result, "text/html;charset=UTF-8", JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
