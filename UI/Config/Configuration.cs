﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Mesen.GUI.Forms;

namespace Mesen.GUI.Config
{
	public class Configuration
	{
		private const int MaxRecentFiles = 10;
		private bool _needToSave = false;

		public string Version = "0.1.0";
		public List<RecentItem> RecentFiles;
		public VideoConfig Video;
		public DebugInfo Debug;
		public Point? WindowLocation;
		public Size? WindowSize;

		public Configuration()
		{
			RecentFiles = new List<RecentItem>();
			Debug = new DebugInfo();
			Video = new VideoConfig();
		}

		~Configuration()
		{
			//Try to save before destruction if we were unable to save at a previous point in time
			Save();
		}

		public void Save()
		{
			if(_needToSave) {
				Serialize(ConfigManager.ConfigFile);
			}
		}

		public bool NeedToSave
		{
			set
			{
				_needToSave = value;
			}
		}

		public void ApplyConfig()
		{
			Video.ApplyConfig();
		}

		public void InitializeDefaults()
		{
		}
		
		public void AddRecentFile(ResourcePath romFile, ResourcePath? patchFile)
		{
			RecentItem existingItem = RecentFiles.Where((item) => item.RomFile == romFile && item.PatchFile == patchFile).FirstOrDefault();
			if(existingItem != null) {
				RecentFiles.Remove(existingItem);
			}
			RecentItem recentItem = new RecentItem { RomFile = romFile, PatchFile = patchFile };

			RecentFiles.Insert(0, recentItem);
			if(RecentFiles.Count > Configuration.MaxRecentFiles) {
				RecentFiles.RemoveAt(Configuration.MaxRecentFiles);
			}
			ConfigManager.ApplyChanges();
		}

		public static Configuration Deserialize(string configFile)
		{
			Configuration config;

			try {
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
				using(TextReader textReader = new StreamReader(configFile)) {
					config = (Configuration)xmlSerializer.Deserialize(textReader);
				}
			} catch {
				config = new Configuration();
			}

			return config;
		}

		public void Serialize(string configFile)
		{
			try {
				if(!ConfigManager.DoNotSaveSettings) {
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
					using(TextWriter textWriter = new StreamWriter(configFile)) {
						xmlSerializer.Serialize(textWriter, this);
					}
				}
				_needToSave = false;
			} catch {
				//This can sometime fail due to the file being used by another Mesen instance, etc.
				//In this case, the _needToSave flag will still be set, and the config will be saved when the emulator is closed
			}
		}

		public Configuration Clone()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
			StringWriter stringWriter = new StringWriter();
			xmlSerializer.Serialize(stringWriter, this);

			StringReader stringReader = new StringReader(stringWriter.ToString());
			Configuration config = (Configuration)xmlSerializer.Deserialize(stringReader);
			config.NeedToSave = false;
			return config;
		}
	}

	public class RecentItem
	{
		public ResourcePath RomFile;
		public ResourcePath? PatchFile;

		public override string ToString()
		{
			string text;
			/*if(ConfigManager.Config.PreferenceInfo.ShowFullPathInRecents) {
				text = RomFile.ReadablePath.Replace("&", "&&");
			} else {*/
				text = Path.GetFileName(RomFile.FileName).Replace("&", "&&");
			//}

			if(PatchFile.HasValue) {
				text += " [" + Path.GetFileName(PatchFile.Value) + "]";
			}
			return text;
		}
	}

	[Flags]
	public enum DefaultKeyMappingType
	{
		None = 0,
		Xbox = 1,
		Ps4 = 2,
		WasdKeys = 3,
		ArrowKeys = 4
	}
}
