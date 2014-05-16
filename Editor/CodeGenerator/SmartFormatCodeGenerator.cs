// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System.IO;
using System.Collections;
using SmartFormat;
using SmartFormat.Core;

namespace Scio.CodeGenerator
{
	public class SmartFormatCodeGenerator : CodeGenerator
	{
		FileCodeElement fileCodeElement;
		CodeGeneratorConfig config;
		CodeGeneratorResult result;

		string template = "";
		string code = "";
		public string Code { get { return code; } }

		public CodeGeneratorResult Prepare (CodeGeneratorConfig inputConfig) {
			this.config = inputConfig;
			result = new CodeGeneratorResult ();
			if (config == null) {
				return result.SetError ("No Config", "No config for SmartFormatCodeGenerator provided.");
			}
			try {
				if (File.Exists (config.Template)) {
					using (StreamReader fileReader = new StreamReader (config.Template)) {				
						template = fileReader.ReadToEnd ();
						fileReader.Close ();
						if (string.IsNullOrEmpty (template)) {
							result.SetError ("Template Empty", "Template file " + config.Template + " found but it seems to be empty.");
						}
					}
				} else {
					result.SetError ("Template Not Found", "Template file " + config.Template + " does not exist.");
				}
			} catch (System.Exception ex) {
				result.SetError ("Error Loading Template", "I/O error while trying to load Template file " + config.Template + "\n" + ex.Message);
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
			
			Smart.Default.Parser.UseBraceEscaping ();
			code = Smart.Format(template, fileCodeElement);

			return result;
		}
	}
}

