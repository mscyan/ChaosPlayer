using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosPlayer.ClassLib
{
	class VideoSource
	{
		public VideoSource()
		{

		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name">文件名</param>
		/// <param name="path">路径</param>
		public VideoSource(string name, string path)
		{
			this.name = name;
			this.path = path;
		}

		
		private string token;
		/// <summary>
		/// token 用户从应用程序文件记录中获取文件权限的令牌
		/// </summary>
		public string Token
		{
			get { return token; }
			set { token = value; }
		}

		/// <summary>
		/// 缩略图文件的名称
		/// </summary>
		private string thumbnail;
		public string Thumbnail
		{
			get { return thumbnail; }
			set { thumbnail = value; }
		}

		private string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string path;
		public string Path
		{
			get { return path; }
			set { path = value; }
		}

		private string duration;
		public string Duration
		{
			get { return duration; }
			set { duration = value; }
		}

		private string position;
		public string Position
		{
			get { return position; }
			set { position = value; }
		}

		public void SetTime()
		{
			double timeLen = double.Parse(duration);

			string hour = ((int)(timeLen / 3600)).ToString();
			string minute = ((int)(timeLen / 60 % 60)).ToString();
			string second = ((int)(timeLen % 60)).ToString();

			hour = "0" + hour;
			Hours = hour.Substring(hour.Length - 2, 2);
			minute = "0" + minute;
			Minutes = minute.Substring(minute.Length - 2, 2);
			second = "0" + second;
			Seconds = second.Substring(second.Length - 2, 2);
		}

		public string Hours { get; set; }
		public string Minutes { get; set; }
		public string Seconds { get; set; }
	}
}
