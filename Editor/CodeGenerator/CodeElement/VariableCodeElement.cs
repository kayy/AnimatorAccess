// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections.Generic;

namespace Scio.CodeGeneration
{
	public class GenericVariableCodeElement : MemberCodeElement
	{
		public bool Const = false;
		public bool ReadOnly = false;

		public string Modifiers {
			get { 
				if (Const) {
					return "const";
				} else if (declaredStatic) {
					if (ReadOnly) {
						return "static readonly";
					} else {
						return "static";
					}
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
			base (CodeElementUtils.GetFormattedType (typeof (T)), name, init, access) 
		{
		}
		
	}
	
}