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
	public enum AccessType
	{
		Public = 3,
		Protected = 2,
		Private = 1,
	}

	/// <summary>
	/// Base class for all those elements that have some basic coding features in common e.g. name, access type, 
	/// attributes, ... Some other elements like files or attributes don't share these features and thus do not
	/// inherit from this class.
	/// TODO_kay: Renaming and restructuring to better fit individual elements.
	/// </summary>
	public abstract class AbstractCodeElement : CodeElement
	{
		public abstract MemberTypeID MemberType { get; }

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
			get { return Attributes.FindIndex ((a) => a is ObsoleteAttributeCodeElement) >= 0; }
			set {
				// add default obsolete attribute if it does not yet exist:
				if (!Obsolete) {
					AddAttribute (new ObsoleteAttributeCodeElement ("", false));
				}
			}
		}

		/// <summary>
		/// Name of the element.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Comments to add to summary like this one. 
		/// </summary>
		public SummaryCodeElement Summary = new SummaryCodeElement ();
	
		protected AbstractCodeElement (string name, AccessType access = AccessType.Public)
		{
			Name = name;
			accessType = access;
		}

		public void AddAttribute (AttributeCodeElement attribute) {
			AttributeCodeElement existingAttribute = Attributes.Find ((a) => a.Name == attribute.Name);
			if (existingAttribute != null) {
				Attributes.Remove (existingAttribute);
			}
			Attributes.Add (attribute);
		}

		public virtual string GetSignature () {
			return Name;
		}

		public override bool Equals (object obj) {
			return obj is AbstractCodeElement && GetSignature () == ((AbstractCodeElement)(obj)).GetSignature ();
		}
		
		public override int GetHashCode () {
			return GetSignature ().GetHashCode ();
		}
		
		public override string ToString () {
			string summaryStr = Summary.ToString ();
			string obs = "";
			Attributes.ForEach ((AttributeCodeElement a) => obs += (obs.Length > 0 ? "\n" : "") + a);
			return string.Format ("{0}\n{1}\n{2} {3}", obs, summaryStr, Access, Name);
		}
		
	}
}
