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
using System;
using System.IO;
using System.Reflection;
using Scio.CodeGeneration;

namespace Scio.AnimatorWrapper
{
	/// <summary>
	/// Handles post processing after AssetDatabase.Refresh. Especially after calling Create ... all assemblies are 
	/// reloaded. In order to offer adding the newly created component to the curretnly selected game object, some 
	/// special action is required in that the file name ahs to be made persistent.
	/// </summary>
	[InitializeOnLoad]
	public class EditorStatusObserver
	{
		static EditorStatusObserver ()
		{
			// ask user delayed delayed due to Unity errors when done during construction
			EditorApplication.update += OnEditorApplicationUpdate;
			if (Manager.SharedInstance == null) {
				Debug.LogError ("Could not intantiate Manager!");
			}
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
				Logger.Debug ("No game object selected");
			} else if (activeGameObject.GetComponent (t) != null) {
				Logger.Debug (activeGameObject.name + " already has a component " + t.Name + " attached");
			} else if (activeGameObject.GetComponent<Animator> () == null) {
				Logger.Debug ("No animator component, skipping");
			} else {
				if (EditorUtility.DisplayDialog ("Add Commponent", "Add " + className + " to " + activeGameObject.name + " ?", "Yes", "No")) {
					Component c = activeGameObject.AddComponent (t);
					if (c == null) {
						Logger.Warning ("Could not add component of type " + t + ".");
					}
				}
			}
		}
		
	}
}

