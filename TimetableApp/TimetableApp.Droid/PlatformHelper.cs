using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimetableApp.Core
{
    public static partial class PlatformHelper
    {
        static partial void DetectPlatform()
        {
            _platform = Platform.Android;
        }
    }
}
