using System.Runtime.InteropServices;

namespace TimetableApp.Core
{
    public static partial class PlatformHelper
    {
        public static bool IsWindows
        {
            get => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        private static Platform? _platform;
        public static Platform RuntimePlatform
        {
            get
            {
                if (_platform == null)
                {
                    DetectPlatform();
                }
                return _platform.Value;
            }
        }

        static partial void DetectPlatform();
    }
}
