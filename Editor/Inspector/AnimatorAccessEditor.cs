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
using System.Collections.Generic;
using Scio.CodeGeneration;
using AnimatorAccess;

namespace Scio.AnimatorWrapper 
{
	[CustomEditor(typeof(AnimatorAccess.BaseAnimatorAccess), true)]
	public class AnimatorAccessEditor : Editor 
	{
		const string InspectorIconsDir = "/Editor/Inspector/Icons/";

		static bool updateCheckFoldOutState = true;

		static Texture iconRemove = null;
		static Texture iconAdd = null;
		static Texture iconObsolete = null;

		static AnimatorAccessEditor () {
			string dir = "Assets/" + Manager.SharedInstance.InstallDir + InspectorIconsDir;
			iconRemove = AssetDatabase.LoadAssetAtPath (dir + "icon_remove.png", typeof(Texture)) as Texture;
			iconObsolete = AssetDatabase.LoadAssetAtPath (dir + "icon_obsolete.png", typeof(Texture)) as Texture;
			iconAdd = AssetDatabase.LoadAssetAtPath (dir + "icon_add.png", typeof(Texture)) as Texture;
		}
		
		List<ClassMemberCompareElement> updateCheck = null;

		bool dirty = false;

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
			EditorGUILayout.BeginHorizontal ();
			if(GUILayout.Button("Check", EditorStyles.miniButtonLeft)) {
				updateCheck = Manager.SharedInstance.CheckForUpdates (Selection.activeGameObject);
				updateCheck.Sort ((x, y) => {
					if (x.result != y.result) {
						return x.result - y.result;
					}
					return x.member.CompareTo (y.member);
				});
			}
			if(GUILayout.Button("Update", (updateCheck != null && updateCheck.Count > 0 ? InspectorStyles.MidMiniButtonHighLighted : EditorStyles.miniButtonMid))) {
				Manager.SharedInstance.Update (Selection.activeGameObject);
				updateCheck = null;
				dirty = true;
			}
			if(GUILayout.Button("Refresh" + (dirty ? "*" : ""), (dirty ? InspectorStyles.RightMiniButtonHighLighted : EditorStyles.miniButtonRight))) {
				Manager.SharedInstance.Refresh ();
				dirty = false;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator ();
			if (updateCheck != null) {
				if (updateCheck.Count > 0) {
					EditorGUILayout.BeginVertical ();
					updateCheckFoldOutState = EditorGUILayout.Foldout (updateCheckFoldOutState, updateCheck.Count + " class members to update");
					if (updateCheckFoldOutState) {
						foreach (ClassMemberCompareElement c in updateCheck) {
							string label = string.Format ("{0}", c.member);
							Texture icon = null;
							string tooltip = "";
							switch (c.result) {
							case ClassMemberCompareElement.Result.New:
								icon = iconAdd;
								tooltip = string.Format ("{0} {1} will be added", c.memberType, c.member);
								break;
							case ClassMemberCompareElement.Result.Obsolete:
								icon = iconObsolete;
								tooltip = string.Format ("{0} {1} will be marked as obsolete", c.memberType, c.member);
								break;
							case ClassMemberCompareElement.Result.Remove:
								icon = iconRemove;
								tooltip = string.Format ("{0} {1} will be removed", c.memberType, c.member);
								break;
							default:
								break;
							}
							GUIContent content = new GUIContent (label, icon, tooltip);
							EditorGUILayout.LabelField (content);
						}
					}
					EditorGUILayout.EndVertical ();
				} else {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.LabelField (myTarget.GetType().Name + " is up to date");
					EditorGUILayout.EndVertical ();
				}
			} else {
				EditorGUILayout.BeginVertical ();
				EditorGUILayout.LabelField ("Press 'Check' to get update information about " + myTarget.GetType().Name);
				EditorGUILayout.EndVertical ();
			}
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			if (Manager.SharedInstance.HasBackup (myTarget)) {
				EditorGUILayout.LabelField ("Saved version " + Manager.SharedInstance.GetBackupTimestamp (myTarget));
				if (GUILayout.Button ("Undo")) {
					Manager.SharedInstance.Undo (myTarget);
					updateCheck = null;
					dirty = false;
				}
			} else {
				EditorGUILayout.LabelField ("No backup available");
				GUILayout.Button ("Undo", InspectorStyles.ButtonDisabled);
			}
			EditorGUILayout.EndHorizontal ();
		}
	}
}


