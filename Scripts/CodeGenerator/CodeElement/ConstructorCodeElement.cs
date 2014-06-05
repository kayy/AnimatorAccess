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
using UnityEngine;
using System.Collections.Generic;

namespace Scio.CodeGeneration
{
	public class ConstructorCodeElement : AbstractCodeElement, ICodeBlock
	{
		// TODO_kay: refactor to share code between methods and constructors

		public override MemberTypeID MemberType {
			get { return MemberTypeID.Constructor; }
		}
		public string Parameters = "";
		public List<string> code = new List<string> ();
		public List<string> Code {
			get { return code; }
		}

		public ConstructorCodeElement (string name, string parameters = "", AccessType access = AccessType.Public) :
			base (name, access)
		{
			this.Parameters = parameters;
		}

		public override string GetSignature () {
			string s = Name + "(" + Parameters + ")";
			return s;
		}

		public override bool Equals (object obj) {
			if (obj is ConstructorCodeElement) {
				return GetSignature () == ((ConstructorCodeElement)obj).GetSignature ();
			} else {
				return base.Equals (obj);
			}
		}

		public override int GetHashCode () {
			return GetSignature ().GetHashCode ();
		}
		
		public override string ToString () {
			string str = "";
			Code.ForEach ((string s) => str += s + "\n");
			return string.Format ("{0} ({1})\n{2}", base.ToString (), Parameters, str);
		}
	}
}

