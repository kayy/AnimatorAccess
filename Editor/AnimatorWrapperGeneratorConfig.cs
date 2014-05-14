// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections;

public partial class AnimatorWrapperGeneratorConfig 
{
	/// <summary>
	/// If true, the layer name will be prepended even for layer 0.
	/// </summary>
	public bool ForceLayerPrefix = false;
	/// <summary>
	/// Optional prefix for all methods that check animation state e.g. Is<Prefix>Idle ()
	/// </summary>
	public string AnimationStatePrefix = "";
	/// <summary>
	/// Optional prefix for parameter access properties, e.g. float <Prefix>Speed
	/// </summary>
	public string ParameterPrefix = "";
	
	public override string ToString () {
		return string.Format ("[AnimatorWrapperGeneratorConfig: ForceLayerPrefix={0}, AnimationStatePrefix={1}, ParameterPrefix={2}]", ForceLayerPrefix, AnimationStatePrefix, ParameterPrefix);
	}
	
}

