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
		const string defaultMonoBehaviourTemplateFileName = "DefaultTemplate.txt";
		protected virtual string DefaultMonoBehaviourTemplateFileName {
			get { return defaultMonoBehaviourTemplateFileName; }
		}

		const string defaultPlainClassTemplateFileName = "PlainClassDefaultTemplate.txt";
		protected virtual string DefaultPlainClassTemplateFileName {
			get { return defaultPlainClassTemplateFileName; }
		}

		const string defaultTemplateSubDirectory = "Templates";
		public virtual string DefaultTemplateSubDirectory {
			get { return defaultTemplateSubDirectory; }
		}

		// TODO_kay: change AnimatorWrapper -> AnimatorAccess
		const string pathToTemplateDirectory = "AnimatorWrapper/Editor";
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
		public virtual bool ForceOverwritingOldClass {
			get { return Preferences.GetBool (Preferences.Key.ForceOverwritingOldClass); }
			set { Preferences.SetBool (Preferences.Key.ForceOverwritingOldClass, value); }
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
		/// Optional prefix for all methods that check animation state e.g. Is<Prefix>Idle ()
		/// </summary>
		public virtual string AnimatorStatePrefix {
			get { return Preferences.GetString (Preferences.Key.AnimatorStatePrefix, ""); }
			set { Preferences.SetString (Preferences.Key.AnimatorStatePrefix, value); }
		}
		
		/// <summary>
		/// Optional prefix for parameter access properties, e.g. float <Prefix>Speed
		/// </summary>
		public virtual string ParameterPrefix {
			get { return Preferences.GetString (Preferences.Key.ParameterPrefix, ""); }
			set { Preferences.SetString (Preferences.Key.ParameterPrefix, value); }
		}
		
		/// <summary>
		/// Returns the appropriate default template file depending on flag GenerateMonoBehaviourComponent.
		/// </summary>
		public virtual string GetDefaultTemplateFileName () {
			return (GenerateMonoBehaviourComponent ? DefaultMonoBehaviourTemplateFileName : DefaultPlainClassTemplateFileName);
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

