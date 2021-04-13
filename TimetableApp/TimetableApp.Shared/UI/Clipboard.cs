using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.DataTransfer;

namespace TimetableApp.UI
{
    public static class Clipboard
    {
        public static void SetText(string text)
        {
            var package = new DataPackage();
            package.RequestedOperation = DataPackageOperation.Copy;
            package.SetText(text);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(package);
        }
    }
}
