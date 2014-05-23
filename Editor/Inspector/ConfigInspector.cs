// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Scio.AnimatorWrapper
{
	public class ConfigInspector : EditorWindow
	{
		static GUIContent AutoRefreshAssetDatabase = new GUIContent ("Auto Refresh AssetDatabase", "Automatically call an AssetDabase.Refresh () after updating an existing AnimatorAccess class.\n" +
			"Note that the MonoDevelop project is reloaded too which can be annoying.");
		static GUIContent ForceOverwritingOldClass = new GUIContent ("Ignore Existing Code", " Unchecked means that existing members are created once with the obsolete attribute set.\nCheck this if existing members should be removed in new code.");
		static GUIContent KeepObsolete = new GUIContent ("Keep Obsolete Members", "Obsolete properties and methods starting with 'Is' are not removed. Note that this might lead to a lot of unnecessary code.");
		static GUIContent ForceLayerPrefix = new GUIContent ("Force Layer Prefix", "Check this if you want the layer name be prepended even for layer 0.");
		static GUIContent AnimatorStatePrefix = new GUIContent ("Animator State Prefix", "Optional prefix for all methods that check animation state e.g. Is<Prefix>Idle ().");
		static GUIContent ParameterPrefix = new GUIContent ("Parameter Prefix", "Optional prefix for parameter access properties, e.g. float <Prefix>Speed.");

		public void OnGUI() {
			Config config = AnimatorWrapperConfigFactory.DefaultConfig;
			config.AutoRefreshAssetDatabase = EditorGUILayout.Toggle (AutoRefreshAssetDatabase, config.AutoRefreshAssetDatabase);
			config.KeepObsoleteMembers = EditorGUILayout.Toggle (KeepObsolete, config.KeepObsoleteMembers);
			config.ForceOverwritingOldClass = EditorGUILayout.Toggle (ForceOverwritingOldClass, config.ForceOverwritingOldClass);
			config.ForceLayerPrefix = EditorGUILayout.Toggle (ForceLayerPrefix, config.ForceLayerPrefix);
			config.AnimatorStatePrefix = EditorGUILayout.TextField (AnimatorStatePrefix, config.AnimatorStatePrefix);
			config.ParameterPrefix = EditorGUILayout.TextField (ParameterPrefix, config.ParameterPrefix);
		}

	}
}

