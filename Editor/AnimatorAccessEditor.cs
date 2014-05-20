// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Scio.AnimatorWrapper {
	[CustomEditor(typeof(AnimatorAccess.AnimatorAccess))]
	public class AnimatorAccessEditor : Editor 
	{
		public override void OnInspectorGUI()
		{
			AnimatorAccess.AnimatorAccess myTarget = (AnimatorAccess.AnimatorAccess)target;
			myTarget.regenerate = EditorGUILayout.Toggle("ToggleRegenerate", myTarget.regenerate);
			if(GUILayout.Button("Regenerate")) {
				myTarget.regenerate = false;
				MenuGenerateAnimatorWrapper.GenerateAnimatorWrapper ();
			}
			EditorGUILayout.LabelField("Regenerate", myTarget.regenerate.ToString());
		}
	}
}


