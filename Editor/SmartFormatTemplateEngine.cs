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
using System.IO;
using System.Collections;
using SmartFormat;
using SmartFormat.Core;

namespace Scio.CodeGeneration
{
	public class SmartFormatTemplateEngine : TemplateEngine
	{
		FileCodeElement fileCodeElement;
		TemplateEngineConfig config;
		CodeGeneratorResult result;

		string template = "";
		string code = "";
		public string Code { get { return code; } }

		public CodeGeneratorResult Prepare (TemplateEngineConfig inputConfig) {
			this.config = inputConfig;
			result = new CodeGeneratorResult ();
			if (config == null) {
				return result.SetError ("No Config", "No config for SmartFormatTemplateEngine provided.");
			}
			try {
				if (File.Exists (config.TemplatePath)) {
					Logger.Debug ("Reading template file " + config.TemplatePath);
					using (StreamReader fileReader = new StreamReader (config.TemplatePath)) {				
						template = fileReader.ReadToEnd ();
						fileReader.Close ();
						if (string.IsNullOrEmpty (template)) {
							result.SetError ("Template Empty", "Template file " + config.TemplatePath + " found but it seems to be empty.");
						}
					}
				} else {
					result.SetError ("Template Not Found", "Template file " + config.TemplatePath + " does not exist.");
				}
			} catch (System.Exception ex) {
				result.SetError ("Error Loading Template", "I/O error while trying to load Template file " + config.TemplatePath + "\n" + ex.Message);
			}
			return result;
		}

		public CodeGeneratorResult GenerateCode (FileCodeElement c) {
			this.fileCodeElement = c;
			result = new CodeGeneratorResult ();
			if (fileCodeElement == null) {
				return result.SetError ("No Class Data", "The providing classCodeElement is null. This indicates a problem during preprocessing the input source.");	
			}
			Smart.Default.ErrorAction = ErrorAction.OutputErrorInResult;
			Smart.Default.Parser.ErrorAction = ErrorAction.ThrowError;
			Smart.Default.Parser.UseAlternativeEscapeChar ('\\');
			
			code = Smart.Format(template, fileCodeElement);

			return result;
		}
	}
}

