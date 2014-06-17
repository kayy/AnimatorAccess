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

namespace Scio.CodeGeneration
{
	/// <summary>
	/// Template engine implementors have to provide a 3 step procedure for generating the code:
	/// - Prepare to check if all preconditions are met.
	/// - GenerateCode to generate the code and store it internally.
	/// - Code property to provide the result
	/// </summary>
	public interface TemplateEngine
	{
		/// <summary>
		/// Provides the result after GenerateCode has finished.
		/// </summary>
		/// <value>The code.</value>
		string Code {get;}
		/// <summary>
		/// Check if all preconditions are met e.g. template found and loaded, file system access, ...
		/// </summary>
		/// <returns>CodeGeneratorResult.Success or warnings otherwise.</returns>
		/// <param name="inputConfig">Input config.</param>
		CodeGeneratorResult Prepare (TemplateEngineConfig inputConfig);
		/// <summary>
		/// Generates the code and stores it internally to be retrieved later by property Code.
		/// </summary>
		/// <returns>CodeGeneratorResult.Success or warnings otherwise.</returns>
		/// <param name="classCodeElement">Class code element.</param>
		CodeGeneratorResult GenerateCode (FileCodeElement classCodeElement);
	}

	public class TemplateEngineConfig
	{
		public string TemplatePath { get; set; }

		public TemplateEngineConfig (string templatePath = "") {
			this.TemplatePath = templatePath;
		}
	}

}

