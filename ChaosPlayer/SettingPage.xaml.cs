using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ChaosPlayer
{
	/// <summary>
	/// 可用于自身或导航至 Frame 内部的空白页。
	/// </summary>
	public sealed partial class SettingPage : Page
	{
		public SettingPage()
		{
			this.InitializeComponent();
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			ApplicationDataContainer localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;

			if ((_rb_notFullScreen.IsChecked == false && _rb_isFullScreen.IsChecked == false)||(_rb_isAutoPlay.IsChecked==false&&_rb_notAutoPlay.IsChecked==false))
			{
				ContentDialog dia0 = new ContentDialog();
				dia0.Content = "请确保选中选择框！";
				dia0.PrimaryButtonText = "确定";
				dia0.ShowAsync();
			}
			else
			{
				if (_rb_isFullScreen.IsChecked == true)
				{
					localsettings.Values["isFullScreen"] = 1;
				}
				if (_rb_notFullScreen.IsChecked == true)
				{
					localsettings.Values["isFullScreen"] = 0;
				}
				if (_rb_isAutoPlay.IsChecked == true)
				{
					localsettings.Values["isAutoPlay"] = 1;
				}
				if (_rb_notAutoPlay.IsChecked == true)
				{
					localsettings.Values["isAutoPlay"] = 0;
				}
				ContentDialog dia = new ContentDialog();
				dia.Content = "保存成功";
				dia.HorizontalContentAlignment = HorizontalAlignment.Center;
				dia.PrimaryButtonText = "确定";
				dia.ShowAsync();
			}
		}

		private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
		{
			AboutMeFrame.Visibility = Visibility.Visible;
		}
	}
}
