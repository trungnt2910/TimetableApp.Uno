using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableApp.Dialogs
{
    public sealed class ChooseAllowEntryBeforeTimeDialog : TimeSpanPickerContentDialog
    {
        public ChooseAllowEntryBeforeTimeDialog() : 
            base("How early should we allow you to join the next lesson?",
                "Always allow joining next lesson")
        {

        }
    }
}
