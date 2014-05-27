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
using System.Reflection;
using System.Collections.Generic;

namespace Scio.CodeGeneration
{
	public class ReflectionCodeElementsBuilder : CodeElementsBuilder
	{
		object obj;
		string assembyName;
		string className;
		public string ClassName { get { return className; } }

		BindingFlags fieldBinding = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | 
			BindingFlags.NonPublic;
		
		BindingFlags propertiesBinding = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | 
			BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty;

		BindingFlags methodBinding = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | 
			BindingFlags.InvokeMethod;

		Predicate<FieldInfo> fieldInfoFilter = null;
		public Predicate<FieldInfo> FieldInfoFilter {
			get { return fieldInfoFilter; }
			set { fieldInfoFilter = value; }
		}
		
		Predicate<MethodInfo> methodInfoFilter = null;
		public Predicate<MethodInfo> MethodInfoFilter {
			get { return methodInfoFilter; }
			set { methodInfoFilter = value; }
		}
		
		Predicate<PropertyInfo> propertyInfoFilter = null;
		public Predicate<PropertyInfo> PropertyInfoFilter {
			get { return propertyInfoFilter; }
			set { propertyInfoFilter = value; }
		}

		public ReflectionCodeElementsBuilder (string assembyName, string nameSpace, string className) {
			this.assembyName = assembyName;
			if (string.IsNullOrEmpty (nameSpace)) {
				this.className = className;
			} else {
				this.className = nameSpace + "." + className;
			}
		}

		public ReflectionCodeElementsBuilder (object o) {
			this.obj = o;
			if (obj != null) {
				Type type = obj.GetType ();
				className = type.Name;
				assembyName = type.Assembly.GetName ().Name;
			}
		}

		public bool HasType () {
			try {
				if (obj == null) {
					Assembly assemblyCSharp = Assembly.Load (assembyName);
					Type t = assemblyCSharp.GetType (className);
					if (t == null) {
						return false;
					}
					obj = assemblyCSharp.CreateInstance (className);
				}
				return obj != null;
			} catch (System.Exception ex) {
				if (ex == null) {}
				return false;
			}
		}

		public ClassCodeElement Build () {
			ClassCodeElement classCodeElement = new ClassCodeElement (className);
			if (!HasType ()) {
				return classCodeElement;
			}
			Type type = obj.GetType ();
			classCodeElement.NameSpace.Name = type.Namespace;
			CodeElementUtils.AddAttributes (classCodeElement.Attributes, type.GetCustomAttributes (false));
			PropertyInfo[] propertyInfoArray = type.GetProperties (propertiesBinding);
			List<PropertyInfo> propertyInfos = GetFilteredList (propertyInfoArray, propertyInfoFilter);
			CodeElementUtils.AddPropertyInfos (classCodeElement.Properties, propertyInfos);
			MethodInfo[] methodInfoArray = type.GetMethods (methodBinding);
			List<MethodInfo> methodInfos = GetFilteredList (methodInfoArray, methodInfoFilter);
			CodeElementUtils.AddMethodInfos (classCodeElement.Methods, methodInfos);
			FieldInfo[] fieldInfoArray = type.GetFields (fieldBinding);
			List<FieldInfo> fieldInfos = GetFilteredList (fieldInfoArray, fieldInfoFilter);
			CodeElementUtils.AddFieldInfos (classCodeElement.Fields, fieldInfos);
			return classCodeElement;
		}

		List<T> GetFilteredList<T> (T[] array, Predicate<T> filterPredicate) {
			if (array == null) {
				return new List<T> ();
			}
			List<T> listAll = new List<T> (array);
			if (filterPredicate == null) {
				return listAll;
			}
			return listAll.FindAll (filterPredicate);
		}
	}
}

