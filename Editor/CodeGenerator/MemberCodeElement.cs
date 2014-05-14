// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections.Generic;

namespace Scio.CodeGenerator
{
	public abstract class MemberCodeElement : AbstractCodeElement
	{
		/// <summary>
		/// The original state or parameter name.
		/// </summary>
		public string Origin;

		protected string elementType;
		
		protected MemberCodeElement (Type type, string name, AccessType access = AccessType.Public) :
			this (GetFormattedType (type), name, access)
		{
		}
		
		protected MemberCodeElement (string typeString, string name, AccessType access = AccessType.Public) :
			base (name, access)
		{
			elementType = typeString;
		}
		
		public override string ToString () {
			string str = "";
			Summary.ForEach ((string s) => str += "///" + s + "\n");
			string obs = (Obsolete ? "Obsolete" : "");
			return string.Format ("{0} {1}\n{2} {3} {4}", obs, str, Access, elementType, Name);
		}
	}
}

