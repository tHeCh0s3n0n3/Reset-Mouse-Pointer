using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ResetMousePointer
{
    public class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        private const uint SPI_SETCURSORS = 0x0057;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDCHANGE = 0x02;

        public static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                return -2;
            }

            SystemParametersInfo(SPI_SETCURSORS, 0, new IntPtr(), SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            // Get the last error and display it.
            int errorCode = Marshal.GetLastWin32Error();

            string eventSource;

            if (0 != errorCode)
            {
                eventSource = CreateEventSource(AppDomain.CurrentDomain.FriendlyName);
                EventLog appLog = new EventLog("Application")
                {
                    Source = eventSource
                };
                appLog.WriteEntry($"Failed with error code {errorCode}", EventLogEntryType.Error);
            }

            return errorCode;
        }

        private static string CreateEventSource(string currentAppName)
        {
            string eventSource = currentAppName;
            bool sourceExists;
            try
            {
                // searching the source throws a security exception ONLY if not exists!
                sourceExists = EventLog.SourceExists(eventSource);
                if (!sourceExists)
                {   // no exception until yet means the user as admin privilege
                    EventLog.CreateEventSource(eventSource, "Application");
                }
            }
            catch (System.Security.SecurityException)
            {
                eventSource = "Application";
            }

            return eventSource;
        }
    }
}
