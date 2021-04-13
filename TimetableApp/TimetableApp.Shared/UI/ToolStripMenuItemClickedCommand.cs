using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableApp.UI
{
    public class ToolStripMenuItemClickedCommand : System.Windows.Input.ICommand
    {
        private ToolStripMenuItem owner;
        public bool CanExecute(object parameter)
        {
            return owner.Enabled;
        }

        public void Execute(object param)
        {
            (param as ToolStripMenuItem).OnClick(new EventArgs());
        }

        public event EventHandler CanExecuteChanged;

        public ToolStripMenuItemClickedCommand(ToolStripMenuItem owner)
        {
            this.owner = owner;
        }
    }
}
