// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections;

namespace Scio.CodeGeneration
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class GeneratedClassAttribute : System.Attribute
	{
		public readonly string CreationDate;
		
		public GeneratedClassAttribute (string creationDate)  // url is a positional parameter
		{
			this.CreationDate = creationDate;
		}
		
	}
}

