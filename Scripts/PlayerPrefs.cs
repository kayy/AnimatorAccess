/*
	PreviewLabs.PlayerPrefs

	Public Domain
	
	To the extent possible under law, PreviewLabs has waived all copyright and related or neighboring rights to this document. This work is published from: Belgium.
	
	http://www.previewlabs.com/writing-playerprefs-fast/
	
*/

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PreviewLabs.AnimatorAccessGenerator
{
	/// <summary>
	/// Derived from PreviewLabs PlayerPrefs (s. note at the beginning of the file). Writes key value pairs in a text
	/// file in <Application.persistentDataPath>/AnimatorAccessConfig.txt. Keys are sorted in alpha numeric order.
	/// </summary>
	public static class PlayerPrefs
	{
		public static bool fileExists = false;
		
		private static SortedDictionary<string, object> playerPrefsDict = new SortedDictionary<string, object> ();
		
		private static bool hashTableChanged = false;
		private static string serializedOutput = "";
		private static string serializedInput = "";
		
		private const string PARAMETERS_SEPERATOR = "\n";
		private const string KEY_VALUE_SEPERATOR = ":";
		
		public static readonly string fileName = Application.persistentDataPath + "/AnimatorAccessConfig.txt";
		
		static bool ReadFile () {
			StreamReader fileReader = null;
			if (File.Exists (fileName)) {
				fileExists = true;
				fileReader = new StreamReader (fileName);
				serializedInput = fileReader.ReadToEnd ();
				fileReader.Close ();
				return true;
			} else {
				return false;
			}
		}
		
		static PlayerPrefs () {
			if (ReadFile ()) {
				Deserialize ();
			}
		}
		
		public static bool HasKey(string key)
		{			
			return playerPrefsDict.ContainsKey(key);
		}
		
		public static void SetString(string key, string value)
		{
			if(!playerPrefsDict.ContainsKey(key))
			{
				playerPrefsDict.Add(key, value);
			}
			else
			{
				playerPrefsDict[key] = value;
			}
			
			hashTableChanged = true;
		}
		
		public static void SetInt(string key, int value)
		{
			if(!playerPrefsDict.ContainsKey(key))
			{
				playerPrefsDict.Add(key, value);
			}
			else
			{
				playerPrefsDict[key] = value;
			}
			
			hashTableChanged = true;
		}
		
		public static void SetFloat(string key, float value)
		{
			if(!playerPrefsDict.ContainsKey(key))
			{
				playerPrefsDict.Add(key, value);
			}
			else
			{
				playerPrefsDict[key] = value;
			}
			
			hashTableChanged = true;
		}
		
		public static void SetBool(string key, bool value)
		{
			if(!playerPrefsDict.ContainsKey(key))
			{
				playerPrefsDict.Add(key, value);
			}
			else
			{
				playerPrefsDict[key] = value;
			}
			
			hashTableChanged = true;
		}
		
		public static string GetString(string key)
		{			
			if(playerPrefsDict.ContainsKey(key))
			{
				return playerPrefsDict[key].ToString();
			}
			
			return null;
		}
		
		public static string GetString(string key, string defaultValue)
		{
			if(playerPrefsDict.ContainsKey(key))
			{
				return playerPrefsDict[key].ToString();
			}
			else
			{
				playerPrefsDict.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}
		
		public static int GetInt(string key)
		{			
			if(playerPrefsDict.ContainsKey(key))
			{
				return (int) playerPrefsDict[key];
			}
			
			return 0;
		}
		
		public static int GetInt(string key, int defaultValue)
		{
			if(playerPrefsDict.ContainsKey(key))
			{
				return (int) playerPrefsDict[key];
			}
			else
			{
				playerPrefsDict.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}
		
		public static float GetFloat(string key)
		{			
			if(playerPrefsDict.ContainsKey(key))
			{
				return (float) playerPrefsDict[key];
			}
			
			return 0.0f;
		}
		
		public static float GetFloat(string key, float defaultValue)
		{
			if(playerPrefsDict.ContainsKey(key))
			{
				return (float) playerPrefsDict[key];
			}
			else
			{
				playerPrefsDict.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}
		
		public static bool GetBool(string key)
		{			
			if(playerPrefsDict.ContainsKey(key))
			{
				return (bool) playerPrefsDict[key];
			}
			
			return false;
		}
		
		public static bool GetBool(string key, bool defaultValue)
		{
			if(playerPrefsDict.ContainsKey(key))
			{
				return (bool) playerPrefsDict[key];
			}
			else
			{
				playerPrefsDict.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}
		
		public static void DeleteKey(string key)
		{
			playerPrefsDict.Remove(key);
			hashTableChanged = true;
		}
		
		public static void DeleteAll()
		{
			playerPrefsDict.Clear();
			hashTableChanged = true;
		}
		
		public static void Flush()
		{
			if(hashTableChanged)
			{
				Serialize();
				
				StreamWriter fileWriter = null;
				fileWriter = File.CreateText(fileName);
			
				if (fileWriter == null)
				{ 
					Debug.LogWarning("PlayerPrefs::Flush() opening file for writing failed: " + fileName);
				}
				
				fileWriter.Write(serializedOutput);
				
				fileWriter.Close();

				serializedOutput = "";
			}
		}
		
		private static void Serialize () {
		
			int i = 0;
			foreach (KeyValuePair<string, object> myEnumerator in playerPrefsDict) {
				i++;
				if (serializedOutput != "") {
					serializedOutput += PARAMETERS_SEPERATOR;
				}
				if (myEnumerator.Key == null || myEnumerator.Value == null) {
					Debug.LogWarning ("Skipping line " + i + " with undefined value for [" + myEnumerator.Key + "=" + myEnumerator.Value + "]");
					continue;
				}
				serializedOutput += EscapeNonSeperators (myEnumerator.Key.ToString ()) + " " + KEY_VALUE_SEPERATOR + " " + EscapeNonSeperators (myEnumerator.Value.ToString ()) + " " + KEY_VALUE_SEPERATOR + " " + myEnumerator.Value.GetType ();
				
			}
		}
		
		private static void Deserialize () {
			string parametersSeparator = PARAMETERS_SEPERATOR;
			string[] parameters = serializedInput.Split (new string[] {parametersSeparator}, StringSplitOptions.None);
			int i = 0;
			string previous = "";
			foreach (string parameter in parameters) {
				i++;
				if (parameter.Length <= 3) {
					// ignore 
					continue;
				}
				string[] parameterContent = parameter.Split (new string[]{" " + KEY_VALUE_SEPERATOR + " "}, StringSplitOptions.None);
				if (parameterContent.Length != 3) {
					Debug.LogWarning (i + " (after " + previous + "): PlayerPrefs.Deserialize() parameterContent has " + parameterContent.Length + " elements ! " + 
						(parameterContent.Length >= 1 ? parameterContent [0] : "(no name read)"));
				} else {
					playerPrefsDict.Add (DeEscapeNonSeperators (parameterContent [0], parametersSeparator), 
						GetTypeValue (parameterContent [2], DeEscapeNonSeperators (parameterContent [1], parametersSeparator)));
					previous = parameterContent [0];
				}
			}
		}
		
		private static string EscapeNonSeperators(string inputToEscape)
		{
			inputToEscape = inputToEscape.Replace(KEY_VALUE_SEPERATOR,"\\" + KEY_VALUE_SEPERATOR);
			inputToEscape = inputToEscape.Replace(PARAMETERS_SEPERATOR,"\\" + PARAMETERS_SEPERATOR);
			return inputToEscape;
		}
		
		private static string DeEscapeNonSeperators(string inputToDeEscape, string parametersSeparator)
		{
			inputToDeEscape = inputToDeEscape.Replace("\\" + KEY_VALUE_SEPERATOR, KEY_VALUE_SEPERATOR);
			inputToDeEscape = inputToDeEscape.Replace("\\" + parametersSeparator, parametersSeparator);
			return inputToDeEscape;
		}
		
		public static object GetTypeValue(string typeName, string value)
		{
			if (typeName == "System.String")
			{
				return (object)value.ToString();
			}
			if (typeName == "System.Int32")
			{
				return (object)System.Convert.ToInt32(value);
			}
			if (typeName == "System.Boolean")
			{
				return (object)System.Convert.ToBoolean(value);
			}
			if (typeName == "System.Single")// -> single = float
			{
				return (object)System.Convert.ToSingle(value);
			}
			else
			{
				Debug.LogError("Unsupported type: " + typeName + " value: " + value);
			}	
			
			return null;
		}
	}	
}
