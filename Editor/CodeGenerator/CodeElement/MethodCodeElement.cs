// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections.Generic;

namespace Scio.CodeGeneration
{
	public class GenericMethodCodeElement : MemberCodeElement
	{
		public List<ParameterCodeElement> Parameters = new List<ParameterCodeElement> ();
		public List<string> Code = new List<string> ();
		
		public GenericMethodCodeElement (Type type, string name, AccessType access = AccessType.Public) : 
			base (type, name, access) {
		}

		public GenericMethodCodeElement (string type, string name, AccessType access = AccessType.Public) : 
			base (type, name, access) {
		}
		
		public void AddParameter (Type type, string name) {
			Parameters.Add (new ParameterCodeElement (type, name));
		}

		public void AddParameter (Type type, string name, object defaultValue) {
			Parameters.Add (new ParameterCodeElement (type, name, defaultValue));
		}

		public override string ToString ()
		{
			string str = "";
			Code.ForEach ((string s) => str += s + "\n");
			string pStr = "";
			Parameters.ForEach ((ParameterCodeElement p) => pStr += p + ", ");
			return string.Format ("{0} ({1})\n{2}", base.ToString (), pStr, str);
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
