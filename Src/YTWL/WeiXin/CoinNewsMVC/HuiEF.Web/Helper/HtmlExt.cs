using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Residentialsoft.UI.Extensions
{
    /// <summary>
    /// 创建人：惠
    /// 创建时间：2017-01-17
    /// 功能说明：
    /// Html扩展类后台公用MVC视图HTML组件扩展
    /// </summary>
    public static class HtmlExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name">控件名称</param>
        /// <param name="value">控件值</param>
        /// <param name="url">对话框URL</param>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="style"></param>
        /// <param name="css"></param>
        /// <returns></returns>
        public static HtmlString DialogInput(this HtmlHelper helper, string name, string value, string url, string title, int width, int height, string style, string css = "")
        {
            var js_dialog = "window.top.showLayerDialog('{0}', '{1}', {2}, {3}, false);";
            var dialog_url = url;
            if (dialog_url.IndexOf('?') > 0)
            {
                dialog_url = url + "&cid=" + name;
            }
            else
            {
                dialog_url = url + "?cid=" + name;
            }
            js_dialog = string.Format(js_dialog, dialog_url, title, width, height);

            return new HtmlString(string.Format(@" <div class=""clearfix"">
                                <div class=""pull-left"">
                                    <input id=""{0}"" name=""{0}"" type=""text"" class=""input_text {4}"" value=""{1}"" style=""{3}"" />&nbsp;
                                </div>
                                <div class=""pull-left btn-group"">
                                    <a class=""btn"" href=""javascript:;"" title=""选择"" onclick=""{2}""><i class=""icon-th""></i></a>
                                </div>
                            </div>", name, value, js_dialog, style, css));
        }

        /// <summary>
        /// 生成录入图片框
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name">控件名称</param>
        /// <param name="src">图片地址</param>
        /// <param name="style">样式</param>
        /// <returns></returns>
        public static HtmlString ImageBox(this HtmlHelper helper, string name, string src, string style, string css = "")
        {
            return new HtmlString(string.Format(@" <div class=""clearfix"">
                                <div class=""pull-left"">
                                    <input id=""{0}"" name=""{0}"" type=""text"" class=""input_text {3}"" value=""{1}"" style=""{2}"" />&nbsp;
                                </div>
                                <div class=""pull-left btn-group"">
                                    <a class=""btn"" href=""javascript:;"" title=""选择"" onclick=""window.top.showBrowerImageDialog('{0}')""><i class=""icon-th""></i></a>
                                    <a class=""btn"" href=""javascript:;"" title=""上传"" onclick=""window.top.showUpImageDialog('{0}')""><i class=""icon-arrow-up""></i></a>
                                    <a class=""btn"" href=""javascript:;"" title=""裁切"" onclick=""window.top.showClipImageDialog('{0}')""><i class=""icon-crop""></i></a>
                                    <a class=""btn"" href=""javascript:;"" title=""预览"" onclick=""window.top.showPreviewImage('{0}')""><i class=""icon-eye-open""></i></a>
                                </div>
                            </div>", name, src, style, css));
        }

        /// <summary>
        /// 生成普通文件上传框
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name">控件名称</param>
        /// <param name="src">文件地址</param>
        /// <param name="style">样式</param>
        /// <returns></returns>
        public static HtmlString FileBox(this HtmlHelper helper, string name, string src, string style, string css = "")
        {
            return new HtmlString(string.Format(@" <div class=""clearfix"">
                                <div class=""pull-left"">
                                    <input id=""{0}"" name=""{0}"" type=""text"" class=""input_text {3}"" value=""{1}"" style=""{2}"" />&nbsp;
                                </div>
                                <div class=""pull-left btn-group"">
                                    <a class=""btn"" href=""javascript:;"" title=""选择"" onclick=""window.top.showBrowerFileDialog('{0}')""><i class=""icon-th""></i></a>
                                    <a class=""btn"" href=""javascript:;"" title=""上传"" onclick=""window.top.showUpFileDialog('{0}')""><i class=""icon-arrow-up""></i></a>
                                    <a class=""btn"" href=""javascript:;"" title=""预览"" onclick=""window.top.showPreviewFile('{0}')""><i class=""icon-eye-open""></i></a>
                                </div>
                            </div>", name, src, style, css));
        }

        /// <summary>
        /// 生成视频文件上传框
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name">控件名称</param>
        /// <param name="src">视频文件地址</param>
        /// <param name="style">样式</param>
        /// <returns></returns>
        public static HtmlString VideoBox(this HtmlHelper helper, string name, string src, string style, string css = "")
        {
            return new HtmlString(string.Format(@" <div class=""clearfix"">
                                <div class=""pull-left"">
                                    <input id=""{0}"" name=""{0}"" type=""text"" class=""input_text {3}"" value=""{1}"" style=""{2}"" />&nbsp;
                                </div>
                                <div class=""pull-left btn-group"">
                                    <a class=""btn"" href=""javascript:;"" title=""选择"" onclick=""window.top.showBrowerVideoDialog('{0}')""><i class=""icon-th""></i></a>
                                    <a class=""btn"" href=""javascript:;"" title=""上传"" onclick=""window.top.showUpVideoDialog('{0}')""><i class=""icon-arrow-up""></i></a>
                                    <a class=""btn"" href=""javascript:;"" title=""预览"" onclick=""window.top.showPreviewVideo('{0}')""><i class=""icon-eye-open""></i></a>
                                </div>
                            </div>", name, src, style, css));
        }

        /// <summary>
        /// 生成音乐文件上传框
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name">控件名称</param>
        /// <param name="src">音乐文件地址</param>
        /// <param name="style">样式</param>
        /// <returns></returns>
        public static HtmlString MusicBox(this HtmlHelper helper, string name, string src, string style, string css = "")
        {
            return new HtmlString(string.Format(@" <div class=""clearfix"">
                                <div class=""pull-left"">
                                    <input id=""{0}"" name=""{0}"" type=""text"" class=""input_text {3}"" value=""{1}"" style=""{2}"" />&nbsp;
                                </div>
                                <div class=""pull-left btn-group"">
                                    <a class=""btn"" href=""javascript:;"" title=""选择""><i class=""icon-th""></i></a>
                                    <a class=""btn"" href=""javascript:;"" title=""上传""><i class=""icon-arrow-up""></i></a>
                                    <a class=""btn"" href=""javascript:;"" title=""预览""><i class=""icon-eye-open""></i></a>
                                </div>
                            </div>", name, src, style, css));
        }

        /// <summary>
        /// 初始化编辑器
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static HtmlString InitUEditor(this HtmlHelper helper)
        {
            string html = @"<script type=""text/javascript"">
                var InitEditorConfig = function () {
                    window.UEDITOR_SNAP_HOST = """ + GetWebSiteHost() + @""";
                    window.UEDITOR_HOME_URL = ""/sitefiles/bairong/texteditor/ueditor/"";
                    window.UEDITOR_IMAGE_URL = ""/website.admin/ueditor/uploadimage"";
                    window.UEDITOR_SCRAWL_URL = ""/website.admin/ueditor/uploadscrawl"";
                    window.UEDITOR_CATCH_URL = ""/website.admin/ueditor/catchimage"";
                    window.UEDITOR_FILE_URL = ""/website.admin/ueditor/uploadfile"";
                    window.UEDITOR_IMAGE_MANAGER_URL = ""/website.admin/ueditor/listimage"";
                    window.UEDITOR_MOVIE_URL = ""/website.admin/ueditor/uploadvideo""
                }
                InitEditorConfig();
            </script>
            <script type=""text/javascript"" src=""/sitefiles/bairong/texteditor/ueditor/editor_config.js""></script>
            <script type=""text/javascript"" src=""/sitefiles/bairong/texteditor/ueditor/editor_all.js""></script>";
            return new HtmlString(html);
        }

        /// <summary>
        /// 创建编辑器
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name">控件名称</param>
        /// <returns></returns>
        public static HtmlString UEditor(this HtmlHelper helper, string name)
        {
            string html = @"<script type=""text/javascript""> $(function () {$('#" + name + @"').show();" +
                        @"window.ue_" + name + @" = UE.getEditor('" + name + @"'); });</script>";
            return new HtmlString(html);
        }

        /// <summary>
        /// 创建编辑器
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name">控件名称</param>
        /// <param name="opts">编辑器opts参数js变量名</param>
        /// <returns></returns>
        public static HtmlString UEditor(this HtmlHelper helper, string name, string opts)
        {
            string html = @"<script type=""text/javascript""> $(function () {$('#" + name + @"').show();" +
                        @"window.ue_" + name + @" = UE.getEditor('" + name + @"'," + opts + "); });</script>";
            return new HtmlString(html);
        }

        /// <summary>
        /// 取得当前服务器的网址
        /// </summary>
        /// <returns></returns>
        private static string GetWebSiteUrl()
        {
            var req = HttpContext.Current.Request;
            var port = req.Url.Port;
            return req.Url.Scheme + "://" + req.Url.DnsSafeHost
                + (port == 80 ? "" : ":" + port);
        }

        /// <summary>
        /// 获取当前站点的Web主机名称（或域名,包含接口号)
        /// </summary>
        /// <returns></returns>
        private static string GetWebSiteHost()
        {
            var req = HttpContext.Current.Request;
            var port = req.Url.Port;
            return req.Url.DnsSafeHost + (port == 80 ? "" : ":" + port);
        }

    }
}