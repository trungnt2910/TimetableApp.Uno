using TimetableApp.UI;

namespace TimetableApp.Core
{
    public interface ICredentials
    {
        string Type { get; }
        ToolStripMenuItem[] ToolStripMenuItems { get; }
        bool SupportsOneClick { get; }
        void EnterClass(StudentInfo info);
    }
}
