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
	/// <summary>
	/// Config inspector for Settings window.
	/// </summary>
	public class ConfigInspector : EditorWindow
	{
		static bool advancedSettingFoldoutState = false;

		static GUIContent AutoRefreshAssetDatabase = new GUIContent ("Auto Refresh AssetDatabase", "Automatically call an AssetDabase.Refresh () after updating an existing AnimatorAccess class.\n" +
			"Note that the MonoDevelop project is reloaded too which can be annoying.");
		static GUIContent AutoRefreshInterval = new GUIContent ("Auto Refresh Interval", "Automatically check for updates after this interval (in seconds) has elapsed.\nSet this to 0 to suppress automatic checking.");
		static GUIContent GenerateStateEventHandler = new GUIContent ("State Event Handler", "Select if and where to generate the code for automatic event handling and invoking of callbacks on state changes:\n" +
			"FixedUpdate: Check for state changes and transitions is performed in FixedUpdate\n" +
			"Update: Same for Update method\n" +
			"None: you have to manually invoke method CheckForAnimatorStateChanges () of the generated class.");
		static GUIContent IgnoreExistingCode = new GUIContent ("Ignore Existing Code", " Unchecked (default) means that the current version of the class is analysed first. Existing members that are not valid any longer are created once but with the obsolete attribute set.\n\n" +
			"So you can replace references to these outdated members in your code. Performing another generation will then remove all obsolete members.\n" +
			"Check this if existing members should be removed immediately.\n\nNote that this option overwrites 'Keep Obsolete Members'");
		static GUIContent KeepObsolete = new GUIContent ("Keep Obsolete Members", "Obsolete fields and methods are not removed. This might lead to a lot of unnecessary code.\n" +
			"Note that this option is not considered if 'Ignore Existing Code' is set.");
		static GUIContent ForceLayerPrefix = new GUIContent ("Force Layer Prefix", "Check this if you want the layer name be prepended even for Animator states of layer 0.");
		static GUIContent AnimatorStatePrefix = new GUIContent ("Animator State Prefix", "Optional prefix for all methods that check animation state. Prefix is set betwen 'Is' and the state name.\nExample:\n'State' will generate the method 'bool IsStateIdle ()' for state Idle.");
		static GUIContent AnimatorStateHashPrefix = new GUIContent ("Animator State Hash Prefix", "Optional prefix for all int fields representing an animator state.\nExample:\n'stateHash' will generate the field 'int stateHashIdle' for state 'Idle'.");
		static GUIContent ParameterPrefix = new GUIContent ("Parameter Prefix", "Optional prefix for parameter access methods. Prefix is set betwen 'Get'/'Set' and the parameter name.\nExample:\n'Param' will generate the method 'float SetParamSpeed ()' for parameter 'Speed'.");
		static GUIContent ParameterHashPrefix = new GUIContent ("Parameter Hash Prefix", "Optional prefix for int fields representing a parameter.\nExample:\n'paramHash' will generate the field 'float paramHashSpeed' for parameter 'Speed'.");
		static GUIContent GenerateNameDictionary = new GUIContent ("Generate Name Dictionary", "Create an hash ID to plain name dictionary. If true, a method IdToName (int id) is added to look up the full name of Animator states, ... by their hash IDs.");

		static GUIContent DebugMode = new GUIContent ("Debug Mode", "Extended logging to console view.");

		public void OnGUI() {
			Config config = ConfigFactory.DefaultConfig;
			config.AutoRefreshAssetDatabase = EditorGUILayout.Toggle (AutoRefreshAssetDatabase, config.AutoRefreshAssetDatabase);
			EditorGUILayout.Separator ();
			config.AutoRefreshInterval = EditorGUILayout.IntField (AutoRefreshInterval, config.AutoRefreshInterval);
			EditorGUILayout.Separator ();
			config.GenerateStateEventHandler = (StateEventHandlingMethod)EditorGUILayout.EnumPopup (GenerateStateEventHandler, config.GenerateStateEventHandler);
			EditorGUILayout.Separator ();
			config.IgnoreExistingCode = EditorGUILayout.Toggle (IgnoreExistingCode, config.IgnoreExistingCode);
			config.KeepObsoleteMembers = EditorGUILayout.Toggle (KeepObsolete, config.KeepObsoleteMembers);
			EditorGUILayout.Separator ();
			config.AnimatorStatePrefix = EditorGUILayout.TextField (AnimatorStatePrefix, config.AnimatorStatePrefix);
			config.AnimatorStateHashPrefix = EditorGUILayout.TextField (AnimatorStateHashPrefix, config.AnimatorStateHashPrefix);
			EditorGUILayout.Separator ();
			config.ParameterPrefix = EditorGUILayout.TextField (ParameterPrefix, config.ParameterPrefix);
			config.ParameterHashPrefix = EditorGUILayout.TextField (ParameterHashPrefix, config.ParameterHashPrefix);
			config.ForceLayerPrefix = EditorGUILayout.Toggle (ForceLayerPrefix, config.ForceLayerPrefix);
			EditorGUILayout.Separator ();
			config.GenerateNameDictionary = EditorGUILayout.Toggle (GenerateNameDictionary, config.GenerateNameDictionary);
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

