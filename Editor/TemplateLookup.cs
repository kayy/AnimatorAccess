// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Scio.CodeGeneration;

namespace Scio.AnimatorAccessGenerator
{
	public class TemplateLookup
	{
		string templateDir = "";

		public TemplateEngineConfig TemplateConfig { get; private set; }
		Config config;

		public TemplateLookup (Config config) {
			this.TemplateConfig = new TemplateEngineConfig ();
			this.config = config;
		}

		public CodeGeneratorResult GetPathToTemplate (string className) {
			CodeGeneratorResult result = new CodeGeneratorResult ();
			templateDir = Preferences.GetString (Preferences.Key.TemplateDir);
			if (string.IsNullOrEmpty (templateDir) || !Directory.Exists (templateDir)) {
				result = SearchTemplateDirectory (result);
				if (result.NoSuccess) {
					return result;
				}
			} else {
				string classSpecificTemplate = Path.Combine (templateDir, className + ".txt");
				if (File.Exists (classSpecificTemplate)) {
					TemplateConfig.TemplatePath = classSpecificTemplate;
					return result;
				} else {
					string defaultTemplate = Path.Combine (templateDir, config.GetDefaultTemplateFileName ());
					if (File.Exists (defaultTemplate)) {
						TemplateConfig.TemplatePath = defaultTemplate;
						return result;
					} else {
						result = SearchTemplateDirectory (result);
						if (result.NoSuccess) {
							return result;
						}
					}
				}
			}
			string defaultTemplate2 = Path.Combine (templateDir, config.GetDefaultTemplateFileName ());
			if (!File.Exists (defaultTemplate2)) {
				return result.SetError ("Default Template Not Found", "The default template file " + config.GetDefaultTemplateFileName () + " could not be found. Path: " + defaultTemplate2);
			}
			TemplateConfig.TemplatePath = defaultTemplate2;
			return result;
		}

		CodeGeneratorResult SearchTemplateDirectory (CodeGeneratorResult result) {
			templateDir = "";
			TemplateConfig.TemplatePath = "";
			string searchRoot = Path.Combine (Application.dataPath, Manager.SharedInstance.InstallDir);
			Logger.Debug ("Searching for default template in " + searchRoot);
			string[] files = Directory.GetFiles (searchRoot, config.GetDefaultTemplateFileName (), SearchOption.AllDirectories);
			if (files.Length == 0) {
				// fallback, scan all directories under Assets folder
				files = Directory.GetFiles (Application.dataPath, config.GetDefaultTemplateFileName (), SearchOption.AllDirectories);
			}
			if (files.Length == 0) {
				return result.SetError ("Template Directory Not Found", "The default template " + config.GetDefaultTemplateFileName () + "could not be found anywhere under your Assets directory.");
			} else if (files.Length > 1) {
				Logger.Info ("More than one default template found. Searching the best match");
				string rootDir = config.PathToTemplateDirectory;
				foreach (string item in files) {
					if (item.Contains (rootDir)) {
						TemplateConfig.TemplatePath = item;
						break;
					}
				}
				if (string.IsNullOrEmpty (TemplateConfig.TemplatePath)) {
					TemplateConfig.TemplatePath = files [0];
					Logger.Debug ("More than one default template found but non of them matching the path " + rootDir);
				}
			} else {
				TemplateConfig.TemplatePath = files [0];
			}
			templateDir = Path.GetDirectoryName (TemplateConfig.TemplatePath);
			Preferences.SetString (Preferences.Key.TemplateDir, templateDir);
			return result;
		}
	
	}
}

