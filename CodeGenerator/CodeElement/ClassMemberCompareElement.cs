// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections.Generic;

namespace Scio.CodeGeneration
{
	public class ClassMemberCompareElement
	{
		public enum Result
		{
			Remove = -1,
			Obsolete = 0,
			New = 1,
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

