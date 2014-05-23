// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Scio.CodeGeneration;
using AnimatorAccess;

using System.Reflection;

namespace Scio.AnimatorWrapper
{
	public class Manager
	{
		public const string resourcesDir = "Scripts";
			
		static Manager instance = null;
		public static Manager SharedInstance {
			get {
				if (instance == null) {
					instance = new Manager ();
					instance.repository.Prepare ();
				}
				return instance;
			}
		}

		MetaInfoRepository repository = new MetaInfoRepository ();

		Manager () {}
		
		public void TestAnimatorWrapper (GameObject go) {
			AnimatorWrapperGenerator a = new AnimatorWrapperGenerator (go);
			CodeGeneratorResult r = a.Prepare (true);
			if (!r.Error) {
				r = a.GenerateCode ();
				if (r.Success) {
					WriteToFile (a.Code, "/Users/kay/tmp/TimeMachine.ignore/Trash/New.cs");
				}
			} else {
				Debug.Log (r);
			}
		}
		
		public void Create (GameObject go, string targetCodeFile) {
			AnimatorWrapperGenerator gen = new AnimatorWrapperGenerator (go, targetCodeFile);
			CodeGeneratorResult result = gen.Prepare (false);
			if (result.NoSuccess) {
				if (result.AskUser) {
					if (!EditorUtility.DisplayDialog (result.ErrorTitle, result.ErrorText, "OK", "Cancel")) {
						return;
					}
				}
				else {
					EditorUtility.DisplayDialog (result.ErrorTitle, result.ErrorText, "OK");
					return;
				}
			}
			result = gen.GenerateCode ();
			if (result.Success) {
				BackupAndSave (gen.Code, targetCodeFile);
				EditorStatusObserver.RegisterForPostProcessing (gen.FullClassName);
			}
		}

		public void Update (GameObject go) {
			string file = GetTargetFile (go);
			if (string.IsNullOrEmpty (file)) {
				return;
			}
			AnimatorWrapperGenerator a = new AnimatorWrapperGenerator (go);
			CodeGeneratorResult r = a.Prepare (true);
			if (!r.Error) {
				r = a.GenerateCode ();
				if (r.Success) {
					BackupAndSave (a.Code, file);
					EditorStatusObserver.Refresh ();
				}
			} else {
				Debug.Log (r);
			}
		}

		public void CheckForUpdates (GameObject activeGameObject) {
			throw new System.NotImplementedException ();
		}
		
		public void Undo (BaseAnimatorAccess component) {
			if (HasBackup (component)) {
				string backupFile = repository.RemoveBackup (component);
				string file = repository.GetFile (component);
				try {
					File.Copy (backupFile, file, true);
					File.Delete (backupFile);
					EditorStatusObserver.Refresh ();
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

		public void ShowSettings ()
		{
			ConfigInspector window = EditorWindow.GetWindow<ConfigInspector> ("Generator Conf.");
			window.ShowPopup ();
		}

		void WriteToFile (string code, string file) {
			using (StreamWriter writer = new StreamWriter (file, false)) {
				try {
					writer.WriteLine ("{0}", code);
					Debug.Log ("Code written to file " + file);
					return;
				}
				catch (System.Exception ex) {
					string msg = " threw:\n" + ex.ToString ();
					Debug.LogError (msg);
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

