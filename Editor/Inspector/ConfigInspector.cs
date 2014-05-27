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
using System.Collections;

namespace Scio.AnimatorAccessGenerator
{
	public class ConfigInspector : EditorWindow
	{
		static bool advancedSettingFoldoutState = false;

		static GUIContent AutoRefreshAssetDatabase = new GUIContent ("Auto Refresh AssetDatabase", "Automatically call an AssetDabase.Refresh () after updating an existing AnimatorAccess class.\n" +
			"Note that the MonoDevelop project is reloaded too which can be annoying.");
		static GUIContent ForceOverwritingOldClass = new GUIContent ("Ignore Existing Code", " Unchecked means that existing members are created once with the obsolete attribute set.\n" +
			"Check this if existing members should be removed immediately. Note that this option overwrites 'Keep Obsolete Members'");
		static GUIContent KeepObsolete = new GUIContent ("Keep Obsolete Members", "Obsolete properties and methods starting with 'Is' are not removed. This might lead to a lot of unnecessary code.\n" +
			"Note that this option is not considered if 'Force Layer Prefix' is set.");
		static GUIContent ForceLayerPrefix = new GUIContent ("Force Layer Prefix", "Check this if you want the layer name be prepended even for Animator states of layer 0.");
		static GUIContent AnimatorStatePrefix = new GUIContent ("Animator State Prefix", "Optional prefix for all methods that check animation state e.g. Is<Prefix>Idle ().");
		static GUIContent ParameterPrefix = new GUIContent ("Parameter Prefix", "Optional prefix for parameter access properties, e.g. float <Prefix>Speed.");
		static GUIContent DebugMode = new GUIContent ("Debug Mode", "Extended logging to console view.");

		public void OnGUI() {
			Config config = ConfigFactory.DefaultConfig;
			config.AutoRefreshAssetDatabase = EditorGUILayout.Toggle (AutoRefreshAssetDatabase, config.AutoRefreshAssetDatabase);
			EditorGUILayout.Separator ();
			config.KeepObsoleteMembers = EditorGUILayout.Toggle (KeepObsolete, config.KeepObsoleteMembers);
			config.ForceOverwritingOldClass = EditorGUILayout.Toggle (ForceOverwritingOldClass, config.ForceOverwritingOldClass);
			EditorGUILayout.Separator ();
			config.AnimatorStatePrefix = EditorGUILayout.TextField (AnimatorStatePrefix, config.AnimatorStatePrefix);
			EditorGUILayout.Separator ();
			config.ParameterPrefix = EditorGUILayout.TextField (ParameterPrefix, config.ParameterPrefix);
			config.ForceLayerPrefix = EditorGUILayout.Toggle (ForceLayerPrefix, config.ForceLayerPrefix);
			EditorGUILayout.Separator ();
			advancedSettingFoldoutState = EditorGUILayout.Foldout (advancedSettingFoldoutState, "Advanced Settings");
			if (advancedSettingFoldoutState) {
				config.DebugMode = EditorGUILayout.Toggle (DebugMode, config.DebugMode);
				if (config.DebugMode) {
					Scio.CodeGeneration.Logger.Get.CurrentLogLevel = Scio.CodeGeneration.LogLevel.Debug;
				} else {
					Scio.CodeGeneration.Logger.Get.CurrentLogLevel = Scio.CodeGeneration.LogLevel.Info;
				}
			}
		}

	}
}

