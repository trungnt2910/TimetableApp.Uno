using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TimetableApp.UI;
using System.Threading.Tasks;
using Windows.System;

namespace TimetableApp.Core.Zoom
{
    [Serializable]
    public class ZoomCredentials : ICredentials
    {
        private static bool HasZoomProtocol = false;

        public string ID;
        public string Password;

        static ZoomCredentials()
        {
            Func<Task> func = async () =>
            {
                switch (PlatformHelper.RuntimePlatform)
                {
                    case Platform.Android:
                    case Platform.iOS:
                        HasZoomProtocol = (await Launcher.QueryUriSupportAsync(new Uri("zoomus://zoom.us/join?confno=8529015944&pwd=&uname=Nobody%20-%2051800000000"), LaunchQuerySupportType.Uri) == LaunchQuerySupportStatus.Available);
                        break;
                    default:
                        HasZoomProtocol = (await Launcher.QueryUriSupportAsync(new Uri("zoommtg://zoom.us/join?confno=8529015944&pwd=&uname=Nobody%20-%2051800000000"), LaunchQuerySupportType.Uri) == LaunchQuerySupportStatus.Available);
                        break;
                }
            };
            func();
        }

        [Newtonsoft.Json.JsonIgnore]
        public string Type { get => "Zoom"; }

        [Newtonsoft.Json.JsonIgnore]
        public bool SupportsOneClick { get => HasZoomProtocol; }

        [Newtonsoft.Json.JsonIgnore]
        public ToolStripMenuItem[] ToolStripMenuItems
        {
            get
            {
                var copyID = new ToolStripMenuItem()
                {
                    Text = "Copy ID to clipboard"
                };
                copyID.Click += (sender, args) =>
                {
                    Clipboard.SetText(ID);
                };
                var copyPWD = new ToolStripMenuItem()
                {
                    Text = "Copy password to clipboard"
                };
                copyPWD.Click += (sender, args) =>
                {
                    Clipboard.SetText(Password);
                };
                return new ToolStripMenuItem[] {copyID, copyPWD};
            }
        }

        public void EnterClass(StudentInfo info)
        {
            string link;

            //Launch in the user's computer. This function launches the zoom client directly
            if (HasZoomProtocol)
            {
                switch (PlatformHelper.RuntimePlatform)
                {
                    case Platform.Android:
                    case Platform.iOS:
                        link = $"zoomus://zoom.us/join?confno={ID}&pwd={Password}&uname={Uri.EscapeDataString(info.Name)}";
                    break;
                    default:
                        link = $"zoommtg://zoom.us/join?confno={ID}&pwd={Password}&uname={Uri.EscapeDataString(info.Name)}";
                    break;
                }

            }
            else
            {
                //A generic link that opens zoom meetings in browsers. Does not support automatic names.
                //Therefore, not one-click.
                link = $"https://zoom.us/wc/{ID}/join?pwd={Password}";
            }

            System.Diagnostics.Debug.WriteLine(link);

            Launcher.LaunchUriAsync(new Uri(link));
        }

        public override string ToString()
        {
            return $"ID: {ID}, Password: {Password}";
        }
    }
}
