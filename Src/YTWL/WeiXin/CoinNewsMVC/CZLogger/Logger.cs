using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using log4net;

namespace CZLogger
{
    public class Logger
    {
        static Logger() { }


        /// <summary>
        /// 记录致命错误信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Fatal(object message, params object[] extendedProperties)
        {
			Log4NetLogger.Fatal(message, extendedProperties);
        }

        /// <summary>
        /// 记录致命错误信息日志
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Fatal(Exception exception, params object[] extendedProperties)
        {
			Log4NetLogger.Fatal(exception, extendedProperties);
        }

        /// <summary>
        /// 记录错误信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Error(object message, params object[] extendedProperties)
        {
			Log4NetLogger.Error(message, extendedProperties);
        }

        /// <summary>
        /// 记录错误信息日志
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Error(Exception exception, params object[] extendedProperties)
        {
			Log4NetLogger.Error(exception, extendedProperties);
        }

        /// <summary>
        /// 记录警告信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Warn(object message, params object[] extendedProperties)
        {
			Log4NetLogger.Warn(message, extendedProperties);
        }

        /// <summary>
        /// 记录警告信息日志
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Warn(Exception exception, params object[] extendedProperties)
        {
			Log4NetLogger.Warn(exception, extendedProperties);
        }

        /// <summary>
        /// 记录提示信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Info(object message, params object[] extendedProperties)
        {
			Log4NetLogger.Info(message, extendedProperties);
        }

        /// <summary>
        /// 记录提示信息日志
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Info(Exception exception, params object[] extendedProperties)
        {
			Log4NetLogger.Info(exception, extendedProperties);
        }

        /// <summary>
        /// 记录跟踪信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Trace(object message, params object[] extendedProperties)
        {
			Log4NetLogger.Trace(message, extendedProperties);
        }

        /// <summary>
        /// 记录跟踪信息日志
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Trace(Exception exception, params object[] extendedProperties)
        {
			Log4NetLogger.Trace(exception, extendedProperties);
        }
    }
}
