// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections;

namespace AnimatorAccess
{
	public class StateInfo
	{
		public int id;
		public int layer;
		public string layerName;
		public string stateName;

		public StateInfo (int id, int layer, string layerName, string stateName) {
			this.id = id;
			this.layer = layer;
			this.layerName = layerName;
			this.stateName = stateName;
		}

		public override string ToString () {
			return string.Format ("[State '{3}' id={0}, layer({1})='{2}']", id, layer, layerName, stateName);
		}
	}
}

