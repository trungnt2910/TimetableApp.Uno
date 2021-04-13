using System;
using System.Collections.Generic;
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
					Picker.IsEnabled = true;
					HourPicker.SelectedIndex = timeSpan.Hours;
					MinutePicker.SelectedIndex = timeSpan.Minutes;
					SecondPicker.SelectedIndex = timeSpan.Seconds;
				}
				else
                {
					Picker.IsEnabled = false;
                }
			}
        }

		public bool UserResponded { get; private set; } = false;

		public ChooseAllowEntryBeforeTimeDialog()
		{
			this.InitializeComponent();

			CheckBox.IsChecked = true;
			Picker.IsEnabled = false;

			AddRange(HourPicker.Items, 0, 168);
			AddRange(MinutePicker.Items, 0, 60);
			AddRange(SecondPicker.Items, 0, 60);

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

		private void AddRange(ItemCollection col, int begin, int end)
        {
			for (int i = begin; i < end; ++i)
            {
				col.Add(i);
            }
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
