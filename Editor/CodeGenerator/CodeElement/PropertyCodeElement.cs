// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using System;
using System.Collections.Generic;

namespace Scio.CodeGenerator
{
	public class GenericPropertyCodeElement : MemberCodeElement
	{
		public class Member
		{
			string getOrSet = "";
			GenericPropertyCodeElement parent;
			public List<string> CodeLines = new List<string> ();
			public AccessType access = AccessType.Public;
			public AccessType Access {
				get {
					return access;
				}
				set {
					access = value;
					if (parent.accessType < access) {
						parent.accessType = access;
					}
				}
			}
			public List<string> CodeBlock {
				get { 
					List<string> code = new List<string> ();
					if (CodeLines.Count == 1) {
						code.Add (getOrSet + "{ " + CodeLines [0] + " }");
						return code ;
					} else if (CodeLines.Count > 1) {
						code.Add (getOrSet + "{ ");
						code.AddRange (CodeLines);
						code.Add ("}");
					}
					return code; 
				}
			}
			public Member (GenericPropertyCodeElement parent, string getOrSet) {
				this.getOrSet = getOrSet;
				this.parent = parent;
			}
		}
		public Member getter;
		public Member Getter {
			get {
				if (getter == null) {
					getter = new Member (this, "get");
				}
				return getter;
			}
		}

		public Member setter;
		public Member Setter {
			get {
				if (setter == null) {
					setter = new Member (this, "set");
				}
				return setter;
			}
		}

		public string SetterAccess {
			get { return Setter.Access.ToString ().ToLower ();}
		}
		
		public string GetterAccess {
			get { 
				if (Getter.Access != accessType) {
					return Getter.Access.ToString ().ToLower ();
				}
				return "";
			}
		}
		
		public GenericPropertyCodeElement (Type type, string name, AccessType access = AccessType.Public) : 
			base (type, name, access)
		{
			Getter.Access = access;
			Setter.Access = access;
		}
		
		public GenericPropertyCodeElement (string type, string name, AccessType access = AccessType.Public) : 
			base (type, name, access)
		{
			Getter.Access = access;
			Setter.Access = access;
		}
		
		public override string ToString () {
			string strGet = "";
			Getter.CodeLines.ForEach ((string s) =>  strGet += "\n" + s);
			string strSet = "";
			Setter.CodeLines.ForEach ((string s) => strSet += "\n" + s);
			return string.Format ("{0}\n\tget {{{1}}}\n\tset {{{2}}}", base.ToString (), strGet, strSet);
		}
	}
	
	public class PropertyCodeElement<T> : GenericPropertyCodeElement
	{
		public PropertyCodeElement (string name, AccessType access = AccessType.Public) : 
			base (typeof (T), name, access)
		{
		}
	}
}

