using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using TimetableApp.Core;
using TimetableApp.Dialogs;
using TimetableApp.Pages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TimetableApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public readonly FontIcon CheckboxIcon = new FontIcon() { FontFamily = (FontFamily)Application.Current.Resources["SymbolThemeFontFamily"], Glyph = "\uF16B" };
        public readonly FontIcon CheckboxCompositeIcon = new FontIcon() { FontFamily = (FontFamily)Application.Current.Resources["SymbolThemeFontFamily"], Glyph = "\uF16C" };

        public FontIcon AutoJoinButtonIcon
        {
            get => TimetableApp.Core.Settings.AutoJoin ? CheckboxCompositeIcon : CheckboxIcon;
        }

        public MainPage()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(HomePage));
        }

        #region Events
        private void AutoJoin_Tapped(object sender, TappedRoutedEventArgs args)
        {
            //Toggles the Check icon.
            var item = sender as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            TimetableApp.Core.Settings.AutoJoin = !TimetableApp.Core.Settings.AutoJoin;
            item.Icon = TimetableApp.Core.Settings.AutoJoin ? CheckboxCompositeIcon : CheckboxIcon;
        }

        private async void ChooseAllowEntryBeforeTime_Tapped(object sender, TappedRoutedEventArgs args)
        {
            var dialog = new ChooseAllowEntryBeforeTimeDialog();
            dialog.DisplaySpan = (TimetableApp.Core.Settings.AlwaysAllowJoin) ? null : (TimeSpan?)TimetableApp.Core.Settings.AllowJoinBeforeTime;
            await dialog.ShowAsync();
            if (!dialog.UserResponded) return;
            if (dialog.Result == null)
            {
                TimetableApp.Core.Settings.AlwaysAllowJoin = true;
            }
            else
            {
                TimetableApp.Core.Settings.AlwaysAllowJoin = false;
                TimetableApp.Core.Settings.AllowJoinBeforeTime = dialog.Result.Value;
            }
        }

        private async void ChooseReportBeforeTime_Tapped(object sender, TappedRoutedEventArgs args)
        {
            var dialog = new ChooseReportBeforeTimeDialog();
            dialog.DisplaySpan = (TimetableApp.Core.Settings.AlwaysReport) ? null : (TimeSpan?)TimetableApp.Core.Settings.ReportBeforeTime;
            await dialog.ShowAsync();
            if (!dialog.UserResponded) return;
            if (dialog.Result == null)
            {
                TimetableApp.Core.Settings.AlwaysReport = true;
            }
            else
            {
                TimetableApp.Core.Settings.AlwaysReport = false;
                TimetableApp.Core.Settings.ReportBeforeTime = dialog.Result.Value;
            }
        }

        private async void Load_Tapped(object sender, TappedRoutedEventArgs args)
        {
            var dialog = new LoadNewTimetableDialog();
            await dialog.ShowAsync();
            if (dialog.UserResponded)
            {
                string oldLocation = Data.Timetable.UpdateURL;
                Data.Timetable.UpdateURL = dialog.Result;

                System.Diagnostics.Debug.WriteLine(oldLocation);
                System.Diagnostics.Debug.WriteLine(Data.Timetable.UpdateURL);

                string response = null;
                var result = await Data.Timetable.UpdateAsync();
                if (result != null)
                {
                    Data.Timetable.UpdateURL = oldLocation;
                    response = $"Timetable failed to load: {result}";
                }
                else response = "Timetable successfully loaded.";

                var message = new MessageDialog(response);
                await message.ShowAsync();
            }
        }

        private async void Sync_Tapped(object sender, TappedRoutedEventArgs args)
        {
            var result = await Data.Timetable.UpdateAsync();
            var dialog = new MessageDialog(
                (result == null) ? "Timetable successfully updated" : $"Timetable update failed: {result}", 
                "Update timetable");
            await dialog.ShowAsync();
        }

        private async void About_Tapped(object sender, TappedRoutedEventArgs args)
        {
            var dialog = new AboutDialog();
            await dialog.ShowAsync();
        }

        private async void BugReport_Tapped(object sender, TappedRoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/AzureAms/TimetableApp.Uno/issues"));
        }
        #endregion
    }
}
