// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
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

	public abstract class AbstractCodeElement : CodeElement
	{
		protected AccessType accessType = AccessType.Public;
		public string Access {
			get { return accessType.ToString ().ToLower ();}
		}

		public bool declaredStatic = false;
		public string Static { get { return (declaredStatic ? "static" : ""); } }

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
		public SummaryCodeElement Summary = new SummaryCodeElement ();
	
		protected AbstractCodeElement (string name, AccessType access = AccessType.Public)
		{
			Name = name;
			accessType = access;
		}

		public override string ToString () {
			string summaryStr = Summary.ToString ();
			string obs = "";
			Attributes.ForEach ((AttributeCodeElement a) => obs += (obs.Length > 0 ? "\n" : "") + a);
			return string.Format ("{0}\n{1}\n{2} {3}", obs, summaryStr, Access, Name);
		}
		
	}
}
