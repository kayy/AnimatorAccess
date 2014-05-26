// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using Scio.CodeGeneration;

namespace Scio.AnimatorWrapper 
{
	[CustomEditor(typeof(AnimatorAccess.BaseAnimatorAccess), true)]
	public class AnimatorAccessEditor : Editor 
	{
		void OnEnable () {
		}
		
		public override void OnInspectorGUI()
		{
			AnimatorAccess.BaseAnimatorAccess myTarget = (AnimatorAccess.BaseAnimatorAccess)target;
			Attribute[] attrs = Attribute.GetCustomAttributes(myTarget.GetType (), typeof (GeneratedClassAttribute), false);
			string version = "Version: ";
			if (attrs.Length == 1) {
				GeneratedClassAttribute a = (GeneratedClassAttribute)attrs[0];
				version += a.CreationDate;
			} else {
				version += " not available";
			}
			GUILayout.Label (version);
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Check For Updates")) {
				Manager.SharedInstance.CheckForUpdates (Selection.activeGameObject);
			}
			if(GUILayout.Button("Update")) {
				Manager.SharedInstance.Update (Selection.activeGameObject);
			}
			if(GUILayout.Button("Refresh")) {
				Manager.SharedInstance.Refresh ();
			}
			EditorGUILayout.EndHorizontal();
			if (Manager.SharedInstance.HasBackup (AnimatorWrapperEditorUtils.GetActiveAnimatorAccessComponent ())) {
				if (GUILayout.Button ("Undo")) {
					Manager.SharedInstance.Undo (AnimatorWrapperEditorUtils.GetActiveAnimatorAccessComponent ());
				}
			} else {
				GUILayout.Button ("Undo", InspectorStyles.buttonDisabled);
			}
		}
	}
}


