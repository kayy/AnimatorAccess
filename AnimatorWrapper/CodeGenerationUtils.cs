// The MIT License (MIT)
// 
// Copyright (c) 2014 kayy
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
using System.Collections.Generic;

public class CodeGenerationUtils
{
	public string Code (int tabs, string code, int noOfReturns = 1) {
		return Line (tabs, code + ";", noOfReturns);
	}
	
	public string Line (int tabs, string code, int noOfReturns = 1) {
		string indent = "";
		for (int i = 0; i < tabs; i++) {
			indent += "\t";
		}
		string CRs = "";
		for (int i = 0; i < noOfReturns; i++) {
			CRs += "\n";
		}
		return indent + code + CRs;
	}
	
	public string MakeComment (int tabs, string comment) {
		string formatted = Line (tabs, "/// <summary>");
		List<string> lines = Wrap (comment, 120 - tabs * 4 - 4);
		foreach (string line in lines) {
			formatted += Line (tabs, "/// " + line);
		}
		return formatted + Line (tabs, "/// </summary>");
	}
	
	public List<string> Wrap(string text, int maxLength) {              
		if (text.Length == 0) {
			return new List<string>();
		}
		char[] delimiters = new char[] {' ', '\n', '\t'};
		string[] words = text.Split(delimiters);
		List<string> lines = new List<string>();
		string currentLine = "";
		foreach (string currentWord in words)
		{
			if ((currentLine.Length > maxLength) ||
			    ((currentLine.Length + currentWord.Length) > maxLength))
			{
				lines.Add(currentLine);
				currentLine = "";
			}
			if (currentLine.Length > 0)
				currentLine += " " + currentWord;
			else
				currentLine += currentWord;
		}
		if (currentLine.Length > 0) {          
			lines.Add(currentLine);
		}
		return lines;
	}

	public string GenerateVariableName (string prefix, string item) {
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
		string fullName = String.IsNullOrEmpty(prefix) ? varName.Substring (0, 1).ToLower () : (prefix + varName.Substring (0, 1).ToUpper ());
		if (String.IsNullOrEmpty(prefix)) {
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
	
	public string GeneratePropertyName (string prefix, string item) {
		string varName = GenerateVariableName (prefix, item);
		string propName = varName.Substring (0,1).ToUpper ();
		if (varName.Length > 1) {
			propName += varName.Substring (1);
		}
		return propName;
	}
	
	public string GenerateMITHeader (string userSelection) {
		string classPrefixCode = Line (0, "//  IMPORTANT: GENERATED CODE  ==>  DO NOT EDIT!");
		classPrefixCode += Line (0, "//  Generated at " + System.DateTime.Now + ". object for generation was " + userSelection);
		classPrefixCode += Line (0, "//");
		classPrefixCode += Line (0, "//  Permission is hereby granted, free of charge, to any person obtaining a copy");
		classPrefixCode += Line (0, "//  of this software and associated documentation files (the \"Software\"), to deal");
		classPrefixCode += Line (0, "//  in the Software without restriction, including without limitation the rights");
		classPrefixCode += Line (0, "//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell");
		classPrefixCode += Line (0, "//  copies of the Software, and to permit persons to whom the Software is");
		classPrefixCode += Line (0, "//  furnished to do so, subject to the following conditions:");
		classPrefixCode += Line (0, "//  ");
		classPrefixCode += Line (0, "//  The above copyright notice and this permission notice shall be included in");
		classPrefixCode += Line (0, "//  all copies or substantial portions of the Software.");
		classPrefixCode += Line (0, "//  ");
		classPrefixCode += Line (0, "//  THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR");
		classPrefixCode += Line (0, "//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,");
		classPrefixCode += Line (0, "//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE");
		classPrefixCode += Line (0, "//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER");
		classPrefixCode += Line (0, "//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,");
		classPrefixCode += Line (0, "//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN");
		classPrefixCode += Line (0, "//  THE SOFTWARE.", 2);
		return classPrefixCode;
	}
	
}

