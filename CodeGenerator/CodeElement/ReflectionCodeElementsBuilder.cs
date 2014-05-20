// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
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

		BindingFlags propertiesBinding = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | 
			BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty;

		BindingFlags methodBinding = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | 
			BindingFlags.InvokeMethod;

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

		public bool HasType () {
			try {
				Assembly assemblyCSharp = Assembly.Load (assembyName);
				Type t = assemblyCSharp.GetType (className);
				if (t == null) {
					return false;
				}
				obj = assemblyCSharp.CreateInstance (className);
				return obj != null;
			} catch (System.Exception ex) {
				if (ex == null) {}
				return false;
			}
		}

		public ClassCodeElement Build () {
			if (obj == null && !HasType ()) {
				return null;
			}
			ClassCodeElement classCodeElement = new ClassCodeElement (className);
			Type type = obj.GetType ();
			classCodeElement.NameSpace.Name = type.Namespace;
			PropertyInfo[] propertyInfoArray = type.GetProperties (propertiesBinding);
			List<PropertyInfo> propertyInfos = GetFilteredList (propertyInfoArray, propertyInfoFilter);
			CodeElementUtils.AddPropertyInfos (classCodeElement.Properties, propertyInfos);
			MethodInfo[] methodInfoArray = type.GetMethods (methodBinding);
			List<MethodInfo> methodInfos = GetFilteredList (methodInfoArray, methodInfoFilter);
			CodeElementUtils.AddMethodInfos (classCodeElement.Methods, methodInfos);
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

