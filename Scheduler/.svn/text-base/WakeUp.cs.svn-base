using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.ComponentModel;

namespace Scheduler
{
    class WakeUp
    {
        //A security descriptor to decide what permissions to apply when 
        //OpenWaitableTimer is called. Reference: http://msdn.microsoft.com/en-us/library/ms686670(v=VS.85).aspx
        private const uint _securityDescriptor = 2031619;

        [DllImport("kernel32.dll")]
        public static extern SafeWaitHandle CreateWaitableTimer(IntPtr
        lpTimerAttributes,
        bool bManualReset, string lpTimerName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CancelWaitableTimer(IntPtr hTimer);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenWaitableTimer(uint dwDesiredAccess,
            bool bInheritHandle, string lpTimerName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWaitableTimer(SafeWaitHandle hTimer, [In] ref long
        pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr
        lpArgToCompletionRoutine, bool fResume);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWaitableTimerEx(SafeWaitHandle hTimer, [In] ref long
        pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr
        lpArgToCompletionRoutine, IntPtr WakeContext, ulong TolerableDelay);

        // SetWakeUpTime(object)
        // --------------------------
        // Description: Attempts to set a new wake timer specified by the 
        // object parameter. Expects this parameter to be a DateTime.
        // If any other object is passed in, this method does nothing.
        // --------------------------
        // Calls to: Win32 API
        // Called by: ScheduledEvent
        // -------------------------- 
        public static void SetWakeUpTime(object setTime)
        {
            if (setTime is DateTime)
            {
                try
                {
                    DateTime utc = ((DateTime)setTime).ToUniversalTime();
                    long duetime = utc.ToFileTime();

                    using (SafeWaitHandle handle = CreateWaitableTimer(IntPtr.Zero, true, "Waitabletimer-" + duetime))
                    {
                        if (SetWaitableTimer(handle, ref duetime, 0, IntPtr.Zero, IntPtr.Zero, true))
                        {
                            using (EventWaitHandle wh = new EventWaitHandle(false, EventResetMode.AutoReset))
                            {
                                wh.SafeWaitHandle = handle;
                                wh.WaitOne();
                            }
                        }
                        else
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                    }
                }
                catch { }
            }
        }

        // CancelWakeUpTime(object)
        // --------------------------
        // Description: Attempts to cancel any wake timer specified by the 
        // object parameter. Expects this parameter to be a DateTime.
        // If any other object is passed in, this method does nothing.
        // --------------------------
        // Calls to: Win32 API
        // Called by: ScheduledEvent
        // -------------------------- 
        public static void CancelWakeUpTime(object setTime)
        {
            if (setTime is DateTime)
            {
                try
                {
                    DateTime utc = ((DateTime)setTime).ToUniversalTime();
                    long duetime = utc.ToFileTime();
                    IntPtr handle = OpenWaitableTimer(_securityDescriptor, true, "Waitabletimer-" + duetime);
                    CancelWaitableTimer(handle);
                }
                catch
                { }
            }
        }

        // Wrapper method for use with thread pool.
        public static void ThreadPoolCallback(object setTime)
        {
            SetWakeUpTime(setTime);
        }
    }
}
