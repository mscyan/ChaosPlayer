using ChaosPlayer.ClassLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.AccessCache;
using Windows.UI.ViewManagement;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ChaosPlayer
{
	/// <summary>
	/// 可用于自身或导航至 Frame 内部的空白页。
	/// </summary>
	public sealed partial class PlayingPage : Page
	{
		static PageSource pagesource;
		static VideoSource _this_main_video;
		static DispatcherTimer _this_timer = new DispatcherTimer();
		public PlayingPage()
		{
			this.InitializeComponent();
			VideoSlider.CanDrag = false;

			LoudSlider.Maximum = 10;
			LoudSlider.Value = 5;

			// -40
			mediaElement.Height = Window.Current.Bounds.Height;
			mediaElement.Width = Window.Current.Bounds.Width;
		}

		private void InitTimer()
		{
			_this_timer.Tick += refreshSlider;
			_this_timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			_this_timer.Start();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{

			base.OnNavigatedTo(e);
			
			if (e.Parameter != null)
			{
				//播放记录页面跳转过来，不需要将播放记录添加到xml中，只需要获取token
				pagesource = PageSource._HistoryPage;

				string token = e.Parameter.ToString();

				StorageFile video = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);

				var stream = await video.OpenAsync(FileAccessMode.ReadWrite);

				mediaElement.SetSource(stream, video.ContentType);
				StartOrPauseButton.Content = "\xE103";
				mediaElement.Play();

				mediaElement.Volume = LoudSlider.Value / 20;

				_this_main_video = new VideoSource();
				
			}

		}

		private void refreshSlider(object sender, object e)
		{
			string Hour = "0" + ((int)mediaElement.Position.Hours).ToString();
			string Minute = "0" + ((int)mediaElement.Position.Minutes).ToString();
			string Second = "0" + ((int)mediaElement.Position.Seconds).ToString();

			Hours.Text = Hour.Substring(Hour.Length - 2, 2);
			Minutes.Text = Minute.Substring(Minute.Length - 2, 2);
			Seconds.Text = Second.Substring(Second.Length - 2, 2);

			_this_main_video.Position = Hours.Text.ToString() + ":" + Minutes.Text.ToString() + ":" + Seconds.Text.ToString();
		}

		private async void file_open()
		{
			Windows.Storage.Pickers.FileOpenPicker picker =
				new Windows.Storage.Pickers.FileOpenPicker();
			picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;

			picker.FileTypeFilter.Add(".mkv");
			picker.FileTypeFilter.Add(".mp4");
			try
			{
				Windows.Storage.StorageFile video = await picker.PickSingleFileAsync();

				if(video!=null)
				{
					var stream = await video.OpenAsync(Windows.Storage.FileAccessMode.Read);
					string token = StorageApplicationPermissions.FutureAccessList.Add(video);

					//保存缩略图
					var img = await video.GetScaledImageAsThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.VideosView);
					StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
					StorageFile newFile = await storageFolder.CreateFileAsync(video.Name.Substring(0, video.Name.LastIndexOf(".")) + ".png", CreationCollisionOption.ReplaceExisting);
					Stream imgStream = img.AsStreamForRead();
					Stream desStream = await newFile.OpenStreamForWriteAsync();

					byte[] bytes = new byte[1024];
					while (imgStream.Read(bytes, 0, 512) != 0)
					{
						desStream.Write(bytes, 0, 512);
					}
					imgStream.Dispose();
					desStream.Dispose();

					//dialog.ShowAsync();

					mediaElement.SetSource(stream, video.ContentType);
					StartOrPauseButton.Content = "\xE103";
					mediaElement.Play();

					mediaElement.Volume = LoudSlider.Value / 20;
					_this_main_video = new VideoSource(video.Name, video.Path);
					_this_main_video.Thumbnail = newFile.Path.ToString();
					_this_main_video.Token = token;

					pagesource = PageSource._MainPage;
				}
			}
			catch
			{
				// do nothing
			}
		}

		private void StartOrPause()
		{
			if (mediaElement.CurrentState == MediaElementState.Playing)
			{
				mediaElement.Pause();
				StartOrPauseButton.Content = "\xE102";
			}
			else if (mediaElement.CurrentState == MediaElementState.Paused)
			{
				mediaElement.Play();
				StartOrPauseButton.Content = "\xE103";
			}
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			if (mediaElement.NaturalDuration.TimeSpan.TotalSeconds != 0)
			{
				mediaElement.Position -= TimeSpan.FromSeconds(5);
			}
		}

		private void StartOrPauseButton_Click(object sender, RoutedEventArgs e)
		{
			StartOrPause();
		}

		private void ForwardButton_Click(object sender, RoutedEventArgs e)
		{
			if (mediaElement.NaturalDuration.TimeSpan.TotalSeconds != 0)
			{
				mediaElement.Position += TimeSpan.FromSeconds(5);
			}
		}

		private void LoudSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			if (mediaElement.CurrentState == MediaElementState.Paused ||
				mediaElement.CurrentState == MediaElementState.Playing)
			{
				mediaElement.Volume = LoudSlider.Value / 20;
			}
		}

		private void OpenFileButton_Click(object sender, RoutedEventArgs e)
		{
			file_open();
		}

		public enum PageSource
		{
			_MainPage = 0,
			_HistoryPage = 1
		}

		private void StrategyBranch(PageSource pagesource,string FileToken,string FileThumbnail)
		{
			
		}

		private async void AddItemToXml()
		{
			SearchVideo search = new SearchVideo();
			List<VideoSource> list = new List<VideoSource>();
			list = await search.getListAsync();
			bool Add = true;
			foreach (var item in list)
			{
				if (item.Token.Equals(_this_main_video.Token))
				{
					Add = false;
				}
			}
			if (Add)
			{
				SearchVideo.AddItemAsync(_this_main_video.Name,
				_this_main_video.Path,
				"00:00:00",
				_this_main_video.Duration,
				_this_main_video.Thumbnail,
				_this_main_video.Token);
			}
		}

		private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
		{
			VideoSlider.CanDrag = true ;
			_this_main_video.Duration = mediaElement.NaturalDuration.TimeSpan.TotalSeconds.ToString();
			_this_main_video.SetTime();
			switch (pagesource)
			{
				case PageSource._MainPage:
					AddItemToXml();
					break;
				case PageSource._HistoryPage:

					break;
				default:
					break;
			}

			TotalSeconds.Text = _this_main_video.Seconds;
			TotalMinutes.Text = _this_main_video.Minutes;
			TotalHours.Text = _this_main_video.Hours;

			VideoSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
			VideoSlider.Value = 0;

			InitTimer();
		}

		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			mediaElement.Height = Window.Current.Bounds.Height;
			mediaElement.Width = Window.Current.Bounds.Width;
		}

		private void mediaElement_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			
		}

		private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
		{
			VideoSlider.CanDrag = false;
			//播放完毕事件
			_this_timer.Stop();
			
			mediaElement.Stop();
			StartOrPauseButton.Content = "\xE102";
		}

		private void FullScreenButton_Click(object sender, RoutedEventArgs e)
		{
			//  退出全屏  &#xE1D8;
			var view = ApplicationView.GetForCurrentView();
			if (view.IsFullScreenMode)
			{
				view.ExitFullScreenMode();
				FullScreenButton.Content = "\xE1D9";
			}
			else
			{
				view.TryEnterFullScreenMode();
				FullScreenButton.Content = "\xE1D8";
			}
		}

		private void mediaElement_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if(BottomPanel.Visibility == Visibility.Collapsed)
			{
				BottomPanel.Visibility = Visibility.Visible;
			}
			else
			{
				BottomPanel.Visibility = Visibility.Collapsed;
			}
		}

		private void mediaElement_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
		{
			StartOrPause();
		}

		private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			ContentDialog dia = new ContentDialog();
			dia.SecondaryButtonText = "hello";
			dia.ShowAsync();
		}

		private void RelativePanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			ContentDialog dia = new ContentDialog();
			dia.SecondaryButtonText = "ha";
			dia.ShowAsync();
		}
	}
}
