// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Scio.CodeGenerator
{
	public abstract class AbstractCodeElement
	{
		protected static string GetFormattedType (Type elementType) {
			if (elementType == typeof(bool)) {
				return "bool";
			} else if (elementType == typeof(int)) {
				return "int";
			} else if (elementType == typeof(float)) {
				return "float";
			} else if (elementType == typeof(double)) {
				return "double";
			} else if (elementType == typeof(string)) {
				return "string";
			}
			return elementType.Name;
		}

		public enum AccessType
		{
			Public,
			Protected,
			Private,
		}
		protected AccessType accessType = AccessType.Public;
		public string Access {
			get { return accessType.ToString ().ToLower ();}
		}

		public bool Static = false;

		/// <summary>
		/// Should the code have the Obsolete atrribute set.
		/// </summary>
		public bool Obsolete = false;
		public bool ErrorOnObsolete = false;
		public string ObsoleteMessage = "";

		public string Name;
		/// <summary>
		/// Comments to add to summary like this one. 
		/// </summary>
		public List<string> Summary = new List<string> ();
	
		protected AbstractCodeElement (string name, AccessType access = AccessType.Public)
		{
			Name = name;
			accessType = access;
		}

		public override string ToString () {
			string summaryStr = "";
			Summary.ForEach ((string s) => summaryStr += "///" + s + "\n");
			string obs = (Obsolete ? "Obsolete" : "");
			return string.Format ("{0}\n{1}\n{2} {3}", obs, summaryStr, Access, Name);
		}
		
	}
}
