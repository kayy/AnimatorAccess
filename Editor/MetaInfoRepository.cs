// The MIT License (MIT)
// 
// Copyright (c) 2014 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using AnimatorAccess;
using Scio.CodeGeneration;

namespace Scio.AnimatorAccessGenerator
{
	public class MetaInfoRepository
	{
		public class MetaInfo
		{
			public string file;
			public string backupFile;
			public string timestamp;

			public MetaInfo (string file, string backupFile, string timestamp) {
				this.file = file;
				this.backupFile = backupFile;
				this.timestamp = timestamp;
			}
		}
		Dictionary<string, MetaInfo> entries = new Dictionary<string, MetaInfo> (); 

		string backupDir;
		
		public void Prepare () {
			backupDir = Preferences.GetString (Preferences.Key.BackupDir);
			if (string.IsNullOrEmpty (backupDir) || !Directory.Exists (backupDir)) {
				backupDir = Application.temporaryCachePath + "/backup";
				try {
					Directory.CreateDirectory (backupDir);
					Preferences.SetString (Preferences.Key.BackupDir, backupDir);
				} catch (System.Exception ex) {
					Logger.Warning (ex.Message);
					backupDir = "";
				}
			}
		}
		
		public void MakeBackup (string className, string file) {
			string backupFile = MakeBackup (file);
			string timestamp = "" + File.GetCreationTime (backupFile);
			entries [className] =  new MetaInfo (file, backupFile, timestamp);
		}

		string GetKey (BaseAnimatorAccess component) {
			if (component != null) {
				return component.GetType ().Name;
			}
			return "";
		}
		
		public string GetFile (BaseAnimatorAccess component) {
			return Get (component).file;
		}

		public string GetBackupTimestamp (BaseAnimatorAccess component) {
			string key = GetKey (component);
			if (entries.ContainsKey (key)) {
				return entries[key].timestamp;
			}
			return "";
		}
		
		public bool HasBackup (BaseAnimatorAccess component) {
			return !string.IsNullOrEmpty (Get (component).backupFile);
		}
		
		public string RemoveBackup (BaseAnimatorAccess component) {
			string key = GetKey (component);
			if (entries.ContainsKey (key)) {
				string s = entries[key].backupFile;
				entries[key].backupFile = "";
				return s;
			}
			return "";
		}
		
		MetaInfo Get (BaseAnimatorAccess component) {
			var key = GetKey (component);
			if (!entries.ContainsKey (key)) {
				string fileName = key +  ".cs";
				string backupFileName = key +  ".txt";
				string file = CodeGenerationUtils.GetPathToFile (fileName);
				string backupFile = CodeGenerationUtils.GetPathToFile (backupFileName, backupDir);
				string timestamp = "";
				if (!string.IsNullOrEmpty (backupFile)) {
				 	timestamp += File.GetCreationTime (backupFile);
				}
				entries [key] = new MetaInfo (file, backupFile, timestamp);
			}
			return entries [key];
		}
		
		string MakeBackup (string inputFile) {
			if (File.Exists (inputFile) && Directory.Exists (backupDir)) {
				try {
					string className = Path.GetFileNameWithoutExtension (inputFile);
					string backupFile = Path.Combine (backupDir, className) + ".txt";
					FileInfo sourceInfo = new FileInfo (inputFile);
					DateTime t = sourceInfo.CreationTime;
					File.Copy (inputFile, backupFile, true);
					File.SetCreationTime (backupFile, t);
					File.SetLastWriteTime (backupFile, t);
					return backupFile;
				}
				catch (System.Exception ex) {
					string msg = " threw:\n" + ex.ToString ();
					Logger.Error (msg);
				}
			}
			return "";
		}
		
	}
}

