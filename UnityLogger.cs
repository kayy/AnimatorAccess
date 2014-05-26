// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections;
using Scio.CodeGeneration;

namespace Scio.AnimatorWrapper
{
	public class UnityLogger : ILogger
	{
		LogLevel logLevel = LogLevel.Warning;
		public LogLevel CurrentLogLevel {
			get { return logLevel; }
			set { logLevel = value; }
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

