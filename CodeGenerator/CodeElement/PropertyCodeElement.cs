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
using System.Linq;
using System.Collections.Generic;

namespace Scio.CodeGeneration
{
	public class GenericPropertyCodeElement : MemberCodeElement
	{
		public class Member
		{
			string getOrSet = "";
			GenericPropertyCodeElement parent;
			public List<string> CodeLines = new List<string> ();
			public AccessType access = AccessType.Public;
			public AccessType Access {
				get {
					return access;
				}
				set {
					access = value;
					if (parent.accessType < access) {
						parent.accessType = access;
					}
				}
			}
			public List<string> CodeBlock {
				get { 
					List<string> code = new List<string> ();
					if (CodeLines.Count == 1) {
						code.Add (getOrSet + "{ " + CodeLines [0] + " }");
						return code ;
					} else if (CodeLines.Count > 1) {
						code.Add (getOrSet + "{");
						CodeLines.ForEach ((s) => code.Add (s));
						code.Add ("}");
					}
					return code; 
				}
			}
			public Member (GenericPropertyCodeElement parent, string getOrSet) {
				this.getOrSet = getOrSet;
				this.parent = parent;
			}
		}
		public Member getter;
		public Member Getter {
			get {
				if (getter == null) {
					getter = new Member (this, "get");
				}
				return getter;
			}
		}

		public Member setter;
		public Member Setter {
			get {
				if (setter == null) {
					setter = new Member (this, "set");
				}
				return setter;
			}
		}

		public List<string> Code {
			get { 
				List<string> l = Getter.CodeBlock;
				l.AddRange (Setter.CodeBlock);
				return l;
			}
		}
		
		public string GetterAccess {
			get { 
				if (Getter.Access != accessType) {
					return Getter.Access.ToString ().ToLower ();
				}
				return "";
			}
		}
		
		public GenericPropertyCodeElement (Type type, string name, AccessType access = AccessType.Public) : 
			base (type, name, access)
		{
			Getter.Access = access;
			Setter.Access = access;
		}
		
		public GenericPropertyCodeElement (string type, string name, AccessType access = AccessType.Public) : 
			base (type, name, access)
		{
			Getter.Access = access;
			Setter.Access = access;
		}
		
		public override string ToString () {
			string strGet = "";
			Getter.CodeLines.ForEach ((string s) =>  strGet += "\n" + s);
			string strSet = "";
			Setter.CodeLines.ForEach ((string s) => strSet += "\n" + s);
			return string.Format ("{0}\n\tget {{{1}}}\n\tset {{{2}}}", base.ToString (), strGet, strSet);
		}
	}
	
	public class PropertyCodeElement<T> : GenericPropertyCodeElement
	{
		public PropertyCodeElement (string name, AccessType access = AccessType.Public) : 
			base (typeof (T), name, access)
		{
		}
	}
}

