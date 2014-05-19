// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections;
using Scio.CodeGeneration;

namespace Scio.AnimatorWrapper {
	/// <summary>
	/// Contains all configuration parameters available for controlling the code generation. If you like to change
	/// something, DO NOT EDIT THIS CLASS! As this is a partial class there is an easier and more convenient way:
	/// 
	/// Create a new C# file e.g. AnimatorWrapperConfigExt.cs and change the class name to be the other 
	/// (additional) part of the class definition.
	/// Then define a default constructor and set the porperties you like.
	/// 
	/// Example file AnimatorWrapperConfigExt.cs:
	/// -----------------------
	/// namespace Scio.AnimatorWrapper {
	///     public partial class AnimatorWrapperrConfig {
	///         public AnimatorWrapperConfig () {
	///             ForceLayerPrefix = true;
	///         }
	///     }
	/// }
	/// -----------------------
	/// If you need a more specific configuration handling, you can register file specific instances in 
	/// AnimatorWrapperConfigFactory. See documentation there.
	/// </summary>
	public partial class AnimatorWrapperConfig 
	{
		const string defaultMonoBehaviourTemplateFileName = "ComponentDefaultTemplate.txt";
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

		const string pathToTemplateDirectory = "AnimatorWrapper/Editor/CodeGenerator";
		/// <summary>
		/// If more than one file is found during the initial directory scan, use this as identifier
		/// </summary>
		public virtual string PathToTemplateDirectory {
			get { return pathToTemplateDirectory; }
		}

		/// <summary>
		/// true means the user will not be asked if existing members will be removed in new code.
		/// The default behaviour (false) is to recreate existing members with the obsolete attribute once. Afterwards
		/// at the next creation, these deprecated members will be removed if keepDeprecatedMembers is false which is
		/// the default.
		/// </summary>
		bool forceOverwritingOldClass = false;
		public virtual bool ForceOverwritingOldClass {
			get { return forceOverwritingOldClass; }
			set { forceOverwritingOldClass = value; }
		}

		/// <summary>
		/// If true, all deprecated members will be regenerated. In this case the user should manually remove the code
		/// after all warnings are fixed.
		/// </summary>
		bool keepObsoleteMembers = false;
		public virtual bool KeepObsoleteMembers {
			get { return keepObsoleteMembers; }
			set { keepObsoleteMembers = value; }
		}
		
		bool generateMonoBehaviourComponent = false;
		/// <summary>
		/// If true, a MonoBehaviour will be generated, otherwise a plain class.
		/// </summary>
		public virtual bool GenerateMonoBehaviourComponent {
			get { return generateMonoBehaviourComponent; }
			set { generateMonoBehaviourComponent = value; }
		}

		bool autoSelectTemplate = true;
		/// <summary>
		/// If true, the generator looks for <DefaultTemplateSubDirectory>/<ClassName>.txt. If false or no class 
		/// specific template is found, the default template will be used.
		/// </summary>
		public virtual bool AutoSelectTemplate {
			get { return autoSelectTemplate; }
			set { autoSelectTemplate = value; }
		}

		string defaultNamespace = "AnimatorAccess";
		/// <summary>
		/// The namespace or no namespace directive if empty.
		/// </summary>
		public virtual string DefaultNamespace {
			get { return defaultNamespace; }
			set { defaultNamespace = value; }
		}
		
		bool forceLayerPrefix = false;
		/// <summary>
		/// If true, the layer name will be prepended even for layer 0.
		/// </summary>
		public virtual bool ForceLayerPrefix {
			get { return forceLayerPrefix; }
			set { forceLayerPrefix = value; }
		}

		string animationStatePrefix = "";
		/// <summary>
		/// Optional prefix for all methods that check animation state e.g. Is<Prefix>Idle ()
		/// </summary>
		public virtual string AnimationStatePrefix {
			get { return animationStatePrefix; }
			set { animationStatePrefix = value; }
		}
		
		string parameterPrefix = "";
		/// <summary>
		/// Optional prefix for parameter access properties, e.g. float <Prefix>Speed
		/// </summary>
		public virtual string ParameterPrefix {
			get { return parameterPrefix; }
			set { parameterPrefix = value; }
		}
		
		CodeGenerator generator = new SmartFormatCodeGenerator ();
		/// <summary>
		/// The template engine to use for code generation.
		/// </summary>
		public virtual CodeGenerator Generator {
			get { return generator; }
			set { generator = value; }
		}

		/// <summary>
		/// Returns the appropriate default template file depending on flag GenerateMonoBehaviourComponent.
		/// </summary>
		public virtual string GetDefaultTemplateFileName () {
			return (GenerateMonoBehaviourComponent ? DefaultMonoBehaviourTemplateFileName : DefaultPlainClassTemplateFileName);
		}

		public override string ToString ()
		{
			return string.Format ("[AnimatorWrapperConfig: defaultMonoBehaviourTemplateFileName={0}, defaultPlainClassTemplateFileName={1}, defaultTemplateSubDirectory={2}, pathToTemplateDirectory={3}, generateMonoBehaviourComponent={4}, autoSelectTemplate={5}, defaultNamespace={6}, forceLayerPrefix={7}, animationStatePrefix={8}, parameterPrefix={9}]", defaultMonoBehaviourTemplateFileName, defaultPlainClassTemplateFileName, defaultTemplateSubDirectory, pathToTemplateDirectory, generateMonoBehaviourComponent, autoSelectTemplate, defaultNamespace, forceLayerPrefix, animationStatePrefix, parameterPrefix);
		}
		
	}
}

