// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Scio.CodeGenerator
{
	public static class CodeElementUtils
	{
		public static string GetFormattedType (Type elementType) {
			if (elementType == typeof(bool)) {
				return "bool";
			} else if (elementType == typeof(int)) {
				return "int";
			} else if (elementType == typeof(float)) {
				return "float";
			} else if (elementType == typeof(double)) {
				return "double";
			} else if (elementType == typeof(string)) {
				return "string";
			}
			return elementType.Name;
		}
		
		public static string GetFormattedValue (object obj) {
			if (obj == null) {
				return "null";
			} else if (obj == typeof(bool)) {
				return obj.ToString ().ToLower ();
			} else if (obj == typeof(string)) {
				string s = obj.ToString ();
				if (s.StartsWith ("\"") && s.EndsWith ("\"")) {
					return s;
				}
				return "\"" + s + "\"";
			}
			return obj.ToString ();
		}
		
		static AccessType GetAccessType (MethodInfo method) {
			if (method == null) {
				return AccessType.Private;
			}
			AccessType access = AccessType.Public;
			if (method.IsFamily) {
				access = AccessType.Protected;
			} else if (method.IsPrivate) {
				access = AccessType.Private;
			}
			return access;
		}

		public static void AddPropertyInfos (List<GenericPropertyCodeElement> properties, PropertyInfo[] propertyInfos) {
			foreach (PropertyInfo propertyInfo in propertyInfos) {
				string name = propertyInfo.Name;
				Type type = propertyInfo.PropertyType;
				AccessType access = AccessType.Public;
				GenericPropertyCodeElement p = new GenericPropertyCodeElement (type, name, access);
				MethodInfo[] accessors = propertyInfo.GetAccessors (true);
				foreach (MethodInfo m in accessors) {
					AccessType acc = GetAccessType (m);
					if (m.Name.StartsWith ("get_")) {
						p.Getter.Access = acc;
						p.Getter.Code.Add ("throw new System.NotImplementedException ();");
					} else if (m.Name.StartsWith ("set_")) {
						p.Setter.Access = acc;
						p.Setter.Code.Add ("throw new System.NotImplementedException ();");
					}
				}
				AddAttributes (p.Attributes, propertyInfo.GetCustomAttributes (false));
//				AccessType access = (getterAccess <= setterAccess ? getterAccess : setterAccess);
				properties.Add (p);
			}
		}

		public static void AddMethodInfos (List<GenericMethodCodeElement> methods, List<MethodInfo> methodInfos) {
			foreach (MethodInfo m in methodInfos) {
				if (m.Name.StartsWith ("Is")) {
				}
				Type t = m.ReturnType;
				Log.Temp (t + " " + m.Name);
				GenericMethodCodeElement methodElement = new GenericMethodCodeElement (t, m.Name, GetAccessType (m));
				foreach (ParameterInfo param in m.GetParameters ()) {
					if (param.DefaultValue != null) {
						// NOTE: There is no chance .NET 3.5 to distinguish between a null default value and NO default value
						// In 4.0 a new property HasDefaultValue was introduced
						methodElement.AddParameter (param.ParameterType, param.Name, param.DefaultValue);
					} else {
						methodElement.AddParameter (param.ParameterType, param.Name);
					}
				}
				methods.Add (methodElement);
			}
		}
		
		static void AddAttributes (List<AttributeCodeElement> attributeList, object[] attributeObjects) {
			foreach (object o in attributeObjects) {
				if (o is ObsoleteAttribute) {
					ObsoleteAttribute a = (ObsoleteAttribute)o;
					attributeList.Add (new ObsoleteAttributeCodeElement (a.Message, a.IsError));
				} else {
					Log.Temp ("Attribute = " + o.ToString ());
				}
			}
		}

	}
}

