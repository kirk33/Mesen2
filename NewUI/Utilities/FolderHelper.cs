﻿using Mesen.GUI.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesen.Utilities
{
	public static class FolderHelper
	{
		private static HashSet<string> _romExtensions = new HashSet<string>() {
			".sfc", ".smc", ".fig", ".swc", ".bs",
			".gb", ".gbc",
			".nes", ".unif", ".fds", ".studybox"
		};

		public static bool IsRomFile(string path)
		{
			string ext = Path.GetExtension(path).ToLower();
			return _romExtensions.Contains(ext);
		}

		public static bool IsArchiveFile(string path)
		{
			string ext = Path.GetExtension(path).ToLower();
			return ext == ".7z" || ext == ".zip";
		}

		public static bool CheckFolderPermissions(string folder, bool checkWritePermission = true)
		{
			if(!Directory.Exists(folder)) {
				try {
					if(string.IsNullOrWhiteSpace(folder)) {
						return false;
					}
					Directory.CreateDirectory(folder);
				} catch {
					return false;
				}
			}
			if(checkWritePermission) {
				try {
					string fileName = Guid.NewGuid().ToString() + ".txt";
					File.WriteAllText(Path.Combine(folder, fileName), "");
					File.Delete(Path.Combine(folder, fileName));
				} catch {
					return false;
				}
			}
			return true;
		}
		
		//TODO
		/*public static bool MigrateData(string source, string target, Form parent)
		{
			using(frmCopyFiles frm = new frmCopyFiles(source, target)) {
				frm.ShowDialog(parent);
				if(frm.Exception != null) {
					throw frm.Exception;
				}
			}
			if(File.Exists(Path.Combine(source, "settings.backup.xml"))) {
				File.Delete(Path.Combine(source, "settings.backup.xml"));
			}
			File.Move(Path.Combine(source, "settings.xml"), Path.Combine(source, "settings.backup.xml"));

			ConfigManager.InitHomeFolder();
			ConfigManager.ApplyChanges();
			ConfigManager.SaveConfig();

			ConfigManager.RestartMesen(true);

			return true;
		}*/
	}
}