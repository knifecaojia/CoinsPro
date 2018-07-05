using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public static class TimerHelper
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccesss, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, [MarshalAs(UnmanagedType.Struct)] ref LUID lpLuid);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, [MarshalAs(UnmanagedType.Struct)]ref TOKEN_PRIVILEGES NewState, uint BufferLength, IntPtr PreviousState, uint ReturnLength);
        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref SystemTime lpSystemTime);

        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime lpSystemTime);

        [DllImport("Kernel32.dll")]
        public static extern void GetSystemTime(ref SystemTime lpSystemTime);

        [DllImport("Kernel32.dll")]
        public static extern bool SetSystemTime(ref SystemTime lpSystemTime);
        public const uint TOKEN_QUERY = 0x0008;
        public const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        public const uint SE_PRIVILEGE_ENABLED = 0x00000002;

        // 授予权限  
        public static void grantTimeZonePrivilege()
        {
            bool flag;
            LUID locallyUniqueIdentifier = new LUID();
            flag = LookupPrivilegeValue(null, "SeTimeZonePrivilege", ref locallyUniqueIdentifier);
            TOKEN_PRIVILEGES tokenPrivileges = new TOKEN_PRIVILEGES();
            tokenPrivileges.PrivilegeCount = 1;

            LUID_AND_ATTRIBUTES luidAndAtt = new LUID_AND_ATTRIBUTES();
            // luidAndAtt.Attributes should be SE_PRIVILEGE_ENABLED to enable privilege  
            luidAndAtt.Attributes = SE_PRIVILEGE_ENABLED;
            luidAndAtt.Luid = locallyUniqueIdentifier;
            tokenPrivileges.Privilege = luidAndAtt;

            IntPtr tokenHandle = IntPtr.Zero;
            flag = OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out tokenHandle);
            flag = AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 1024, IntPtr.Zero, 0);
            flag = CloseHandle(tokenHandle);
        }

        // 撤销权限  
        public static void revokeTimeZonePrivilege()
        {
            bool flag;
            LUID locallyUniqueIdentifier = new LUID();
            LookupPrivilegeValue(null, "SeTimeZonePrivilege", ref locallyUniqueIdentifier);
            TOKEN_PRIVILEGES tokenPrivileges = new TOKEN_PRIVILEGES();
            tokenPrivileges.PrivilegeCount = 1;

            LUID_AND_ATTRIBUTES luidAndAtt = new LUID_AND_ATTRIBUTES();
            // luidAndAtt.Attributes should be none (not set) to disable privilege  
            luidAndAtt.Luid = locallyUniqueIdentifier;
            tokenPrivileges.Privilege = luidAndAtt;

            IntPtr tokenHandle = IntPtr.Zero;
            flag = OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out tokenHandle);
            flag = AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 1024, IntPtr.Zero, 0);
            flag = CloseHandle(tokenHandle);
        }
        static public double GetTimeStamp(DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            double t = (dt - startTime).TotalSeconds;   //除10000调整为13位      
            return Math.Round(t);
        }
        /// <summary>
        /// 返回的是nano
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        static public long GetTimeStampMilliSeconds(DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (long)(dt - startTime).TotalSeconds * 1000;   //除10000调整为13位      
            return t;
        }
        static public DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1);


            return dtStart.AddSeconds(Convert.ToDouble(timeStamp));
        }
        static public DateTime ConvertStringToDateTime(double timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1);

            return dtStart.AddSeconds(timeStamp);
        }
        static public string GetTimeStampNonce()
        {
            return GetTimeStamp(DateTime.Now).ToString();
        }
        /// <summary>
        /// 根据时间间隔返回redis需要的时间key 该函数应该也可以使用到文件目录中
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="inputTimeStamp"></param>
        /// <returns></returns>
        static public DateTime GetStartTimeStampByPreiod(TimePeriodType tp, DateTime inputTimeStamp)
        {
            int min = 0;
            switch (tp)
            {
                case TimePeriodType.m1:
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, inputTimeStamp.Minute, 0);
                case TimePeriodType.m5:
                    min = inputTimeStamp.Minute - inputTimeStamp.Minute % 5;
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, min, 0);
                case TimePeriodType.m10:
                    min = inputTimeStamp.Minute - inputTimeStamp.Minute % 10;
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, min, 0);
                case TimePeriodType.m30:
                    min = inputTimeStamp.Minute - inputTimeStamp.Minute % 30;
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, min, 0);
                case TimePeriodType.h1:
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, 0, 0);
                case TimePeriodType.h4:
                    int hour = inputTimeStamp.Hour - inputTimeStamp.Hour % 4;
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, hour, 0, 0);
                case TimePeriodType.d1:
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, 0, 0, 0);
                default:
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, inputTimeStamp.Minute, 0);
            }
        }
        /// <summary>
        /// 根据时间间隔返回redis需要的时间key 该函数应该也可以使用到文件目录中
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="inputTimeStamp"></param>
        /// <returns></returns>
        static public DateTime GetEndTimeStampByPreiod(TimePeriodType tp, DateTime inputTimeStamp)
        {
            
            switch (tp)
            {
                case TimePeriodType.m1:
                    return inputTimeStamp.AddMinutes(1);
                case TimePeriodType.m5:
                    
                     return inputTimeStamp.AddMinutes(5);
                case TimePeriodType.m10:
                     return inputTimeStamp.AddMinutes(10);
                case TimePeriodType.m30:
                    return inputTimeStamp.AddMinutes(30);
                case TimePeriodType.h1:
                    return inputTimeStamp.AddHours(1);
                case TimePeriodType.h4:
                    return inputTimeStamp.AddHours(4);
                case TimePeriodType.d1:
                    return inputTimeStamp.AddHours(24);
                default:
                    return inputTimeStamp.AddMinutes(30);
            }
        }
        /// <summary>  
        /// 获取本地时间  
        /// </summary>  
        /// <returns></returns>  
        public static DateTime getLocalTime()
        {
            SystemTime sysTime = new SystemTime();
            GetLocalTime(ref sysTime);
            return SystemTime2DateTime(sysTime);
        }

        /// <summary>  
        /// 设置本地时间  
        /// </summary>  
        /// <param name="dateTime"></param>  
        /// <returns></returns>  
        public static bool setLocalTime(DateTime dateTime)
        {
            grantTimeZonePrivilege();
            SystemTime sysTime = DateTime2SystemTime(dateTime);
            bool success = SetLocalTime(ref sysTime);
            revokeTimeZonePrivilege();
            return success;

        }

        /// <summary>  
        /// 获取系统时间  
        /// </summary>  
        /// <returns></returns>  
        public static DateTime getSystemTime()
        {
            SystemTime sysTime = new SystemTime();
            GetSystemTime(ref sysTime);
            return SystemTime2DateTime(sysTime);
        }

        /// <summary>  
        /// 设置系统时间（UTC）  
        /// </summary>  
        /// <param name="dateTime"></param>  
        /// <returns></returns>  
        public static bool setSystemTime(DateTime dateTime)
        {


            grantTimeZonePrivilege();
            // 授权成功  
            SystemTime sysTime = DateTime2SystemTime(dateTime);
            bool success = SetSystemTime(ref sysTime);
            revokeTimeZonePrivilege();
            return success;

            


        }

        /// <summary>  
        /// 将SystemTime转换为DateTime  
        /// </summary>  
        /// <param name="sysTime"></param>  
        /// <returns></returns>  
        public static DateTime SystemTime2DateTime(SystemTime sysTime)
        {
            return new DateTime(sysTime.year, sysTime.month, sysTime.day, sysTime.hour, sysTime.minute, sysTime.second, sysTime.milliseconds);
        }

        /// <summary>  
        /// 将DateTime转换为SystemTime  
        /// </summary>  
        /// <param name="dateTime"></param>  
        /// <returns></returns>  
        public static SystemTime DateTime2SystemTime(DateTime dateTime)
        {
            SystemTime sysTime = new SystemTime();
            sysTime.year = Convert.ToUInt16(dateTime.Year);
            sysTime.month = Convert.ToUInt16(dateTime.Month);
            sysTime.day = Convert.ToUInt16(dateTime.Day);
            sysTime.hour = Convert.ToUInt16(dateTime.Hour);
            sysTime.minute = Convert.ToUInt16(dateTime.Minute);
            sysTime.second = Convert.ToUInt16(dateTime.Second);
            sysTime.milliseconds = Convert.ToUInt16(dateTime.Millisecond);
            return sysTime;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        [MarshalAs(UnmanagedType.U2)]
        internal ushort year; // 年  
        [MarshalAs(UnmanagedType.U2)]
        internal ushort month; // 月  
        [MarshalAs(UnmanagedType.U2)]
        internal ushort dayOfWeek; // 星期  
        [MarshalAs(UnmanagedType.U2)]
        internal ushort day; // 日  
        [MarshalAs(UnmanagedType.U2)]
        internal ushort hour; // 时  
        [MarshalAs(UnmanagedType.U2)]
        internal ushort minute; // 分  
        [MarshalAs(UnmanagedType.U2)]
        internal ushort second; // 秒  
        [MarshalAs(UnmanagedType.U2)]
        internal ushort milliseconds; // 毫秒  
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LUID
    {
        public int LowPart;
        public uint HighPart;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LUID_AND_ATTRIBUTES
    {
        public LUID Luid;
        public uint Attributes;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TOKEN_PRIVILEGES
    {
        public int PrivilegeCount;
        public LUID_AND_ATTRIBUTES Privilege;
    }
}
