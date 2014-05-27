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
using System.Collections;
using Scio.CodeGeneration;

namespace Scio.AnimatorAccessGenerator
{
	public class UnityLogger : ILogger
	{
		LogLevel logLevel = LogLevel.Warning;
		public LogLevel CurrentLogLevel {
			get { return logLevel; }
			set { logLevel = value; }
		}

		public UnityLogger (bool debug) {
			if (debug) {
				logLevel = LogLevel.Debug;
			}
		}
		
		public void Clear () {
		}

		string MakeString (object title, object text)
		{
			return (title != null ? title.ToString () : "") + (text != null ? text.ToString () : "");
		}

		public void Error (object title, object text) {
			UnityEngine.Debug.LogError (MakeString (title, text));
		}

		public void Warning (object title, object text) {
			if (logLevel >= LogLevel.Warning) {
				UnityEngine.Debug.LogWarning (MakeString (title, text));
			}
		}

		public void Info (object title, object text) {
			if (logLevel >= LogLevel.Info) {
				UnityEngine.Debug.Log (MakeString (title, text));
			}
		}

		public void Debug (object title, object text) {
			if (logLevel >= LogLevel.Debug) {
				UnityEngine.Debug.Log (MakeString (title, text));
			}
		}

	}
}

