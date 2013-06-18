using System.Diagnostics;

namespace WIO.Diagnostics
{
    internal static class DebugMode
    {
        public static bool IsDebugging 
        {
            get
            {
#if DEBUG
                return Debugger.IsAttached;
#else
                return false;
#endif
            }
        }
    }
}
