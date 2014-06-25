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
	/// <summary>
	/// Marker interface for all code elements. Code elements can be of very different types like line comments,
	/// class, file, constructor, using directives, ...
	/// </summary>
	public interface CodeElement
	{
		/// <summary>
		/// Gets the type of the CodeElement.
		/// </summary>
		/// <value>The type of the member.</value>
		MemberTypeID MemberType { get; }
	}

	/// <summary>
	/// Code elements that contain a code block like methods or contructors.
	/// </summary>
	public interface ICodeBlock
	{
		List<string> Code { get; }
	}

	/// <summary>
	/// Member type needed for every CodeElement implementor.
	/// </summary>
	public enum MemberTypeID
	{
		Undefined = 0,
		Field = 1,
		Property = 2,
		Method = 3,
		Constructor = 4,
		Delegate = 5,
		Class = 10,
		Parameter = 20,
		Comment = 21,
		Attribute = 22,
		LineOfCode = 23,
		NamespaceDirective = 40,
		UsingDirective = 41,
		File = 100,
	}
	
}

