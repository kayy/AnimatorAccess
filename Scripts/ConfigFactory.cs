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
using System.Collections.Generic;

namespace Scio.AnimatorAccessGenerator
{
	/// <summary>
	/// DO NOT EDIT THIS CLASS! THERE ARE EASIER WAYS. 
	/// Summary: Factory for providing configuration parameters depending on the class to generate. If you like to 
	/// change something, the appropriate way is:
	/// 
	/// Create a new C# file e.g. ConfigFactoryExt.cs and change the class name to be the other 
	/// (additional) part of this partial class definition.
	/// Then define a default constructor and set the porperties you like.
	/// 
	/// Example file ConfigFactoryExt.cs:
	/// -----------------------
	/// namespace Scio.AnimatorAccessGenerator {
	///     public partial class ConfigFactory {
	/// 		static ConfigFactory () {
    ///				ConfigFactory myFactory = new ConfigFactory ();
    ///				instance = myFactory;
    ///				myFactory.defaultConfig = new Config ();
    ///				Config specialConfig = new SpecialConfig ();
    ///				specialConfig.AnimatorStatePrefix = "MyAnim";
    ///				myFactory.configs ["ExamplePlayerAnimatorAccess"] = specialConfig;
	///         }
    ///		}
	///     public class SpecialConfig : Config {
	///     	public override string AnimatorStatePrefix {
	///     		get {
	///     			return "MyAnim";
	///     		}
	///     		// provide an empty setter to avoid that "MyAnim" is written back to the default config.
	///     		set {}
	///     	}
	///     }    
	/// -----------------------
	/// But you should really know what you are doing !
	/// If you want even more control, just create your own factory class and register it the way shown above.
	/// </summary>
	public interface IConfigFactory
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

	public partial class ConfigFactory : IConfigFactory
	{
		protected static IConfigFactory instance = null;

		public static IConfigFactory Instance {
			get {
				if (instance == null) {
					instance = new ConfigFactory ();
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

