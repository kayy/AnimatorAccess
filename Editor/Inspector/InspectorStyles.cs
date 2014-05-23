// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections;

namespace Scio.AnimatorWrapper {
	
	public static class InspectorStyles
	{
		public static GUIStyle buttonDisabled = null;
		
		static InspectorStyles () {
			if (buttonDisabled == null) {
				buttonDisabled = new GUIStyle ("button");
				buttonDisabled.name = "disabled";
				//				styleDisabled.font = GUI.skin.font;
				buttonDisabled.normal.textColor = Color.gray;
			}
			
		}
	}
}

