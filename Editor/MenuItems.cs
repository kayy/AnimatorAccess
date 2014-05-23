// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//#define TEST_ANIMATOR_WRAPPER

using UnityEngine;
using UnityEditor;
using AnimatorAccess;

namespace Scio.AnimatorWrapper
{
	public static class MenuItems
	{
		/// <summary>
		/// The root menu where Animator Access menus are located. Change this if you wish another location.
		/// To put the items as submenu under Windows for example set "Window/Animator Access/".
		/// This doesn't work for the Component menu.
		/// </summary>
		const string RootMenu = "Animator Access/";

		const string MenuTest = RootMenu + "Test Animator Wrapper %#t";
		const string MenuCreate = RootMenu + "Create Animator Access";
		const string MenuUpdate = RootMenu + "Update Animator Access";
		const string MenuSettings = RootMenu + "Settings";
		
		/// <summary>
		/// Default name for Save As dialog.
		/// </summary>
		static string targetClassNameDefault = "AnimatorAccess";
		
		static string targetCodeFile = targetClassNameDefault + ".cs";

#if TEST_ANIMATOR_WRAPPER
		[MenuItem(MenuTest)]
		public static void TestAnimatorWrapper () {
			Manager.SharedInstance.TestAnimatorWrapper (Selection.activeGameObject);
		}

		[MenuItem(MenuTest, true)]
		public static bool ValidateTestAnimatorWrapper () {
			return Selection.activeObject != null;
		}
#endif
		[MenuItem(MenuCreate)]
		public static void CreateAnimatorAccess () {
			if (!DisplayFileDialog ()) {
				return;
			}
			Manager.SharedInstance.Create (Selection.activeGameObject, targetCodeFile);
		}

		[MenuItem(MenuUpdate)]
		public static void UpdateAnimatorAccess () {
			Manager.SharedInstance.Update (Selection.activeGameObject);
		}

		[MenuItem(MenuUpdate, true)]
		public static bool ValidateUpdateAnimatorAccess () {
			return AnimatorWrapperEditorUtils.GetActiveAnimatorAccessComponent () != null;
		}

		[MenuItem(MenuSettings)]
		public static void ShowSettings () {
			Manager.SharedInstance.ShowSettings ();
		}

		static bool DisplayFileDialog () {
			if ((Selection.gameObjects == null) || (Selection.gameObjects.Length == 0)) {
				EditorUtility.DisplayDialog ("No selection", "Please select an object to generate the animator access class for.", "OK");
				return false;
			}
			if (Selection.gameObjects.Length != 1) {
				EditorUtility.DisplayDialog ("No selection", "Please select exactly one object to generate an animator access class for. Multiple selection is not yet supported.", "OK");
				return false;
			}
			GameObject activeGameObject = Selection.activeGameObject;
			if (activeGameObject.GetComponent<Animator> () == null) {
				EditorUtility.DisplayDialog ("No Animator", "The selected a game object does not contain an animator compnent. Please select another object.", "OK");
				return false;
			}
			targetCodeFile = activeGameObject.name + targetClassNameDefault + ".cs";
			targetCodeFile = EditorUtility.SaveFilePanel ("Generate C# Code for Animator Access Class", Manager.resourcesDir, targetCodeFile, "cs");
			if (targetCodeFile == null || targetCodeFile == "") {
				return false;
			}
			return true;
		}

	}
}

