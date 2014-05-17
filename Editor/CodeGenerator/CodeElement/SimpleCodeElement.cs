// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections.Generic;

namespace Scio.CodeGenerator
{
	public class SimpleCodeElement : GenericCodeElement
	{
		public string Content { get { return content; } }

		public SimpleCodeElement (string str) : base (str) {
			this.content = str;
		}
		public SimpleCodeElement () {}

	}

	public class LineCommentCodeElement : SimpleCodeElement
	{
		public LineCommentCodeElement (string comment) : base (comment) {
		}
	}
	
	public class SummaryCodeElement : CodeElement
	{
		List<string> Comments = new List<string> ();

		public SummaryCodeElement () {
		}
		public SummaryCodeElement (string comment) {
			Add (comment);
		}

		public void Add (string comment) {
			if (Comments.Count == 0) {
				Comments.Add ("<summary>");
				Comments.Add ("</summary>");
			}
			int i = Comments.Count - 1;
			if (i > 0) {
				Comments.Insert (i, comment);
			}
		}
		public override string ToString () {
			string summaryStr = "";
			Comments.ForEach ((string s) => summaryStr += "///" + s + "\n");
			return summaryStr;
		}
	}
	
	public class UsingCodeElement : SimpleCodeElement
	{
		public string Name { get { return content; } }

		public UsingCodeElement (string usingNamespace) : base (usingNamespace) {
		}
	}

	public class NameSpaceCodeElement : SimpleCodeElement
	{
		public bool UseDefault { get { return string.IsNullOrEmpty (content); } }

		public string Name {
			get { return content; }
			set { content = value; }
		}

		public NameSpaceCodeElement (string namespaceToUse) : base (namespaceToUse) {
		}
		public NameSpaceCodeElement () : base ("") {
		}
	}

	public class ParameterCodeElement : CodeElement
	{
		public string ParameterType;
		public string Name;
		public string DefaultValue = "";
		
		public ParameterCodeElement (Type type, string name) {
			ParameterType = CodeElementUtils.GetFormattedType (type);
			Name = name;
		}
		public ParameterCodeElement (Type type, string name, object defaultValue) :
			this (type, name)
		{
			DefaultValue = CodeElementUtils.GetFormattedValue (defaultValue);
		}
		public override string ToString () {
			return string.Format ("{0} {1}{2}", ParameterType, Name, (DefaultValue.Length > 0 ? " = " + DefaultValue : ""));
		}
		
	}
	
}

