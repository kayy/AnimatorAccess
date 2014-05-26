// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using UnityEngine;
using System;
using PlayerPrefs = PreviewLabs.AnimatorWrapper.PlayerPrefs;

namespace Scio.AnimatorWrapper
{
	/// <summary>
	/// Wrapper class for PlayerPrefs. Note that PreviewLabs.AnimatorWrapper.PlayerPrefs writes a txt file to
	/// Application.persistentDataPath
	/// </summary>
	public class Preferences
	{
		public enum Key
		{
			TemplateDir,
			BackupDir,
			PostProcessingFile,
			AutoRefreshAssetDatabase,
			ForceOverwritingOldClass,
			KeepObsoleteMembers,
			DefaultNamespace,
			MonoBehaviourComponentBaseClass,
			ForceLayerPrefix,
			AnimatorStatePrefix,
			ParameterPrefix,
			DebugMode
		}
		const string Prefix = "Scio.AnimatorWrapper.";

		public static string GetString (Key key) {
			return PlayerPrefs.GetString (Prefix + key);
		}
		
		public static string GetString (Key key, string defaultValue) {
			return PlayerPrefs.GetString (Prefix + key, defaultValue);
		}
		
		public static void SetString (Key key, string s) {
			PlayerPrefs.SetString (Prefix + key, s);
			PlayerPrefs.Flush ();
		}
		
		public static bool GetBool (Key key) {
			return PlayerPrefs.GetBool (Prefix + key);
		}
		
		public static void SetBool (Key key, bool s) {
			PlayerPrefs.SetBool (Prefix + key, s);
			PlayerPrefs.Flush ();
		}
		
		public static int GetInt (Key key) {
			return PlayerPrefs.GetInt (Prefix + key);
		}
		
		public static void SetInt (Key key, int s) {
			PlayerPrefs.SetInt (Prefix + key, s);
			PlayerPrefs.Flush ();
		}
		
		public static float GetFloat (Key key) {
			return PlayerPrefs.GetFloat (Prefix + key);
		}
		
		public static void SetFloat (Key key, float s) {
			PlayerPrefs.SetFloat (Prefix + key, s);
			PlayerPrefs.Flush ();
		}
		
		public static void Delete (Key key) {
			PlayerPrefs.DeleteKey (Prefix + key);
		}

	}
}

