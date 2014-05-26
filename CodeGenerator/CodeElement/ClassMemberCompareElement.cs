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
	public class ClassMemberCompareElement
	{
		public enum Result
		{
			New = 1,
			Remove = 2,
			Obsolete = 3,
		}
		public Result result;
	
		public enum MemberType
		{
			Undefined = 0,
			Field = 1,
			Property = 2,
			Method = 3,
		}
		public MemberType memberType;
	
		public string member;
	
		public ClassMemberCompareElement (MemberCodeElement element, Result result) {
			this.member = element.Name;
			this.result = result;
			if (element is GenericPropertyCodeElement) {
				memberType = MemberType.Property;
			} else if (element is GenericMethodCodeElement) {
				memberType = MemberType.Method;
			} else if (element is GenericFieldCodeElement) {
				memberType = MemberType.Field;
			} else {
				this.memberType = MemberType.Undefined;
			}
		}

		public ClassMemberCompareElement (string member, Result result, MemberType memberType = MemberType.Undefined) {
			this.member = member;
			this.result = result;
			this.memberType = memberType;
		}

		public override string ToString ()
		{
			return string.Format ("[{0} {1}: {2}]", result, memberType, member);
		}
		
	}
	
	public class MemberNameComparer<T> : IEqualityComparer <T> where T : MemberCodeElement
	{
		public bool Equals (T x, T y)
		{
			return x.Name == y.Name;
		}
		
		public int GetHashCode (T obj)
		{
			return obj.Name.GetHashCode ();
		}
	}
}

