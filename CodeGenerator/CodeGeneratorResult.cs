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
	public class CodeGeneratorResult
	{
		public enum Feedback
		{
			Success = 0,
			AskUserInput = 1,
			Error = 2,
		}
		public Feedback feedback = Feedback.Success;
		
		public bool NoSuccess { get { return feedback > Feedback.Success; } }
		public bool Success { get { return feedback == Feedback.Success; } }
		public bool AskUser { get { return feedback == Feedback.AskUserInput; } }
		public bool Error { get { return feedback == Feedback.Error; } }
		
		public string ErrorTitle = "";
		
		public string ErrorText = "";
		
		public void Reset () {
			feedback = Feedback.Success;
			ErrorTitle = "";
			ErrorText = "";
		}
		public CodeGeneratorResult SetError (object title = null, object text = null) {
			return Check (false, feedback, Feedback.Error, title, text);
		}
		
		public CodeGeneratorResult SetWarning (object title = null, object text = null) {
			return Check (false, feedback, Feedback.AskUserInput, title, text);
		}
		
		public CodeGeneratorResult SetSuccess (object title = null, object text = null) {
			this.ErrorTitle = (title != null ? title.ToString () : "");
			this.ErrorText = (text != null ? text.ToString () : "");
			return this;
		}
		
		public CodeGeneratorResult Check (bool condition, Feedback ok, Feedback failed, 
		                                  object title = null, object text = null) {
			if (condition) {
				feedback = ok;
			} else {
				feedback = failed;
				if (title != null) {
					ErrorTitle = title.ToString ();
				}
				if (text != null) {
					ErrorText = text.ToString ();
				}
			}
			return this;
		}
		
		public override string ToString () {
			return string.Format ("[CodeGeneratorResult: feedback={0}, ErrorTitle={1}, ErrorText={2}]", feedback, ErrorTitle, ErrorText);
		}
		
	}
}

