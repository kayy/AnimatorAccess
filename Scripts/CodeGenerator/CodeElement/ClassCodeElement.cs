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
	/// Contains all elements needed to generate a class including namespace but without using directives. One or 
	/// more class code elements are the most important parts of the FileCodeElement supplied to the template engine.
	/// </summary>
	public class ClassCodeElement : AbstractCodeElement
	{
		public override MemberTypeID MemberType {
			get { return MemberTypeID.Class; }
		}
		public NameSpaceCodeElement NameSpace = new NameSpaceCodeElement ();

		public bool PartialClass = false;
		public string Partial { get { return (PartialClass ? "partial" : "");} }
		
		public bool AbstractClass = false;
		public string Abstract { get { return (AbstractClass ? "abstract" : "");} }

		protected List<string> interfaces = new List<string> ();
		protected string baseClass = "";
		/// <summary>
		/// Provides the base class and interfaces as list of strings sorted to be used after the colon (:).
		/// </summary>
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

		/// <summary>
		/// List of delegate definitions.
		/// </summary>
		public List<DelegateDefinitionCodeElement> Delegates = new List<DelegateDefinitionCodeElement> ();
		/// <summary>
		/// Private, protected and publich fields, each one might contain initialisation code e.g. "bool b = true".
		/// </summary>
		public List<GenericFieldCodeElement> Fields = new List<GenericFieldCodeElement> ();
		/// <summary>
		/// All methods but no delegates (s. member Delegates).
		/// </summary>
		public List<GenericMethodCodeElement> Methods = new List<GenericMethodCodeElement> ();
		/// <summary>
		/// The properties. Note that auto porperties are not supported.
		/// </summary>
		public List<GenericPropertyCodeElement> Properties = new List<GenericPropertyCodeElement> ();
		/// <summary>
		/// Constructors of this class.
		/// </summary>
		public List<ConstructorCodeElement> Constructors = new List<ConstructorCodeElement> ();
		
		public ClassCodeElement (string name, AccessType access = AccessType.Public) :
			base (name, access)
		{
		}

		public bool IsEmpty () {
			int i = Fields.Count + Methods.Count + Properties.Count + Constructors.Count;
			return i == 0;
		}

		public void SetBaseClass (string baseClass) {
			this.baseClass = baseClass;
		}

		public void AddInterface (string nextInterface) {
			interfaces.Add (nextInterface);
		}

		/// <summary>
		/// Gets all fields, properties and methods of this class.
		/// </summary>
		/// <returns>The all members.</returns>
		public List<MemberCodeElement> GetAllMembers () {
			List<MemberCodeElement> l = new List<MemberCodeElement> ();
			Fields.ForEach ((item) => l.Add (item));
			Methods.ForEach ((item) => l.Add (item));
			Properties.ForEach ((item) => l.Add (item));
			return l;
		}

		/// <summary>
		/// Merges all those methods of other class into this one, that meet the filter condition.
		/// </summary>
		/// <param name="other">Other class whose methods will be merged into this class.</param>
		/// <param name="filter">Filter (optional), defines a subset of other's methods.</param>
		public void MergeMethods (ClassCodeElement other, Predicate<GenericMethodCodeElement> filter = null) {
			CodeElementUtils.MergeElements <GenericMethodCodeElement> (Methods, other.Methods, filter);
		}
		
		/// <summary>
		/// Merges all those properties of other class into this one, that meet the filter condition.
		/// </summary>
		/// <param name="other">Other class whose properties will be merged into this class.</param>
		/// <param name="filter">Filter (optional), defines a subset of other's properties.</param>
		public void MergeProperties (ClassCodeElement other, Predicate<GenericPropertyCodeElement> filter = null) {
			CodeElementUtils.MergeElements <GenericPropertyCodeElement> (Properties, other.Properties, filter);
		}
		
		/// <summary>
		/// Merges all those fields of other class into this one, that meet the filter condition.
		/// </summary>
		/// <param name="other">Other class whose fields will be merged into this class.</param>
		/// <param name="filter">Filter (optional), defines a subset of other's fields.</param>
		public void MergeFields (ClassCodeElement other, Predicate<GenericFieldCodeElement> filter = null) {
			CodeElementUtils.MergeElements <GenericFieldCodeElement> (Fields, other.Fields, filter);
		}

		/// <summary>
		/// Adds the given attribute to all methods, fields, constructors and properties.
		/// </summary>
		/// <param name="attribute">Attribute.</param>
		public void AddAttributeToAllMembers (AttributeCodeElement attribute) {
			Constructors.ForEach ((c) => c.AddAttribute (attribute));
			Methods.ForEach ((c) => c.AddAttribute (attribute));
			Properties.ForEach ((c) => c.AddAttribute (attribute));
			Fields.ForEach ((c) => c.AddAttribute (attribute));
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
			foreach (GenericFieldCodeElement item in Fields) {
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

