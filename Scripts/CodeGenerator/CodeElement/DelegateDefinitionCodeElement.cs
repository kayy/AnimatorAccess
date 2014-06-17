// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Scio.CodeGeneration
{
	/// <summary>
	/// Delegate definition e.g. public delegate void MyCallback (int current, int previous);.
	/// </summary>
	public class DelegateDefinitionCodeElement : MemberCodeElement
	{
		public override MemberTypeID MemberType {
			get { return MemberTypeID.Delegate; }
		}

		public List<ParameterCodeElement> Parameters = new List<ParameterCodeElement> ();

		public DelegateDefinitionCodeElement (string type, string name , AccessType access = AccessType.Public) : 
			base (type, name, access) {
		}
	
		public void AddParameter (Type type, string name) {
			Parameters.Add (new ParameterCodeElement (type, name));
		}

		public override string ToString ()
		{
			return string.Format ("{0} delegate {1} {2} ({3})]", accessType, elementType, Name, ParameterCodeElement.ListToString (Parameters));
		}
		
	}
}

