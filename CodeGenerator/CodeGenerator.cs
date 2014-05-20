// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;

namespace Scio.CodeGeneration
{
	public interface CodeGenerator
	{
		string Code {get;}

		CodeGeneratorResult Prepare (CodeGeneratorConfig inputConfig);

		CodeGeneratorResult GenerateCode (FileCodeElement classCodeElement);
	}

	public class CodeGeneratorConfig
	{
		public string Template;
		public CodeGeneratorConfig (string template) {
			this.Template = template;
		}
	}

}

