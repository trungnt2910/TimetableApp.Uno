using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace TimetableApp.Skia.Tizen
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new TimetableApp.App(), args);
            host.Run();
        }
    }
}
