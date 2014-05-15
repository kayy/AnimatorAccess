// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public static class MenuGenerateAnimatorWrapper
{
	const string resourcesDir = "Scripts";
	
	/// <summary>
	/// Default name for Save As dialog.
	/// </summary>
	static string targetClassNameDefault = "AnimatorWrapper";
	
	static string targetCodeFile = targetClassNameDefault + ".cs";
	
	[MenuItem("Tools/Generate Animator Wrapper")]
	public static void GenerateAnimatorWrapper () {
		if (!DisplayFileDialog ()) {
			return;
		}
new AnimatorWrapperGenerator (Selection.activeGameObject, targetCodeFile).GetHashCode();
//		AnimatorWrapperGenerator gen = new AnimatorWrapperGenerator (Selection.activeGameObject, targetCodeFile);
//		if (gen.Generate ()) {
//			WriteCodeToFile (gen.Code);
//		}
	}
	
	static bool DisplayFileDialog () {
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
	
	static void WriteCodeToFile (string code) {
		using (StreamWriter writer = new StreamWriter (targetCodeFile, false)) {
			try {
				writer.WriteLine ("{0}", code);
				Debug.Log ("Code written to file " + targetCodeFile);
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
	
}

