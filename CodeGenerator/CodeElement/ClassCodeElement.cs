// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections.Generic;


namespace Scio.CodeGeneration
{
	public class ClassCodeElement : AbstractCodeElement
	{
		public NameSpaceCodeElement NameSpace = new NameSpaceCodeElement ();

		public bool PartialClass = false;
		public string Partial { get { return (PartialClass ? "partial" : "");} }
		
		public bool AbstractClass = false;
		public string Abstract { get { return (AbstractClass ? "abstract" : "");} }

		protected List<string> interfaces = new List<string> ();
		protected string baseClass = "";
		public List<string> BaseClassAndInterfaces {
			get {
				List<string> l = new List<string> ();
				if (!string.IsNullOrEmpty (baseClass)) {
					l.Add (baseClass);
				}
				if (interfaces.Count > 0) {
					l.AddRange (interfaces);
				}
				return l;
			}
		}
		public bool HasBaseClassOrInterfaces { get { return !string.IsNullOrEmpty (baseClass) || interfaces.Count > 0; } }

		public List<GenericVariableCodeElement> Variables = new List<GenericVariableCodeElement> ();
		
		public List<GenericMethodCodeElement> Methods = new List<GenericMethodCodeElement> ();
		
		public List<GenericPropertyCodeElement> Properties = new List<GenericPropertyCodeElement> ();

		public List<ConstructorCodeElement> Constructors = new List<ConstructorCodeElement> ();
		
		public ClassCodeElement (string name, AccessType access = AccessType.Public) :
			base (name, access)
		{
		}

		public bool IsEmpty () {
			int i = Variables.Count + Methods.Count + Properties.Count + Constructors.Count;
			return i == 0;
		}

		public void SetBaseClass (string baseClass) {
			this.baseClass = baseClass;
		}

		public void AddInterface (string nextInterface) {
			interfaces.Add (nextInterface);
		}

		public void MergeMethods (ClassCodeElement other, Predicate<GenericMethodCodeElement> filter = null) {
			CodeElementUtils.MergeElements <GenericMethodCodeElement> (Methods, other.Methods, filter);
		}
		
		public void MergeProperties (ClassCodeElement other, Predicate<GenericPropertyCodeElement> filter = null) {
			CodeElementUtils.MergeElements <GenericPropertyCodeElement> (Properties, other.Properties, filter);
		}

		public void AddAttributeToAllMembers (AttributeCodeElement attribute) {
			Constructors.ForEach ((c) => c.AddAttribute (attribute));
			Methods.ForEach ((c) => c.AddAttribute (attribute));
			Properties.ForEach ((c) => c.AddAttribute (attribute));
			Variables.ForEach ((c) => c.AddAttribute (attribute));
		}

		public override string ToString () {
			string summaryStr = Summary.ToString ();
			string obs = (Obsolete ? "Obsolete" : "");
			string extendsStr = "";
			BaseClassAndInterfaces.ForEach ((i) => extendsStr += i + ", ");
			string str = string.Format ("{0}\n{1}\n{2} class {3}.{4} : {5}", obs, summaryStr, Access, NameSpace, Name, extendsStr);
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
			return str;
		}
	}
}

