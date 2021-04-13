using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TimetableApp.UI
{
    public class ContextMenuStrip : MenuFlyout
    {
        public new ToolStripItemCollection Items { get; private set; }
        public ContextMenuStrip()
        {
            Items = new ToolStripItemCollection(base.Items);
        }
        public void Show(UIElement control, Point point)
        {
            base.ShowAt(control, point);
        }
    }
}
