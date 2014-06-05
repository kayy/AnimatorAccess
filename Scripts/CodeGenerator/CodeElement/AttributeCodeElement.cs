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
using System.Collections.Generic;

namespace Scio.CodeGeneration
{
	public class AttributeCodeElement : CodeElement
	{
		public MemberTypeID MemberType {
			get { return MemberTypeID.Attribute; }
		}
		
		public string Name;
	
		public List<string> Parameters = new List<string> ();
	
		public AttributeCodeElement (string name)
		{
			this.Name = name;
		}
	
		public void AddParameter (bool parameter) {
			Parameters.Add (CodeElementUtils.GetFormattedValue (parameter));
		}
		
		public void AddParameter (string parameter) {
			Parameters.Add (parameter);
		}
		
		public void AddStringParameter (string param) {
			AddParameter ("\"" + param + "\"");
		}

		public string GetParameter (int i) {
			return Parameters.Count >= i - 1 ? Parameters [i] : "";
		}

		public override string ToString () {
			string str = "";
			Parameters.ForEach ((string s) => str += (str.Length > 0 ? ", " : "") + s);
			return string.Format ("[{0} ({1})]", Name, str);
		}
	}
	
	public class ObsoleteAttributeCodeElement : AttributeCodeElement
	{
		public ObsoleteAttributeCodeElement (string message, bool error) : base ("System.ObsoleteAttribute")
		{
			AddStringParameter (message);
			AddParameter (error.ToString ().ToLower ());
		}
	}

	public class GeneratedClassAttributeCodeElement : AttributeCodeElement
	{
		public GeneratedClassAttributeCodeElement (string creationDate, string lastVersionDate = "") : base ("Scio.CodeGeneration.GeneratedClassAttribute")
		{
			AddStringParameter (creationDate);
			if (!string.IsNullOrEmpty (lastVersionDate)) {
				AddStringParameter (lastVersionDate);
			}
		}
	}

	public class GeneratedMemberAttributeCodeElement : AttributeCodeElement
	{
		public GeneratedMemberAttributeCodeElement (string creationDate) : base ("Scio.CodeGeneration.GeneratedMemberAttribute")
		{
			AddStringParameter (creationDate);
		}
	}
}

