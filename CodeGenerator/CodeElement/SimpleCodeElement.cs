// The MIT License (MIT)
// 
// Copyright (c) 2014 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;
using System.Collections.Generic;

namespace Scio.CodeGeneration
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
		List<string> comments = new List<string> ();
		public List<string> Comments {
			get {
				List<string> formattedComments = new List<string> ();
				if (comments.Count > 0) {
					formattedComments.Add ("<summary>");
					formattedComments.AddRange (comments);
					formattedComments.Add ("</summary>");
				}
				return comments;
			}
		}

		public SummaryCodeElement () {
		}
		public SummaryCodeElement (string comment) {
			Add (comment);
		}

		public void Add (string comment) {
			if (comments.Count == 0) {
				comments.Add ("<summary>");
				comments.Add ("</summary>");
			}
			int i = comments.Count - 1;
			if (i > 0) {
				comments.Insert (i, comment);
			}
		}
		public override string ToString () {
			string summaryStr = "";
			comments.ForEach ((string s) => summaryStr += "/// " + s);
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

