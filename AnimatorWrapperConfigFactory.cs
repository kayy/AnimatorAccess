// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections.Generic;

namespace Scio.AnimatorWrapper
{
	/// <summary>
	/// DO NOT EDIT THIS CLASS! THERE ARE EASIER WAYS. 
	/// Summary: Factory for providing configuration parameters depending on the class to generate. If you like to 
	/// change something, the appropriate way is:
	/// 
	/// Create a new C# file e.g. AnimatorWrapperConfigExt.cs and change the class name to be the other 
	/// (additional) part of this partial class definition.
	/// Then define a default constructor and set the porperties you like.
	/// 
	/// Example file AnimatorWrapperConfigFactoryExt.cs:
	/// -----------------------
	/// namespace Scio.AnimatorWrapper {
	///     public partial class AnimatorWrapperConfigFactory {
	///         static AnimatorWrapperConfigFactory () {
	///             instance = new AnimatorWrapperConfigFactory ();
	///             instance.defaultConfig = new MyAnimatorWrapperConfig ();
	///         }
	///     }
	/// }
	/// -----------------------
	/// But you should really know what you are doing !
	/// If you want even more control, just create your own factory class and register it the way shown above.
	/// </summary>
	public interface IAnimatorWrapperConfigFactory
	{
		/// <summary>
		/// Gets the specific config for the given class. Default behaviour is: 
		/// If no config is registered in configs for the key className, defaultConfig is taken.
		/// </summary>
		/// <returns>The specific config.</returns>
		/// <param name="className">key to look for in configs.</param>
		/// <param name="relativePath">Relative path from Assets dir.</param>
		Config GetSpecificConfig (string className, string relativePath);

		Config GetDefaultConfig ();
	}

	public partial class AnimatorWrapperConfigFactory : IAnimatorWrapperConfigFactory
	{
		protected static IAnimatorWrapperConfigFactory instance = null;

		public static IAnimatorWrapperConfigFactory Instance {
			get {
				if (instance == null) {
					instance = new AnimatorWrapperConfigFactory ();
				}
				return instance;
			}
		}
	
		public static Config Get (string className, string relativePath = "") {
			return Instance.GetSpecificConfig (className, relativePath);
		}

		public static Config DefaultConfig {
			get {
				return Instance.GetDefaultConfig ();
			}
		}

		public Config GetSpecificConfig (string className, string relativePath) {
			if (!string.IsNullOrEmpty (className) && configs.ContainsKey (className)) {
				Config c = configs [className];
				Scio.CodeGeneration.Logger.Debug ("Using special config for " + className + ": " + c.ToString ());
				return c;
			}
			return defaultConfig;
		}

		public Config GetDefaultConfig () {
			return defaultConfig;
		}

		protected Dictionary<string, Config> configs = new Dictionary<string, Config> ();
		protected Config defaultConfig = new Config ();
	}
}

