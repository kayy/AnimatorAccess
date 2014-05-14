// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections.Generic;

public partial class AnimatorWrapperGeneratorConfigFactory
{
	static AnimatorWrapperGeneratorConfigFactory instance = null;

	public static AnimatorWrapperGeneratorConfig Get (string className) {
		if (instance == null) {
			instance = new AnimatorWrapperGeneratorConfigFactory ();
		}
		if (instance.configs.ContainsKey (className)) {
			AnimatorWrapperGeneratorConfig c = instance.configs [className];
			Log.Debug ("Using special config for " + className + ": " + c.ToString ());
			return c;
		}
		return instance.defaultConfig;
	}

	Dictionary<string, AnimatorWrapperGeneratorConfig> configs = new Dictionary<string, AnimatorWrapperGeneratorConfig> ();
	AnimatorWrapperGeneratorConfig defaultConfig = new AnimatorWrapperGeneratorConfig ();
}

