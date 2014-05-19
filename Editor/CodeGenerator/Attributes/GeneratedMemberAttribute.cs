// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;

namespace Scio.CodeGeneration
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public class GeneratedMemberAttribute : System.Attribute
	{
		public readonly string CreationDate;
		
		bool forceRegeneration;
		public bool Version {
			get { return forceRegeneration; }
			set { forceRegeneration = value; }
		}
		
		public GeneratedMemberAttribute (string creationDate)
		{
			this.CreationDate = creationDate;
		}
		
	}
}
