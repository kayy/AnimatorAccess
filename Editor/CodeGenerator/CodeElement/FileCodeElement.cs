// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System.Collections.Generic;

namespace Scio.CodeGenerator
{
	public class FileCodeElement : CodeElement
	{
		public List<UsingCodeElement> Usings = new List<UsingCodeElement> ();

		public List<ClassCodeElement> Classes = new List<ClassCodeElement> ();
	
		public FileCodeElement () {
		}
		public FileCodeElement (ClassCodeElement c) {
			Classes.Add (c);
		}
	}
}

