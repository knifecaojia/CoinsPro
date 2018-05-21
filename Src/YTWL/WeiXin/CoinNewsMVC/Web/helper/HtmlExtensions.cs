using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Web.helper
{
    public static class HtmlExtensions
    {
        public static string Span(this HtmlHelper helper, string strId, string strContent)
        {
            return string.Format("<span id=\"{0}\">{1}</span>", strId, strContent);
        }

        public static string XXX(this HtmlHelper helper) 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("@using (Html.BeginForm())");
            sb.Append("{");
            sb.Append("<span id=\"123\">456</span>");
            sb.Append("}");
            return sb.ToString();
        }


        //获取某个类里的值
        private static string ReflectionGetProperty<T>(T model, string propertyName)
        {
            PropertyInfo[] infos = model.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (info.Name == propertyName && info.CanRead)
                {
                    return info.GetValue(model, null).ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// 获取一个可分页的table控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="helper"></param>
        /// <param name="titleName">标题名称</param>
        /// <param name="list">集合</param>
        /// <param name="columnName">列名</param>
        /// <param name="pageSum">总条数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageCent">中间省略码</param>
        /// <returns></returns>
        public static string Table<T>(this HtmlHelper helper, List<string> titleName, List<T> list, List<string> columnName,int pageSum, int pageIndex = 1, int pageSize = 10,int pageCent = 5)
        {
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var pageNum = 0;  //总页数
            if (pageSum <= pageSize) 
            {
                pageNum = 1;
            }
            else if (pageSum % pageSize > 0)
            {
                pageNum = pageSum / pageSize + 1;
            }
            else
            {
                pageNum = pageSum / pageSize;
            }

            var n = pageIndex * pageSize;
            n = n > pageSum ? pageSum : n;
            var t = (pageIndex - 1) * pageSize + 1;
            //上一页,下一页是否允许点击
            var Previous = "";
            var Next = "";
            if (pageIndex == 1)
            {
                Previous = "disabled";
            }
            if (pageIndex == pageNum)
            {
                Next = "disabled";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"row\">");
            sb.Append("<div class=\"col-lg-1\">");
            sb.Append("<div class=\"form-group\">");
            sb.Append("<input type=\"hidden\" id=\"pageIndex\" name=\"pageIndex\" value=\"@ViewData[\"pageIndex\"]\" />");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("<section id=\"widget-grid\">");
            sb.Append("<div class=\"row\">");
            sb.Append("<article class=\"col-xs-12 col-sm-12 col-md-12 col-lg-12\">");
            sb.Append("<div class=\"jarviswidget jarviswidget-color-blueDark\" id=\"wid-id-0\" data-widget-editbutton=\"false\">");
            sb.Append("<header>");
            sb.Append("<span class=\"widget-icon\"> <i class=\"fa fa-table\"></i> </span>");
            sb.Append("<h2>老司机分页</h2>");

            sb.Append("</header>");
            sb.Append("<div>");
            sb.Append("<div class=\"jarviswidget-editbox\">");

            sb.Append("</div>");
            sb.Append("<div class=\"widget-body\">");
            sb.Append("<div class=\"table-responsive\">");

            sb.Append("<table class=\"table table-bordered\">");
            sb.Append("<thead>");
            sb.Append("<tr>");
            foreach (var item in titleName)
            {
                sb.Append("<th>" + item + "</th>");
            }
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");
            foreach (var item in list)
            {
                sb.Append("<tr>");
                foreach (PropertyInfo prop in props)
                {
                    if (columnName.Contains(prop.Name))
                    {
                        string value = prop.GetValue(item, null) == null ? "" : prop.GetValue(item, null).ToString();
                        sb.Append(" <td>" + value + "</td>");
                    }
                }
                sb.Append("</tr>");
            }
            sb.Append("</tbody>");
            sb.Append("</table>");

            sb.Append("</div>");
            sb.Append("<div class=\"dt-toolbar-footer\">");
            sb.Append("<div class=\"col-sm-6 col-xs-12 hidden-xs\">");
            sb.Append("<div class=\"dataTables_info\" id=\"datatable_col_reorder_info\" role=\"status\" aria-live=\"polite\">");
            sb.Append("pageSize");
            sb.Append("<span class=\"text-primary\">"+pageSize+"</span>");
            sb.Append("Showing");
            sb.Append("<span class=\"txt-color-darken\">"+t+"</span>");
            sb.Append("to");
            sb.Append("<span class=\"txt-color-darken\">"+n+"</span>");
            sb.Append("of");
            sb.Append("<span class=\"text-primary\">"+pageSum+"</span>");
            sb.Append("entries");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("<div class=\"col-sm-6 col-xs-12\">");
            sb.Append("<div class=\"dataTables_paginate paging_simple_numbers\" id=\"datatable_col_reorder_paginate\">");
            sb.Append("<ul id=\"paging_ul\" class=\"pagination pagination-sm\">");
            sb.Append("<li class=\"paginate_button previous "+Previous+"\" aria-controls=\"datatable_col_reorder\" tabindex=\"0\" id=\"datatable_col_reorder_previous\"><a href=\"#\">Previous</a></li>");

            if (pageIndex > pageCent / 2 + 1)
            {
                sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">1</a></li>");
                sb.Append("<li class=\"paginate_button disabled\" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">...</a></li>");
                for (int i = Convert.ToInt32(pageIndex - pageCent / 2); i < pageIndex; i++)
                {
                    sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">"+i+"</a></li>");
                }
            }
            else 
            {
                for (int i = 1; i < pageIndex; i++) 
                {
                    sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">"+i+"</a></li>");
                }
            }
            sb.Append("<li class=\"paginate_button active\" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">"+pageIndex+"</a></li>");

            if ((pageNum - pageIndex) > pageCent / 2) 
            {
                for (int i = Convert.ToInt32(pageIndex + 1); i < pageIndex + pageCent / 2 + 1; i++) 
                {
                    sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">"+i+"</a></li>");
                }
                sb.Append("<li class=\"paginate_button disabled \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">...</a></li>");
                sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">"+pageNum+"</a></li>");
            }
            else 
            {
                for (int i = Convert.ToInt32(pageIndex + 1); i < pageNum + 1; i++) 
                {
                    sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">"+i+"</a></li>");
                }
            }
            sb.Append("<li class=\"paginate_button next "+Next+"\" aria-controls=\"datatable_col_reorder\" tabindex=\"0\" id=\"datatable_col_reorder_next\"><a href=\"#\">Next</a></li>");
            sb.Append("</ul>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");

            sb.Append("</article>");
            sb.Append("</div>");
            sb.Append("</section>");


            sb.Append("<script>");
                sb.Append("$(\"#paging_ul li\").click(function () {");
                    sb.Append("var Current_Page = $(this).find(\"a\").html();");
                    sb.Append("if (!$(this).hasClass(\"active\") && !$(this).hasClass(\"disabled\")) {");
                        sb.Append("if (Current_Page == \"Previous\") {");
                            sb.Append("Current_Page = parseInt($(\".active\").find(\"a\").html()) - 1;");
                        sb.Append("}");
                        sb.Append("if (Current_Page == \"Next\") {");
                            sb.Append("Current_Page = parseInt($(\".active\").find(\"a\").html()) + 1;");
                        sb.Append("}");
                        sb.Append("$(\"#pageIndex\").val(Current_Page);");
                        sb.Append("$('form').submit();");
                    sb.Append("}");
                sb.Append("})");
            sb.Append("</script>");

            return sb.ToString();
        }

        public static string Paging(this HtmlHelper helper,int pageSum, int pageIndex = 1, int pageSize = 10, int pageCent = 5) 
        {
            var pageNum = 0;  //总页数
            if (pageSum <= pageSize)
            {
                pageNum = 1;
            }
            else if (pageSum % pageSize > 0)
            {
                pageNum = pageSum / pageSize + 1;
            }
            else
            {
                pageNum = pageSum / pageSize;
            }

            var n = pageIndex * pageSize;
            n = n > pageSum ? pageSum : n;
            var t = (pageIndex - 1) * pageSize + 1;
            //上一页,下一页是否允许点击
            var Previous = "";
            var Next = "";
            if (pageIndex == 1)
            {
                Previous = "disabled";
            }
            if (pageIndex == pageNum)
            {
                Next = "disabled";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"dt-toolbar-footer\">");
            sb.Append("<div class=\"col-sm-6 col-xs-12 hidden-xs\">");
            sb.Append("<div class=\"dataTables_info\" id=\"datatable_col_reorder_info\" role=\"status\" aria-live=\"polite\">");
            sb.Append("pageSize ");
            sb.Append("<span class=\"text-primary\">" + pageSize + "</span> ");
            sb.Append("Showing ");
            sb.Append("<span class=\"txt-color-darken\">" + t + "</span> ");
            sb.Append("to ");
            sb.Append("<span class=\"txt-color-darken\">" + n + "</span> ");
            sb.Append("of ");
            sb.Append("<span class=\"text-primary\">" + pageSum + "</span> ");
            sb.Append("entries ");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("<div class=\"col-sm-6 col-xs-12\">");
            sb.Append("<div class=\"dataTables_paginate paging_simple_numbers\" id=\"datatable_col_reorder_paginate\">");
            sb.Append("<ul id=\"paging_ul\" class=\"pagination pagination-sm\">");
            sb.Append("<li class=\"paginate_button previous " + Previous + "\" aria-controls=\"datatable_col_reorder\" tabindex=\"0\" id=\"datatable_col_reorder_previous\"><a href=\"#\">Previous</a></li>");

            if (pageIndex > pageCent / 2 + 1)
            {
                sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">1</a></li>");
                sb.Append("<li class=\"paginate_button disabled\" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">...</a></li>");
                for (int i = Convert.ToInt32(pageIndex - pageCent / 2); i < pageIndex; i++)
                {
                    sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">" + i + "</a></li>");
                }
            }
            else
            {
                for (int i = 1; i < pageIndex; i++)
                {
                    sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">" + i + "</a></li>");
                }
            }
            sb.Append("<li class=\"paginate_button active\" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">" + pageIndex + "</a></li>");

            if ((pageNum - pageIndex) > pageCent / 2)
            {
                for (int i = Convert.ToInt32(pageIndex + 1); i < pageIndex + pageCent / 2 + 1; i++)
                {
                    sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">" + i + "</a></li>");
                }
                sb.Append("<li class=\"paginate_button disabled \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">...</a></li>");
                sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">" + pageNum + "</a></li>");
            }
            else
            {
                for (int i = Convert.ToInt32(pageIndex + 1); i < pageNum + 1; i++)
                {
                    sb.Append("<li class=\"paginate_button \" aria-controls=\"datatable_col_reorder\" tabindex=\"0\"><a href=\"#\">" + i + "</a></li>");
                }
            }
            sb.Append("<li class=\"paginate_button next " + Next + "\" aria-controls=\"datatable_col_reorder\" tabindex=\"0\" id=\"datatable_col_reorder_next\"><a href=\"#\">Next</a></li>");
            sb.Append("</ul>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("<script>");
            sb.Append("$(\"#paging_ul li\").click(function () {");
            sb.Append("var Current_Page = $(this).find(\"a\").html();");
            sb.Append("if (!$(this).hasClass(\"active\") && !$(this).hasClass(\"disabled\")) {");
            sb.Append("if (Current_Page == \"Previous\") {");
            sb.Append("Current_Page = parseInt($(\".active\").find(\"a\").html()) - 1;");
            sb.Append("}");
            sb.Append("if (Current_Page == \"Next\") {");
            sb.Append("Current_Page = parseInt($(\".active\").find(\"a\").html()) + 1;");
            sb.Append("}");
            sb.Append("$(\"#pageIndex\").val(Current_Page);");
            sb.Append("$('form').submit();");
            sb.Append("}");
            sb.Append("})");
            sb.Append("</script>");
            return sb.ToString();
        }
    }
}
