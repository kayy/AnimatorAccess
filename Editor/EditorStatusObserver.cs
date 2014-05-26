// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;

namespace Scio.AnimatorWrapper
{
	[InitializeOnLoad]
	public class EditorStatusObserver
	{
		static EditorStatusObserver ()
		{
			// ask user delayed delayed due to Unity errors when done during construction
			EditorApplication.update += OnEditorApplicationUpdate;
		}
	
		static void OnEditorApplicationUpdate ()
		{
			string fullClassName = Preferences.GetString (Preferences.Key.PostProcessingFile);
			if (!string.IsNullOrEmpty (fullClassName)) {
				DisplayAddComponentDialog (fullClassName);
				Preferences.Delete (Preferences.Key.PostProcessingFile);
			}
			// avoid unnecessary upates (100/sec)
			EditorApplication.update -= OnEditorApplicationUpdate;
		}
	
		public static void CheckForAutoRefresh () {
			if (Preferences.GetBool (Preferences.Key.AutoRefreshAssetDatabase)) {
				Refresh ();
			}
		}

		public static void Refresh () {
			AssetDatabase.Refresh ();
		}

		public static void RegisterForPostProcessing (string fullClassName) {
			Preferences.SetString (Preferences.Key.PostProcessingFile, fullClassName);
			AssetDatabase.Refresh (ImportAssetOptions.Default);
		}
		
		static void DisplayAddComponentDialog (string className)
		{
			Assembly assemblyCSharp = Assembly.Load ("Assembly-CSharp");
			System.Type t = assemblyCSharp.GetType (className, false);
			if (t == null) {
				EditorUtility.DisplayDialog ("Type Not Found", "Type [" + className + "] could not be found in assembly " + assemblyCSharp.GetName () + ".", "OK");
				return;
			}
			GameObject activeGameObject = Selection.activeGameObject;
			if (activeGameObject == null) {
			} else if (activeGameObject.GetComponent (t) != null) {
				Debug.Log (activeGameObject.name + " already has a component " + t.Name + " attached");
			} else if (activeGameObject.GetComponent<Animator> () == null) {
				Debug.Log ("No animator component, skipping");
			} else {
				if (EditorUtility.DisplayDialog ("Add Commponent", "Add " + className + " to " + activeGameObject.name + " ?", "Yes", "No")) {
					Component c = activeGameObject.AddComponent (t);
					if (c == null) {
						Debug.LogWarning ("Could not add component of type " + t + ".");
					}
				}
			}
		}
		
	}
}

