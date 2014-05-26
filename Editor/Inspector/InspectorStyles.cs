// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections;

namespace Scio.AnimatorWrapper {
	
	public static class InspectorStyles
	{
		public static GUIStyle ButtonDisabled = null;
		public static GUIStyle MidMiniButtonHighLighted = null;
		public static GUIStyle RightMiniButtonHighLighted = null;
		public static GUIStyle ButtonRegular = null;

		static InspectorStyles () {
			ButtonDisabled = new GUIStyle ("button");
			ButtonDisabled.name = "ButtonDisabled";
			ButtonDisabled.normal.textColor = Color.gray;
			MidMiniButtonHighLighted = new GUIStyle ("miniButtonMid");
			MidMiniButtonHighLighted.name = "MidMiniButtonHighLighted";
			MidMiniButtonHighLighted.normal.textColor = Color.white;
			RightMiniButtonHighLighted = new GUIStyle ("miniButtonRight");
			RightMiniButtonHighLighted.name = "RightMiniButtonHighLighted";
			RightMiniButtonHighLighted.normal.textColor = Color.white;
			ButtonRegular = new GUIStyle ("button");
		}
	}
}

