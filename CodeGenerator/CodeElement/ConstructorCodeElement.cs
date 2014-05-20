// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections.Generic;

namespace Scio.CodeGeneration
{
	public class ConstructorCodeElement : AbstractCodeElement
	{
		public string Parameters = "";
		public List<string> Code = new List<string> ();

		public ConstructorCodeElement (string name, string parameters = "", AccessType access = AccessType.Public) :
			base (name, access)
		{
			this.Parameters = parameters;
		}

		public override string ToString () {
			string str = "";
			Code.ForEach ((string s) => str += s + "\n");
			return string.Format ("{0} ({1})\n{2}", base.ToString (), Parameters, str);
		}
	}
}

