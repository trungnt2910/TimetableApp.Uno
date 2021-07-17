using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TimetableApp.Dialogs
{
	public sealed partial class ChooseAllowEntryBeforeTimeDialog : ContentDialog
	{
		public TimeSpan? Result
		{
			get => DisplaySpan;
		}

		public TimeSpan? DisplaySpan
        {
			get
            {
				if (Picker.IsEnabled)
				{
					return new TimeSpan(HourPicker.SelectedIndex, MinutePicker.SelectedIndex, SecondPicker.SelectedIndex);
				}
				else return null;
            }
			set
			{
				if (value != null)
                {
					var timeSpan = value.Value;
					CheckBox.IsChecked = false;
					Picker.IsEnabled = true;
					HourPicker.SelectedIndex = timeSpan.Hours;
					MinutePicker.SelectedIndex = timeSpan.Minutes;
					SecondPicker.SelectedIndex = timeSpan.Seconds;
				}
				else
                {
					CheckBox.IsChecked = true;
					Picker.IsEnabled = false;
                }
			}
        }

		public bool UserResponded { get; private set; } = false;

		public ObservableCollection<int> HourPickerItems { get; private set; }
		public ObservableCollection<int> MinutePickerItems { get; private set; }
		public ObservableCollection<int> SecondPickerItems { get; private set; }

		public ChooseAllowEntryBeforeTimeDialog()
		{
			this.InitializeComponent();

			CheckBox.IsChecked = true;
			Picker.IsEnabled = false;

			HourPickerItems = new ObservableCollection<int>(Enumerable.Range(0, 168));
			MinutePickerItems = new ObservableCollection<int>(Enumerable.Range(0, 60));
			SecondPickerItems = new ObservableCollection<int>(Enumerable.Range(0, 60));

			HourPicker.SelectedIndex = 0;
			MinutePicker.SelectedIndex = 0;
			SecondPicker.SelectedIndex = 0;
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			UserResponded = true;
			Hide();
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			Hide();
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs args)
        {
			Picker.IsEnabled = false;
        }

		private void CheckBox_Unchecked(object sender, RoutedEventArgs args)
        {
			Picker.IsEnabled = true;
        }
	}
}
