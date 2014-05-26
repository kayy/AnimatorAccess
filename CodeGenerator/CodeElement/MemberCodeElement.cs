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
	public abstract class MemberCodeElement : AbstractCodeElement
	{
		/// <summary>
		/// The original state or parameter name.
		/// </summary>
		public string Origin;

		protected string elementType;
		public string ElementType { get { return elementType;} }
		
		protected MemberCodeElement (Type type, string name, AccessType access = AccessType.Public) :
			this (CodeElementUtils.GetFormattedType (type), name, access)
		{
		}
		
		protected MemberCodeElement (string typeString, string name, AccessType access = AccessType.Public) :
			base (name, access)
		{
			elementType = typeString;
		}

		public override bool Equals (object obj) {
			return obj is MemberCodeElement && Name == ((MemberCodeElement)(obj)).Name;
		}

		public override int GetHashCode () {
			return Name.GetHashCode ();
		}

		public override string ToString () {
			string str = Summary.ToString ();
			string obs = "";
			Attributes.ForEach ((AttributeCodeElement a) => obs += (obs.Length > 0 ? "\n" : "") + a);
			return string.Format ("{0} {1}\n{2} {3} {4}", obs, str, Access, elementType, Name);
		}
	}
}

