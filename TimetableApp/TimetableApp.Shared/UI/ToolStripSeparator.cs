using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace TimetableApp.UI
{
    public class ToolStripSeparator : ToolStripItem
    {
        public override MenuFlyoutItemBase ToItem()
        {
            return new MenuFlyoutSeparator();
        }
    }
}
