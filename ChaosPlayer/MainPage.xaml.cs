using System;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace ChaosPlayer
{
	/// <summary>
	/// 可用于自身或导航至 Frame 内部的空白页。
	/// </summary>
	public sealed partial class MainPage : Page
    {
		public static MainPage Current;
        public MainPage()
        {

            this.InitializeComponent();
			Current = this;
			ApplicationInitialize();
			Content_Frame.Navigate(typeof(HistoryPage));
        }

		/// <summary>
		/// 应用初始化工作。
		/// </summary>
		private async void ApplicationInitialize()
		{
			Windows.Storage.StorageFolder folder = ApplicationData.Current.LocalFolder;
			try
			{
				StorageFile xmlFile = await folder.CreateFileAsync("Settings.xml", CreationCollisionOption.FailIfExists);
				XmlDocument xml = new XmlDocument();

				XmlElement Root = xml.CreateElement("ChaosPlayer");

				xml.AppendChild(Root);

				await xml.SaveToFileAsync(xmlFile);
				
			}
			catch
			{
				// do nothing
			}
		}

		private void OpenSplitButton_Click(object sender, RoutedEventArgs e)
		{
			MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
		}

		private void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(PlayingItem.IsSelected)
			{
				Frame.Navigate(typeof(PlayingPage));
			}
			if(HistoryItem.IsSelected)
			{
				Content_Frame.Navigate(typeof(HistoryPage));
			}
			if(SettingItem.IsSelected)
			{
				Content_Frame.Navigate(typeof(SettingPage));
			}
			if(FeedbackItem.IsSelected)
			{
				Content_Frame.Navigate(typeof(FeedbackPage));
			}
		}
	}
}
