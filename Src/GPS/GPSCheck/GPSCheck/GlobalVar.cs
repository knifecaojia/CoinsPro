using System;
using System.Collections.Generic;

namespace GPSCheck
{
	internal static class GlobalVar
	{
		public static string username;
		public static string groupstr;
        public static List<string> GroupsList;
        public delegate void StartPlayEventHandler();
        public static event StartPlayEventHandler StartPlayEvent;
        public static void RaiseStartPlayEvent()
        {
            if (StartPlayEvent != null)
                StartPlayEvent();
        }
        public delegate void PausePlayEventHandler();
        public static event PausePlayEventHandler PausePlayEvent;
        public static void RaisePausePlayEvent()
        {
            if (PausePlayEvent != null)
                PausePlayEvent();
        }
        public delegate void PlayPointEventHandler(double pct);
        public static event PlayPointEventHandler PlayPointEvent;
        public static void RaisePlayPointEvent( double pct)
        {
            if (PlayPointEvent != null)
                PlayPointEvent(pct);
        }
        public delegate void UpdatePointEventHandler(DateTime t);
        public static event UpdatePointEventHandler UpdatePointEvent;
        public static void RaiseUpdatePointEvent(DateTime t)
        {
            if (UpdatePointEvent != null)
                UpdatePointEvent(t);
        }
    }
}
