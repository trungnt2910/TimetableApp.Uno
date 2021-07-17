using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableApp.Dialogs
{
    public sealed class ChooseReportBeforeTimeDialog : TimeSpanPickerContentDialog
    {
        public ChooseReportBeforeTimeDialog() : 
            base("How early should we report your lessons?",
                "Always report next lesson")
        {

        }
    }
}
