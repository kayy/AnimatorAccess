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
using System.IO;
using System.Collections.Generic;
using Scio.CodeGeneration;

namespace Scio.AnimatorAccessGenerator
{
	public static class CodeGenerationUtils
	{
		public static string GenerateVariableName (string prefix, string item) {
			string varName = "";
			bool firstChar = true;
			foreach (Char c in item) {
				if (firstChar) {
					firstChar = false;
					if (Char.IsLetter (c)) {
						varName += c;
					} else {
						varName += "_";
						if (Char.IsDigit (c)) {
							varName += c;
						}
					}
				} else {
					if (Char.IsLetterOrDigit (c)) {
						varName += c;
					} else {
						varName += "_";
					}
				}
			}
			string fullName = String.IsNullOrEmpty (prefix) ? varName.Substring (0, 1).ToLower () : (prefix + varName.Substring (0, 1).ToUpper ());
			if (String.IsNullOrEmpty (prefix)) {
				fullName = varName.Substring (0, 1).ToLower ();
			} else {
				string p = prefix.Substring (0, 1).ToLower () + (prefix.Length > 1 ? prefix.Substring (1) : "");
				fullName = p + varName.Substring (0, 1).ToUpper ();
			}
			if (varName.Length > 1) {
				fullName += varName.Substring (1);
			}
			return fullName;
		}
		
		public static string GenerateStateName (string prefix, string item, string layerPrefix)
		{
			string propName = item;
			if (!string.IsNullOrEmpty (layerPrefix)) {
				int i = propName.IndexOf (layerPrefix + ".");
				if (i >= 0) {
					propName = propName.Substring (layerPrefix.Length + 1);
				} else {
					Logger.Warning ("Item [" + item + "] does not contain [" + layerPrefix + "] as prefix");
				}
			}
			return GeneratePropertyName (prefix, propName);
		}

		public static string FirstCharToLower (this string me) {
			if (!string.IsNullOrEmpty(me)) {
				string s = me.Substring (0, 1).ToLower ();
				if (me.Length > 1) {
					s += me.Substring (1);
				}
				me = s;
			}
			return me;
		}
		
		public static string FirstCharToUpper (this string me) {
			if (!string.IsNullOrEmpty(me)) {
				string s = me.Substring (0, 1).ToUpper ();
				if (me.Length > 1) {
					s += me.Substring (1);
				}
				me = s;
			}
			return me;
		}
		
		public static string GeneratePropertyName (string prefix, string item) {
			string varName = GenerateVariableName (prefix, item);
			string propName = varName.FirstCharToUpper ();
			return propName;
		}
	
		public static string GetPathToFile (string fileName, string rootDir = null) {
			string dir = (string.IsNullOrEmpty (rootDir) ? Application.dataPath : rootDir);
			string[] files = Directory.GetFiles (dir, fileName, SearchOption.AllDirectories);
			if (files.Length == 0) {
				return "";
			} else {
				return files [0];
			}
		}
	}
}

