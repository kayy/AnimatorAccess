// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections.Generic;


namespace Scio.CodeGenerator
{
	public class ClassCodeElement : AbstractCodeElement
	{
		public List<GenericVariableCodeElement> Variables = new List<GenericVariableCodeElement> ();
		
		public List<GenericMethodCodeElement> Methods = new List<GenericMethodCodeElement> ();
		
		public List<GenericPropertyCodeElement> Properties = new List<GenericPropertyCodeElement> ();
	
		public List<GenericMethodCodeElement> DeprecatedMethods = new List<GenericMethodCodeElement> ();
		
		public List<GenericPropertyCodeElement> DeprecatedProperties = new List<GenericPropertyCodeElement> ();
		
		public List<ConstructorCodeElement> Constructors = new List<ConstructorCodeElement> ();
		
		public ClassCodeElement (string name, AccessType access = AccessType.Public) :
			base (name, access)
		{
		}

		public override string ToString () {
			string summaryStr = "";
			Summary.ForEach ((string s) => summaryStr += "///" + s + "\n");
			string obs = (Obsolete ? "Obsolete" : "");
			string str = string.Format ("{0}\n{1}\n{2} class {3}", obs, summaryStr, Access, Name);
			str += "\nconstructors:\n";
			foreach (ConstructorCodeElement item in Constructors) {
				str += item + "\n";
			}
			str += "variables:\n";
			foreach (GenericVariableCodeElement item in Variables) {
				str += item + "\n";
			}
			str += "methods:\n";
			foreach (GenericMethodCodeElement item in Methods) {
				str += item + "\n";
			}
			str += "properties:\n";
			foreach (GenericPropertyCodeElement item in Properties) {
				str += item + "\n";
			}
			str += "deprecated methods:\n";
			foreach (GenericMethodCodeElement item in DeprecatedMethods) {
				str += item + "\n";
			}
			str += "deprecated properties:\n";
			foreach (GenericPropertyCodeElement item in DeprecatedProperties) {
				str += item + "\n";
			}
			return str;
		}
	}
}

