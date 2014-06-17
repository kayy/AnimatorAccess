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
using UnityEngine;
using System.Collections;
using Scio.CodeGeneration;

namespace Scio.AnimatorAccessGenerator {
	/// <summary>
	/// Contains all configuration parameters available for controlling the code generation. Most of the configuration 
	/// data is made persistent via Perferences class in AnimatorAccessConfig.txt.
	/// If you like to change something and you know what your doing, DO NOT EDIT THIS CLASS! 
	/// See ConfigFactory for more instructions.
	/// </summary>
	public class Config 
	{
		const string defaultMonoBehaviourTemplateFileName = "DefaultTemplate";
		protected virtual string DefaultMonoBehaviourTemplateFileName {
			get { return defaultMonoBehaviourTemplateFileName; }
		}

		const string defaultPlainClassTemplateFileName = "PlainClassDefaultTemplate.txt";
		[System.ObsoleteAttribute ("Plain class generation is not yet supported", true)]
		protected virtual string DefaultPlainClassTemplateFileName {
			get { return defaultPlainClassTemplateFileName; }
		}

		const string pathToTemplateDirectory = "Editor/Templates";
		/// <summary>
		/// If more than one file is found during the initial directory scan, use this as identifier
		/// </summary>
		public virtual string PathToTemplateDirectory {
			get { return pathToTemplateDirectory; }
		}

		public virtual bool AutoRefreshAssetDatabase {
			get { return Preferences.GetBool (Preferences.Key.AutoRefreshAssetDatabase); }
			set { Preferences.SetBool (Preferences.Key.AutoRefreshAssetDatabase, value); }
		}

		/// <summary>
		/// true means the user will not be asked if existing members should be removed in new code.
		/// The default behaviour (false) is to recreate existing members with the obsolete attribute once. Afterwards
		/// at the next creation, these deprecated members will be removed if keepDeprecatedMembers is false which is
		/// the default.
		/// </summary>
		public virtual bool IgnoreExistingCode {
			get { return Preferences.GetBool (Preferences.Key.IgnoreExistingCode); }
			set { Preferences.SetBool (Preferences.Key.IgnoreExistingCode, value); }
		}

		/// <summary>
		/// If true, all deprecated members will be regenerated. In this case the user should manually remove the code
		/// after all warnings are fixed.
		/// </summary>
		public virtual bool KeepObsoleteMembers {
			get { return Preferences.GetBool (Preferences.Key.KeepObsoleteMembers); }
			set { Preferences.SetBool (Preferences.Key.KeepObsoleteMembers, value); }
		}
		
		bool generateMonoBehaviourComponent = true;
		/// <summary>
		/// Plain class generation is not yet supported! 
		/// If true, a MonoBehaviour will be generated, otherwise a plain class.
		/// </summary>
		public virtual bool GenerateMonoBehaviourComponent {
			get { return generateMonoBehaviourComponent; }
			set { generateMonoBehaviourComponent = value; }
		}

		/// <summary>
		/// Change this only if you really know what you do.
		/// The namespace or no namespace directive if empty.
		/// </summary>
		public virtual string DefaultNamespace {
			get { return Preferences.GetString (Preferences.Key.DefaultNamespace, "AnimatorAccess"); }
			set { Preferences.SetString (Preferences.Key.DefaultNamespace, value); }
		}

		/// <summary>
		/// Change this only if you really know what you do.
		/// The mono behaviour component base class for all generated AnimatorAccess classes. 
		/// </summary>
		public virtual string MonoBehaviourComponentBaseClass {
			get { return Preferences.GetString (Preferences.Key.MonoBehaviourComponentBaseClass, "BaseAnimatorAccess"); }
			set { Preferences.SetString (Preferences.Key.MonoBehaviourComponentBaseClass, value); }
		}

		/// <summary>
		/// If true, the layer name will be prepended even for layer 0.
		/// </summary>
		public virtual bool ForceLayerPrefix {
			get { return Preferences.GetBool (Preferences.Key.ForceLayerPrefix); }
			set { Preferences.SetBool (Preferences.Key.ForceLayerPrefix, value); }
		}

		/// <summary>
		/// Optional prefix for all methods that check animator state e.g. Is<AnimatorStatePrefix>Idle ()
		/// </summary>
		public virtual string AnimatorStatePrefix {
			get { return Preferences.GetString (Preferences.Key.AnimatorStatePrefix, ""); }
			set { Preferences.SetString (Preferences.Key.AnimatorStatePrefix, value); }
		}
		
		/// <summary>
		/// Optional prefix for all int fields representing an animator state e.g. <AnimatorStateHashPrefix>Idle
		/// </summary>
		public virtual string AnimatorStateHashPrefix {
			get { return Preferences.GetString (Preferences.Key.AnimatorStateHashPrefix, "stateId"); }
			set { Preferences.SetString (Preferences.Key.AnimatorStateHashPrefix, value); }
		}
		
		/// <summary>
		/// Optional prefix for parameter access properties, e.g. float <Prefix>Speed
		/// </summary>
		public virtual string ParameterPrefix {
			get { return Preferences.GetString (Preferences.Key.ParameterPrefix, ""); }
			set { Preferences.SetString (Preferences.Key.ParameterPrefix, value); }
		}
		
		/// <summary>
		/// Optional prefix for int fields representing a parameter, e.g. float <ParameterHashPrefix>Speed
		/// </summary>
		public virtual string ParameterHashPrefix {
			get { return Preferences.GetString (Preferences.Key.ParameterHashPrefix, "paramId"); }
			set { Preferences.SetString (Preferences.Key.ParameterHashPrefix, value); }
		}
		
		/// <summary>
		/// Returns the appropriate default template file depending on flag GenerateMonoBehaviourComponent.
		/// </summary>
		public virtual string GetDefaultTemplateFileName () {
			string osFullName = SystemInfo.operatingSystem;
			string template = DefaultMonoBehaviourTemplateFileName;
			if (osFullName.StartsWith ("Win")) {
				template += "-Win.txt";
			} else {
				template += "-UNIX.txt"; 
			}
			return template;
		}
		
		/// <summary>
		/// Determines if and where to generate the code for automatic event handling, i.e. callbacks on state changes.
		/// </summary>
		public virtual StateEventHandlingMethod GenerateStateEventHandler {
			get { return (StateEventHandlingMethod)Preferences.GetInt (Preferences.Key.GenerateStateEventHandler, (int)(StateEventHandlingMethod.FixedUpdate)); }
			set { Preferences.SetInt (Preferences.Key.GenerateStateEventHandler, (int)value); }
		}

		/// <summary>
		/// If true, a <hash Id, state name> dictionary is created including method IdToName (int id).
		/// </summary>
		public virtual bool GenerateNameDictionary {
			get { return Preferences.GetBool (Preferences.Key.GenerateNameDictionary, true); }
			set { Preferences.SetBool (Preferences.Key.GenerateNameDictionary, value); }
		}

		/// <summary>
		/// If true, a <hash Id, state name> dictionary is created including method IdToName (int id).
		/// </summary>
		public virtual int AutoRefreshInterval {
			get { return Preferences.GetInt (Preferences.Key.AutoRefreshInterval, 30); }
			set { Preferences.SetInt (Preferences.Key.AutoRefreshInterval, value); }
		}
		
		/// <summary>
		/// If true, more log messages are written to console view.
		/// </summary>
		public virtual bool DebugMode {
			get { return Preferences.GetBool (Preferences.Key.DebugMode); }
			set { Preferences.SetBool (Preferences.Key.DebugMode, value); }
		}
		
	}
}

