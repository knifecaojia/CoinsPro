using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using log4net;

namespace CZLogger
{
	internal class Log4NetLogger
    {
        private static readonly ILog _logRollingFile = LogManager.GetLogger("RollingFile");
        private static readonly ILog _logEmail = LogManager.GetLogger("Email");


		static Log4NetLogger() { }

        #region private Functions

        private static void WriteLog(string catagory, Assembly assembly, object message, params object[] extendedProperties)
        {            
            message = MergeMessage(catagory, assembly, message,extendedProperties);
            switch (catagory)
            {
                case LogCategory.Trace:
                    if (_logRollingFile.IsDebugEnabled)
                    {
                        _logRollingFile.Debug(message);
                    }                    
                    break;
                case LogCategory.Info:
                    if (_logRollingFile.IsInfoEnabled)
                    {
                        _logRollingFile.Info(message);
                    }                    
                    break;
                case LogCategory.Warn:
                    if (_logRollingFile.IsWarnEnabled)
                    {
                        _logRollingFile.Warn(message);
                    }                    
                    break;
                case LogCategory.Error:
                    if (_logEmail.IsErrorEnabled)
                    {
                        _logEmail.Error(message);
                    }                    
                    break;
                case LogCategory.Fatal:
                    if (_logEmail.IsFatalEnabled)
                    {
                        _logEmail.Fatal(message);
                    }                    
                    break;
            }
        }

        private static string MergeMessage(string category, Assembly assembly, object message, params object[] extendedProperties)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendLine(string.Format("日志种类： {0}", category));
            buffer.AppendLine(string.Format("程序集： {0}", assembly.FullName));

            if (message != null)
            {
                buffer.AppendLine(string.Format("消息： {0}", message));
            }

            if (extendedProperties != null && extendedProperties.Length != 0)
            {
                string delimeter = ", ";
                IEnumerable<string> values = extendedProperties.Select(item => Convert.ToString(item));
                string extendedMessage = string.Join(delimeter, values.ToArray());
                buffer.AppendLine(string.Format("额外属性： {0}", extendedMessage));
            }
            return buffer.ToString();
        }

        private static string GetExceptionMessage(Exception exception)
        {
            StringBuilder buffer = new StringBuilder();

            if (exception != null)
            {
                buffer.AppendFormat("Exception: {0};", exception.Message + System.Environment.NewLine);
                buffer.AppendFormat("Stack Trace: {0};", exception.StackTrace + System.Environment.NewLine);

                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    buffer.AppendFormat("Inner Exception: {0};", exception.Message + System.Environment.NewLine);
                    buffer.AppendFormat("Inner Stack Trace: {0};", exception.StackTrace + System.Environment.NewLine);
                }
            }

            return buffer.ToString();
        }

        #endregion

        /// <summary>
        /// 记录致命错误信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Fatal(object message, params object[] extendedProperties)
        {
            WriteLog(LogCategory.Fatal, Assembly.GetCallingAssembly(), message, extendedProperties);
        }

        /// <summary>
        /// 记录致命错误信息日志
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Fatal(Exception exception, params object[] extendedProperties)
        {
            Fatal(GetExceptionMessage(exception), extendedProperties);
        }

        /// <summary>
        /// 记录错误信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Error(object message, params object[] extendedProperties)
        {
            WriteLog(LogCategory.Error, Assembly.GetCallingAssembly(), message, extendedProperties);
        }

        /// <summary>
        /// 记录错误信息日志
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Error(Exception exception, params object[] extendedProperties)
        {
            Error(GetExceptionMessage(exception), extendedProperties);
        }

        /// <summary>
        /// 记录警告信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Warn(object message, params object[] extendedProperties)
        {
            WriteLog(LogCategory.Warn, Assembly.GetCallingAssembly(), message, extendedProperties);
        }

        /// <summary>
        /// 记录警告信息日志
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Warn(Exception exception, params object[] extendedProperties)
        {
            Warn(GetExceptionMessage(exception), extendedProperties);
        }

        /// <summary>
        /// 记录提示信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Info(object message, params object[] extendedProperties)
        {
            WriteLog(LogCategory.Info, Assembly.GetCallingAssembly(), message, extendedProperties);
        }

        /// <summary>
        /// 记录提示信息日志
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Info(Exception exception, params object[] extendedProperties)
        {
            Info(GetExceptionMessage(exception), extendedProperties);
        }

        /// <summary>
        /// 记录跟踪信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Trace(object message, params object[] extendedProperties)
        {
            WriteLog(LogCategory.Trace, Assembly.GetCallingAssembly(), message, extendedProperties);
        }

        /// <summary>
        /// 记录跟踪信息日志
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="extendedProperties">可选的扩展属性，如方法参数</param>
        public static void Trace(Exception exception, params object[] extendedProperties)
        {
            Trace(GetExceptionMessage(exception), extendedProperties);
        }
    }
}
