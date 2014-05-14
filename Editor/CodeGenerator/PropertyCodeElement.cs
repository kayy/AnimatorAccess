// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections.Generic;

namespace Scio.CodeGenerator
{
	public class GenericPropertyCodeElement : MemberCodeElement
	{
		public bool HasGetter { get { return GetterCode.Count > 0; } }
		public bool HasSetter { get { return SetterCode.Count > 0; } }
		
		public List<string> GetterCode = new List<string> ();
		public List<string> SetterCode = new List<string> ();
		
		public GenericPropertyCodeElement (Type type, string name, AccessType access = AccessType.Public) : 
			base (type, name, access)
		{
		}
		
		public override string ToString () {
			string strGet = "";
			GetterCode.ForEach ((string s) =>  strGet += "\n" + s);
			string strSet = "";
			SetterCode.ForEach ((string s) => strSet += "\n" + s);
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

