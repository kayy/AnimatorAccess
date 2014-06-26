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
	/// <summary>
	/// One item in a list of results when comparing two classes. Most often two different version of a class. 
	/// Provides information about if this element will be inserted, marked as obsolete or removed. Some strings
	/// can be used to display details.
	/// </summary>
	public class ClassMemberCompareElement
	{
		public enum Result
		{
			Undefined = -1,
			Error = 0,
			New = 1,
			Remove = 2,
			Obsolete = 3,
		}
		public Result result;
	
		public virtual string memberType { get; private set; }
	
		public virtual string Member { get; private set; }
		
		public virtual string ElementType { get; private set; }
		
		public virtual string Signature { get; private set; }

		public string Message;

		protected ClassMemberCompareElement (Result result) {
			this.result = result;
		}

		public ClassMemberCompareElement (Result result, string memberType, string Member, string ElementType, string Signature, 
		        string message) : this (Result.Error) {
			this.result = result;
			this.memberType = memberType;
			this.Member = Member;
			this.ElementType = ElementType;
			this.Signature = Signature;
			this.Message = message;
		}

		public override string ToString ()
		{
			return string.Format ("[{0} {1}: {2}]", result, memberType, Member);
		}
		
	}

	public class CodeElementBasedCompareElement : ClassMemberCompareElement
	{
		public override string memberType { get { return UnderlyingElement.MemberType.ToString (); } }
		
		public override string Member { get { return UnderlyingElement.Name; } }
		
		public override string ElementType { get { return UnderlyingElement.ElementType; } }
		
		public override string Signature { get { return UnderlyingElement.GetSignature (); } }
		
		public readonly MemberCodeElement UnderlyingElement;
		
		public CodeElementBasedCompareElement (MemberCodeElement element, Result result) : base (result){
			UnderlyingElement = element;
		}
		public CodeElementBasedCompareElement (MemberCodeElement element, string message) : this (element, Result.Error){
			this.Message = message;
		}
	}
	
}

