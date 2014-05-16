// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections.Generic;

namespace Scio.CodeGenerator
{
	public class SimpleCodeElement : GenericCodeElement
	{
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
	
	public class CommentCodeElement : CodeElement
	{
		public List<LineCommentCodeElement> Comments = new List<LineCommentCodeElement> ();

		public CommentCodeElement (string comment) {
		}

		public void AddComment (string comment) {
			Comments.Add (new LineCommentCodeElement (comment));
		}
	}
	
	public class UsingCodeElement : SimpleCodeElement
	{
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

