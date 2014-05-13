// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System.Collections.Generic;

public class LayerStatesList
{
	public int LayerIndex { get; set; }
	public string LayerName { get; set; }

	List<string> layerStates = new List<string> ();
	public List<string> LayerStates {
		get { return layerStates; }
	}

	public LayerStatesList (int index, string name) {
		LayerIndex = index;
		LayerName = name;
	}
}

