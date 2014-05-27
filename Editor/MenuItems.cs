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

//#define TEST_ANIMATOR_WRAPPER

using UnityEngine;
using UnityEditor;
using AnimatorAccess;

namespace Scio.AnimatorAccessGenerator
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
		public static void TestAnimatorAccessGenerator () {
			Manager.SharedInstance.TestAnimatorAccessGenerator (Selection.activeGameObject);
		}

#endif
		[MenuItem(MenuCreate)]
		public static void CreateAnimatorAccess () {
			if (!DisplayFileDialog ()) {
				return;
			}
			Manager.SharedInstance.Create (Selection.activeGameObject, targetCodeFile);
		}

		[MenuItem(MenuCreate, true)]
		public static bool ValidateCreateAnimatorAccess () {
			return InspectorUtils.GetActiveAnimatorAccessComponent () == null;
		}

		[MenuItem(MenuUpdate)]
		public static void UpdateAnimatorAccess () {
			Manager.SharedInstance.Update (Selection.activeGameObject);
		}

		[MenuItem(MenuUpdate, true)]
		public static bool ValidateUpdateAnimatorAccess () {
			return InspectorUtils.GetActiveAnimatorAccessComponent () != null;
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

