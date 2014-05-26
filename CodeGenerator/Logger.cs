// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections.Generic;

namespace Scio.CodeGeneration
{
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

	public interface ILogger 
	{
		LogLevel CurrentLogLevel {
			get;
			set;
		}
		void Clear ();
		void Error (object title, object text);
		void Warning (object title, object text);
		void Info (object title, object text);
		void Debug (object title, object text);
	}
	
	public class DefaultLogger : ILogger
	{
		List<CodeGeneratorResult> result = new List<CodeGeneratorResult> ();

		LogLevel logLevel = LogLevel.Warning;
		public LogLevel CurrentLogLevel {
			get { return logLevel; }
			set { logLevel = value; }
		}

		public virtual void Clear () {
			result.Clear ();
		}
		
		public virtual void Error (object title, object text = null) {
			result.Insert (0, new CodeGeneratorResult ().SetError (title, text));
		}
		
		public virtual void Warning (object title, object text = null) {
			if (logLevel >= LogLevel.Warning) {
				result.Insert (0, new CodeGeneratorResult ().SetWarning (title, text));
			}
		}
		
		public virtual void Info (object title, object text = null) {
			if (logLevel >= LogLevel.Info) {
				result.Insert (0, new CodeGeneratorResult ().SetSuccess (title, text));
			}
		}

		public virtual void Debug (object title, object text = null) {
			if (logLevel >= LogLevel.Debug) {
				result.Insert (0, new CodeGeneratorResult ().SetSuccess (title, text));
			}
		}

	}
	

}