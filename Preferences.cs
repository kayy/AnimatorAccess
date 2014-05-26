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

