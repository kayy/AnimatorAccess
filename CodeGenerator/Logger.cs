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
using System;
using System.Collections.Generic;

namespace Scio.CodeGeneration
{
	/// <summary>
	/// Convenience class to access the current ILogger implementation.
	/// </summary>
	public static class Logger
	{
		static ILogger logger;
		public static ILogger Get {
			get {
				if (logger == null) {
					logger = new DefaultLogger ();
				}
				return logger;
			}
		}
		public static ILogger Set {
			set { logger = value; }
		}

		public static void Error (object title, object text = null) { Get.Error (title, text); }
		public static void Warning (object title, object text = null) { Get.Warning (title, text); }
		public static void Info (object title, object text = null) { Get.Info (title, text); }
		public static void Debug (object title, object text = null) { Get.Debug (title, text); }
	}

	public enum LogLevel 
	{
		Error = 1,
		Warning = 2,
		Info = 3,
		Debug = 4,
	}

	/// <summary>
	/// Interface for logging. Implementors should output messages depending on the current logLevel.
	/// </summary>
	public interface ILogger 
	{
		LogLevel CurrentLogLevel {
			get;
			set;
		}
		/// <summary>
		/// Clears all previous log messages. Useful only when operating in internal context where no console is 
		/// available.
		/// </summary>
		void Clear ();
		/// <summary>
		/// Should always be handled.
		/// </summary>
		/// <param name="title">Title of this log entry. Should be mandatory.</param>
		/// <param name="text">Optional decription of details.</param>
		void Error (object title, object text);
		/// <summary>
		/// Handled if LogLevel is >= Error.
		/// </summary>
		/// <param name="title">Title of this log entry. Should be mandatory.</param>
		/// <param name="text">Optional decription of details.</param>
		void Warning (object title, object text);
		/// <summary>
		/// Handled if LogLevel is >= Info.
		/// </summary>
		/// <param name="title">Title of this log entry. Should be mandatory.</param>
		/// <param name="text">Optional decription of details.</param>
		void Info (object title, object text);
		/// <summary>
		/// Handled if LogLevel is >= Debug.
		/// </summary>
		/// <param name="title">Title of this log entry. Should be mandatory.</param>
		/// <param name="text">Optional decription of details.</param>
		void Debug (object title, object text);
	}

	/// <summary>
	/// Default logger that just adds all entries to list and removes very old entries.
	/// </summary>
	public class DefaultLogger : ILogger
	{
		const int MaxEntries = 20;

		List<CodeGeneratorResult> entries = new List<CodeGeneratorResult> ();

		LogLevel logLevel = LogLevel.Warning;
		public LogLevel CurrentLogLevel {
			get { return logLevel; }
			set { logLevel = value; }
		}

		public virtual void Clear () {
			entries.Clear ();
		}
		
		public virtual void Error (object title, object text = null) {
			Add (new CodeGeneratorResult ().SetError (title, text));
		}

		void Add (CodeGeneratorResult r) {
			if (entries.Count >= 20) {
				entries.RemoveAt (entries.Count -1);
			}
			entries.Insert (0, r);
		}
		
		public virtual void Warning (object title, object text = null) {
			if (logLevel >= LogLevel.Warning) {
				Add (new CodeGeneratorResult ().SetWarning (title, text));
			}
		}
		
		public virtual void Info (object title, object text = null) {
			if (logLevel >= LogLevel.Info) {
				Add (new CodeGeneratorResult ().SetSuccess (title, text));
			}
		}

		public virtual void Debug (object title, object text = null) {
			if (logLevel >= LogLevel.Debug) {
				Add (new CodeGeneratorResult ().SetSuccess (title, text));
			}
		}

	}
	

}