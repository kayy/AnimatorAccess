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

		public List<GenericFieldCodeElement> Fields = new List<GenericFieldCodeElement> ();
		
		public List<GenericMethodCodeElement> Methods = new List<GenericMethodCodeElement> ();
		
		public List<GenericPropertyCodeElement> Properties = new List<GenericPropertyCodeElement> ();

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

		public void MergeMethods (ClassCodeElement other, Predicate<GenericMethodCodeElement> filter = null) {
			CodeElementUtils.MergeElements <GenericMethodCodeElement> (Methods, other.Methods, filter);
		}
		
		public void MergeProperties (ClassCodeElement other, Predicate<GenericPropertyCodeElement> filter = null) {
			CodeElementUtils.MergeElements <GenericPropertyCodeElement> (Properties, other.Properties, filter);
		}
		
		public void MergeFields (ClassCodeElement other, Predicate<GenericFieldCodeElement> filter = null) {
			CodeElementUtils.MergeElements <GenericFieldCodeElement> (Fields, other.Fields, filter);
		}
		
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

