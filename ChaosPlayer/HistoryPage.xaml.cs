﻿using ChaosPlayer.ClassLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.UI.Core;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ChaosPlayer
{
	/// <summary>
	/// 可用于自身或导航至 Frame 内部的空白页。
	/// </summary>
	public sealed partial class HistoryPage : Page
	{
		private List<VideoSource> Videos = new List<VideoSource>();
		public HistoryPage()
		{
			
			this.InitializeComponent();
			Init();
			//Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
		}

		//private void BackRequested(object sender, BackRequestedEventArgs e)
		//{
			
		//	MainPage.Current.Frame.Navigate(typeof(MainPage));
		//}

		private async void UpdateItems(string thumbnail, string text1, string text2,string token)
		{
			StackPanel stack = new StackPanel();
			stack.Height = 125;
			stack.Width = 125;
			stack.Margin = new Thickness(0, 0, 0, 0);
			Image img = new Image();
			img.Width = 125;
			img.Source = new BitmapImage(new Uri(thumbnail));
			TextBlock tb1 = new TextBlock();
			tb1.FontSize = 12;
			tb1.Margin = new Thickness(2, 0, 0, 0);
			tb1.Height = 32;
			tb1.TextWrapping = TextWrapping.Wrap;
			tb1.Text = text1;
			tb1.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
			TextBlock tb2 = new TextBlock();	
			tb2.FontSize = 12;
			tb2.Foreground = new SolidColorBrush(Windows.UI.Colors.Teal);
			tb2.Text = "";
			stack.Children.Add(img);
			stack.Children.Add(tb1);
			stack.Children.Add(tb2);
			stack.Background = new SolidColorBrush(Windows.UI.Colors.Brown);
			stack.Name = token;
			_gridView.Items.Add(stack);
			
		}

		private async Task Init()
		{
			SearchVideo sv = new SearchVideo();
			List<VideoSource> list = new List<VideoSource>();
			list = await sv.getListAsync();

			foreach (var item in list)
			{
				UpdateItems(item.Thumbnail, item.Name, item.Position,item.Token);
				_gridView.IsItemClickEnabled = true;
				_gridView.ItemClick += GridView_ItemClick;
			}
			if(list.Count==0)
			{
				TextBlock tb = new TextBlock();
				tb.Text = "暂无播放记录";
				tb.FontSize = 15;
				_gridView.Items.Add(tb);
			}
		}

		private void GridView_ItemClick(object sender, ItemClickEventArgs e)
		{
			StackPanel video = (StackPanel)e.ClickedItem;
			string token = video.Name;

			MainPage.Current.Frame.Navigate(typeof(PlayingPage), token);
		}


		private async void ClearItem()
		{
			SearchVideo.ClearHistoryAsync();

			var items = _gridView.Items;
			if(items.First().GetType()==typeof(TextBlock))
			{
				// do nothing
			}
			else
			{
				_gridView.Items.Clear();
			}
		}

		/// <summary>
		/// 清楚xml中的所有记录，并清楚所有图片。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			ClearItem();
		}
	}
}
		