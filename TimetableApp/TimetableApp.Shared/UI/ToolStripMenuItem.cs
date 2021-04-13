using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace TimetableApp.UI
{
    public class ToolStripMenuItem : ToolStripItem
    {
        private bool _enabled;

        public string Text { get; set; }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                OnEnableChanged(new EventArgs());
            }
        }
        public System.Windows.Input.ICommand Command { get; private set; }
        public object CommandParameter => this;

        public event EventHandler Click;
        public event EventHandler EnableChanged;
        
        public ToolStripMenuItem()
        {
            Command = new ToolStripMenuItemClickedCommand(this);
            _enabled = true;
        }

        public ToolStripMenuItem(string text) : this()
        {
            Text = text;
        }

        public void OnClick(EventArgs args)
        {
            Click?.Invoke(this, args);
        }

        public void OnEnableChanged(EventArgs args)
        {
            EnableChanged?.Invoke(this, args);
        }

        public override MenuFlyoutItemBase ToItem()
        {
            return new MenuFlyoutItem()
            {
                Text = Text,
                Command = Command,
                CommandParameter = CommandParameter
            };
        }
    }
}
