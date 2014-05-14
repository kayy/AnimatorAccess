// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections.Generic;

namespace Scio.CodeGenerator
{
	public class GenericVariableCodeElement : MemberCodeElement
	{
		public bool Const = false;

		public string Modifiers {
			get { 
				if (Const) {
					return "const";
				} else if (Static) {
					return "static";
				}
				return "";
			}
		}

		public string InitialiserCode = "";

		public GenericVariableCodeElement (string type, string name , string init = "", AccessType access = AccessType.Public) : 
			base (type, name, access)
		{
			InitialiserCode = init;
		}

	}

	public class VariableCodeElement<T> : GenericVariableCodeElement
	{
		public VariableCodeElement (string name, string init, AccessType access = AccessType.Public) : 
			base (GetFormattedType (typeof (T)), name, init, access) 
		{
		}
		
	}
	
}