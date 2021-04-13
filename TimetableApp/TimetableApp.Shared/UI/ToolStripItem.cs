using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace TimetableApp.UI
{
    public abstract class ToolStripItem
    {
        public abstract MenuFlyoutItemBase ToItem();
    }
}
