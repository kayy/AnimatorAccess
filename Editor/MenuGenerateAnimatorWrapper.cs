// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using Scio.CodeGeneration;

namespace Scio.AnimatorWrapper
{
	public static class MenuGenerateAnimatorWrapper
	{
		const string resourcesDir = "Scripts";
		
		/// <summary>
		/// Default name for Save As dialog.
		/// </summary>
		static string targetClassNameDefault = "AnimatorWrapper";
		
		static string targetCodeFile = targetClassNameDefault + ".cs";

		static string backupLastCode = "";
	
		[MenuItem("Tools/Test Animator Wrapper")]
		public static void TestAnimatorWrapper ()
		{
			// FIXME_kay: remove test code
			targetCodeFile = "/Users/kay/Development/git/SpiceInvaders/SpiceInvaders/Assets/Scripts/Enemy/Generated/UfoAnimatorWrapper.cs";
			AnimatorWrapperGenerator a = new AnimatorWrapperGenerator (Selection.activeGameObject, targetCodeFile);
			CodeGeneratorResult r = a.Prepare ();
			if (!r.Error) {
				r = a.GenerateCode ();
				if (r.Success) {
					WriteCodeToFile (a.Code, true, "/Users/kay/tmp/TimeMachine.ignore/Trash/New.cs");
				}
			} else {
				Debug.Log (r);
			}
		}
	
		[MenuItem("Tools/Generate Animator Wrapper")]
		public static void GenerateAnimatorWrapper ()
		{
			if (!DisplayFileDialog ()) {
				return;
			}
			AnimatorWrapperGenerator gen = new AnimatorWrapperGenerator (Selection.activeGameObject, targetCodeFile);
			CodeGeneratorResult result = gen.Prepare ();
			if (result.NoSuccess) {
				if (result.AskUser) {
					if (!EditorUtility.DisplayDialog (result.ErrorTitle, result.ErrorText, "OK", "Cancel")) {
						return;
					}
				} else {
					EditorUtility.DisplayDialog (result.ErrorTitle, result.ErrorText, "OK");
					return;
				}
			}
			result = gen.GenerateCode ();
			if (result.Success) {
				WriteCodeToFile (gen.Code, true);
			}
		}
		
		[MenuItem("Tools/Undo Last Generation")]
		public static void UndoLastGeneration () {
			if (!string.IsNullOrEmpty (targetCodeFile)) {
				if (!string.IsNullOrEmpty (backupLastCode)) {
					string s = backupLastCode;
					WriteCodeToFile (s, false);
				} else {
					Debug.LogWarning ("No code backup found.");
				}
			} else {
				Debug.LogWarning ("No target file for undo found.");
			}
		}

		static bool DisplayFileDialog ()
		{
			if ((Selection.gameObjects == null) || (Selection.gameObjects.Length == 0)) {
				EditorUtility.DisplayDialog ("No selection", "Please select an object to generate an animator wrapper class for.", "OK");
				return false;
			}
			if (Selection.gameObjects.Length != 1) {
				EditorUtility.DisplayDialog ("No selection", "Please select exactly one object to generate an animator wrapper class for. Multiple selection is not yet supported.", "OK");
				return false;
			}
			GameObject activeGameObject = Selection.activeGameObject;
			if (activeGameObject.GetComponent<Animator> () == null) {
				EditorUtility.DisplayDialog ("No Animator", "Please a game object that contains an animator compnent.", "OK");
				return false;
			}
			targetCodeFile = activeGameObject.name + targetClassNameDefault + ".cs";
			targetCodeFile = EditorUtility.SaveFilePanel ("Generate C# Code for Animator Wrapper Class", resourcesDir, targetCodeFile, "cs");
			if (targetCodeFile == null || targetCodeFile == "") {
				return false;
			}
			return true;
		}
		
		static void WriteCodeToFile (string code, bool saveBackup, string alternateFile = null) {
			string file = (alternateFile != null ? alternateFile : targetCodeFile);
			if (saveBackup && File.Exists (file)) {
				 SaveBackupCode (file);

			}
			using (StreamWriter writer = new StreamWriter (file, false)) {
				try {
					writer.WriteLine ("{0}", code);
					Debug.Log ("Code written to file " + file);
					// both methods trigger occasionally an IOException: Sharing violation on path ...
					//				EditorApplication.ExecuteMenuItem ("Assets/Sync MonoDevelop Project");
					//				AssetDatabase.Refresh (ImportAssetOptions.Default);
					return;
				} catch (System.Exception ex) {
					string msg = " threw:\n" + ex.ToString ();
					Debug.LogError (msg);
					EditorUtility.DisplayDialog ("Error on export", msg, "OK");
				}
			}
		}
		
		static void SaveBackupCode (string file) {
			using (StreamReader reader = new StreamReader (file)) {
				try {
					backupLastCode = reader.ReadToEnd ();
					Debug.Log ("Last Code: " + backupLastCode);
					// both methods trigger occasionally an IOException: Sharing violation on path ...
					//				EditorApplication.ExecuteMenuItem ("Assets/Sync MonoDevelop Project");
					//				AssetDatabase.Refresh (ImportAssetOptions.Default);
					return;
				}
				catch (System.Exception ex) {
					string msg = " threw:\n" + ex.ToString ();
					Debug.LogError (msg);
				}
			}
		}
	}
}

