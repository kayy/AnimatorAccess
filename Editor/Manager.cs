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
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Scio.CodeGeneration;
using AnimatorAccess;

using System.Reflection;

namespace Scio.AnimatorAccessGenerator
{
	/// <summary>
	/// Main façade for accessing all features of AnimatorAccessGenerator.
	/// </summary>
	public class Manager
	{
		public const string resourcesDir = "Scripts";

		const string baseAnimatorAccessCS = "BaseAnimatorAccess.cs";
			
		static Manager instance = null;
		public static Manager SharedInstance {
			get {
				if (instance == null) {
					instance = new Manager ();
					instance.repository.Prepare ();
					string[] files = Directory.GetFiles (Application.dataPath, baseAnimatorAccessCS, SearchOption.AllDirectories);
					if (files.Length != 1) {
						Debug.LogError ("Install directory not found! File " + baseAnimatorAccessCS + " could not be found anywhere under your Assets directory.");
						instance.InstallDir = Application.dataPath;
					} else {
						string s = Path.GetDirectoryName (files[0]);
						string appDataPath = Application.dataPath;
						instance.InstallDir = s.Substring (appDataPath.Length + 1);
					}
					bool logLevel = Preferences.GetBool (Preferences.Key.DebugMode);
					Logger.Set = new UnityLogger (logLevel);
				}
				return instance;
			}
		}

		public string InstallDir { get; private set; }

		MetaInfoRepository repository = new MetaInfoRepository ();

		Manager () {}
		
		public void TestAnimatorAccessGenerator (GameObject go) {
			ClassElementsBuilder a = new ClassElementsBuilder (go);
			CodeGeneratorResult r = a.PrepareCodeGeneration (true);
			if (!r.Error) {
				r = a.GenerateCode ();
				if (r.Success) {
					WriteToFile (a.Code, "/Users/kay/tmp/TimeMachine.ignore/Trash/New.cs");
				}
			} else {
				Debug.Log (r);
			}
		}

		/// <summary>
		/// Create a new AnimatorAccess component for the specified game object and saves it to targetCodeFile. The 
		/// caller is responsible for ensuring that there is not yet a component existing with the same name.
		/// </summary>
		/// <param name="go">Go.</param>
		/// <param name="targetCodeFile">Target code file.</param>
		public void Create (GameObject go, string targetCodeFile) {
			ClassElementsBuilder gen = new ClassElementsBuilder (go, targetCodeFile);
			CodeGeneratorResult result = gen.PrepareCodeGeneration (false);
			if (result.NoSuccess) {
				EditorUtility.DisplayDialog (result.ErrorTitle, result.ErrorText, "OK");
				return;
			}
			result = gen.GenerateCode ();
			if (result.Success) {
				BackupAndSave (gen.Code, targetCodeFile);
				EditorStatusObserver.RegisterForPostProcessing (gen.FullClassName);
			}
		}

		/// <summary>
		/// Updates the AnimatorAccess component of the specified game object.
		/// </summary>
		/// <param name="go">Go.</param>
		public void Update (GameObject go) {
			string file = GetTargetFile (go);
			if (string.IsNullOrEmpty (file)) {
				return;
			}
			ClassElementsBuilder a = new ClassElementsBuilder (go);
			CodeGeneratorResult r = a.PrepareCodeGeneration (true);
			if (!r.Error) {
				r = a.GenerateCode ();
				if (r.Success) {
					BackupAndSave (a.Code, file);
					EditorStatusObserver.CheckForAutoRefresh ();
				}
			} else {
				Logger.Debug (r);
				EditorUtility.DisplayDialog (r.ErrorTitle, r.ErrorText, "OK");
			}
		}

		public List<ClassMemberCompareElement> CheckForUpdates (GameObject go) {
			ClassElementsBuilder a = new ClassElementsBuilder (go);
			return a.Compare (go);
		}

		public void Refresh () {
			EditorStatusObserver.Refresh ();
		}

		/// <summary>
		/// Restore the current AnimatorAcces component, if there is a backup available.
		/// </summary>
		/// <param name="component">Component.</param>
		public void Undo (BaseAnimatorAccess component) {
			if (HasBackup (component)) {
				string backupFile = repository.RemoveBackup (component);
				string file = repository.GetFile (component);
				try {
					FileInfo sourceInfo = new FileInfo (backupFile);
					System.DateTime t = sourceInfo.CreationTime;
					File.Copy (backupFile, file, true);
					File.SetCreationTime (file, t);
					File.SetLastWriteTime (file, t);
					File.Delete (backupFile);
					EditorStatusObserver.CheckForAutoRefresh ();
				} catch (System.Exception ex) {
					Debug.LogWarning (ex.Message);
				}
			} else {
				Debug.LogWarning ("No target file for undo found.");
			}
		}

		public bool HasBackup (BaseAnimatorAccess component) {
			return repository.HasBackup (component);
		}

		public string GetBackupTimestamp (BaseAnimatorAccess component) {
			return repository.GetBackupTimestamp (component);
		}

		public void ShowSettings ()
		{
			ConfigInspector window = EditorWindow.GetWindow<ConfigInspector> ("Generator Conf.");
			window.ShowPopup ();
		}

		void WriteToFile (string code, string file) {
			using (StreamWriter writer = new StreamWriter (file, false)) {
				try {
					writer.WriteLine ("{0}", code);
					Logger.Debug ("Code written to file " + file);
					return;
				}
				catch (System.Exception ex) {
					string msg = " threw:\n" + ex.ToString ();
					Logger.Error (msg);
					EditorUtility.DisplayDialog ("Error on export", msg, "OK");
				}
			}
		}
		
		void BackupAndSave (string code, string file) {
			MakeBackup (file);
			WriteToFile (code, file);
		}
		
		void MakeBackup (string file) {
			if (!File.Exists (file)) {
				return;
			}
			string className = Path.GetFileNameWithoutExtension (file);
			repository.MakeBackup (className, file);
		}

		string GetTargetFile (GameObject go) {
			BaseAnimatorAccess a = go.GetComponent<BaseAnimatorAccess> ();
			string targetCodeFile = a.GetType ().Name + ".cs";
			string[] files = Directory.GetFiles (Application.dataPath, targetCodeFile, SearchOption.AllDirectories);
			if (files.Length > 1 || files.Length == 0) {
				targetCodeFile = EditorUtility.SaveFilePanel (files.Length + " target file(s) found. Please select", resourcesDir, targetCodeFile, "cs");
				if (targetCodeFile == null || targetCodeFile == "") {
					return "";
				}
			} else {
				targetCodeFile = files [0];
			}
			return targetCodeFile;
		}
	}

}

