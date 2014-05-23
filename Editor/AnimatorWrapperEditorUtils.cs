// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using UnityEditor;
using AnimatorAccess;

public static class AnimatorWrapperEditorUtils
{
	public static BaseAnimatorAccess GetActiveAnimatorAccessComponent () {
		BaseAnimatorAccess a = null;
		if (Selection.activeGameObject != null) {
			a = Selection.activeGameObject.GetComponent<BaseAnimatorAccess> ();
		}
		return a;
	}
	

}

