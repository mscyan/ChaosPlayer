using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
//using System.Xml;
//using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
//using Windows.UI.Xaml;
using Windows.Data.Xml.Dom;

namespace ChaosPlayer.ClassLib
{
    class SearchVideo
    {
		public List<VideoSource> videoList = new List<VideoSource>();
		public SearchVideo()
		{
			
		}

		public List<VideoSource> GetList()
		{
			var a = getListAsync();
			return videoList ;
		}

		public async Task<List<VideoSource>> getListAsync()
		{
			StorageFolder folder = ApplicationData.Current.LocalFolder;
			StorageFile file = await folder.CreateFileAsync("Settings.xml", CreationCollisionOption.OpenIfExists);

			System.Xml.XmlReader reader = System.Xml.XmlReader.Create(file.Path);
			System.Xml.XmlDocument videoDataXML = new System.Xml.XmlDocument();
			videoDataXML.Load(reader);
			System.Xml.XmlNode root = videoDataXML.DocumentElement;
			System.Xml.XmlNodeList configs = root.ChildNodes;
			foreach (System.Xml.XmlNode item in configs)
			{
				VideoSource video = new VideoSource();

				System.Xml.XmlElement config = item as System.Xml.XmlElement;
				string filename = config.GetAttribute("filename");
				string path = config.ChildNodes.Item(0).InnerText;
				string position = config.ChildNodes.Item(1).InnerText;
				string duration = config.ChildNodes.Item(2).InnerText;
				string thumbnail = config.ChildNodes.Item(3).InnerText;
				string token = config.ChildNodes.Item(4).InnerText;

				video.Name = filename;
				video.Path = path;
				video.Position = position;
				video.Duration = duration;
				video.Thumbnail = thumbnail;
				video.Token = token;
				videoList.Add(video);
			}

			reader.Dispose();
			return videoList;
		}

		public async static void ClearHistoryAsync()
		{
			StorageFolder folder = ApplicationData.Current.LocalFolder;
			StorageFile file_ = await folder.CreateFileAsync("Settings.xml",CreationCollisionOption.OpenIfExists);
			XmlDocument xml = new XmlDocument();

			XmlElement Root = xml.CreateElement("ChaosPlayer");

			xml.AppendChild(Root);

			//await xml.SaveToFileAsync(file_);
			xml.SaveToFileAsync(file_);
			
		}

		public static async void AddItemAsync(string filename,string filepath,string fileposition,string fileduration,string filethumnnail,string filetoken)
		{
			
			StorageFolder folder = ApplicationData.Current.LocalFolder;
			StorageFile file = await folder.CreateFileAsync("Settings.xml",CreationCollisionOption.OpenIfExists);
			XmlDocument xml = await XmlDocument.LoadFromFileAsync(file);


			XmlElement root = xml.DocumentElement;

			XmlElement videoItem = xml.CreateElement("VideoItem");
			videoItem.SetAttribute("filename", filename);

			XmlElement path = xml.CreateElement("path");
			path.InnerText = filepath;
			XmlElement position = xml.CreateElement("position");
			position.InnerText = fileposition;
			XmlElement duration = xml.CreateElement("duration");
			duration.InnerText = fileduration;
			XmlElement thumbnail = xml.CreateElement("thumbnail");
			thumbnail.InnerText = filethumnnail;
			XmlElement token = xml.CreateElement("token");
			token.InnerText = filetoken;

			videoItem.AppendChild(path);
			videoItem.AppendChild(position);
			videoItem.AppendChild(duration);
			videoItem.AppendChild(thumbnail);
			videoItem.AppendChild(token);

			xml.DocumentElement.AppendChild(videoItem);

			await xml.SaveToFileAsync(file);
			
		}
	}
}
