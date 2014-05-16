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
			GenericPropertyCodeElement parent;
			public List<string> Code = new List<string> ();
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
			public Member (GenericPropertyCodeElement parent) {
				this.parent = parent;
			}
		}
		public Member getter;
		public Member Getter {
			get {
				if (getter == null) {
					getter = new Member (this);
				}
				return getter;
			}
		}

		public Member setter;
		public Member Setter {
			get {
				if (setter == null) {
					setter = new Member (this);
				}
				return setter;
			}
		}

		public string SetterAccess {
			get { return Setter.Access.ToString ().ToLower ();}
		}
		
		public string GetterAccess {
			get { return Getter.Access.ToString ().ToLower ();}
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
			Getter.Code.ForEach ((string s) =>  strGet += "\n" + s);
			string strSet = "";
			Setter.Code.ForEach ((string s) => strSet += "\n" + s);
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

