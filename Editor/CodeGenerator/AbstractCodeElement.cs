// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Scio.CodeGenerator
{
	public enum AccessType
	{
		Public = 3,
		Protected = 2,
		Private = 1,
	}

	public class ParameterCodeElement
	{
		public string ParameterType;
		public string Name;
		public string DefaultValue = "";

		public ParameterCodeElement (Type type, string name) {
			ParameterType = CodeElementUtils.GetFormattedType (type);
			Name = name;
		}
		public ParameterCodeElement (Type type, string name, object defaultValue) :
			this (type, name)
		{
			DefaultValue = CodeElementUtils.GetFormattedValue (defaultValue);
		}
		public override string ToString () {
			return string.Format ("{0} {1}{2}", ParameterType, Name, (DefaultValue.Length > 0 ? " = " + DefaultValue : ""));
		}
		
	}

	public abstract class AbstractCodeElement
	{
		protected AccessType Access = AccessType.Public;
		public string AccessString {
			get { return Access.ToString ().ToLower ();}
		}

		public bool Static = false;

		public List<AttributeCodeElement> Attributes = new List<AttributeCodeElement> ();
		/// <summary>
		/// Should the code have the Obsolete atrribute set.
		/// </summary>
		public bool Obsolete {
			get {return Attributes.FindIndex ((a) => a is ObsoleteAttributeCodeElement) >= 0; }
		}

		public string Name;
		/// <summary>
		/// Comments to add to summary like this one. 
		/// </summary>
		public List<string> Summary = new List<string> ();
	
		protected AbstractCodeElement (string name, AccessType access = AccessType.Public)
		{
			Name = name;
			Access = access;
		}

		public override string ToString () {
			string summaryStr = "";
			Summary.ForEach ((string s) => summaryStr += "///" + s + "\n");
			string obs = "";
			Attributes.ForEach ((AttributeCodeElement a) => obs += (obs.Length > 0 ? "\n" : "") + a);
			return string.Format ("{0}\n{1}\n{2} {3}", obs, summaryStr, AccessString, Name);
		}
		
	}
}
