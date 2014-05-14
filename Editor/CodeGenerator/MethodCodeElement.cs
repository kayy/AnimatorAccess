// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections.Generic;

namespace Scio.CodeGenerator
{
	public class GenericMethodCodeElement : MemberCodeElement
	{
		public string Parameters = "";
		public List<string> Code = new List<string> ();
		
		public GenericMethodCodeElement (Type type, string name, AccessType access = AccessType.Public) : 
			base (type, name, access)
		{
		}
		
		public override string ToString ()
		{
			string str = "";
			Code.ForEach ((string s) => str += s + "\n");
			return string.Format ("{0} ({1})\n{2}", base.ToString (), Parameters, str);
		}
	}
	
	public class MethodCodeElement<T> : GenericMethodCodeElement where T : struct
	{
		public MethodCodeElement (string name, AccessType access = AccessType.Public) : 
			base (typeof (T), name, access)
		{
		}
	}
}
