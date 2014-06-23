// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections;

namespace AnimatorAccess
{
	public class TransitionInfo
	{
		public int id;
		public string name;
		public int layer;
		public string layerName;
		public int sourceId;
		public int destId;
		
		public TransitionInfo (int id, string name, int layer, string layerName, int sourceId, int destId)
		{
			this.id = id;
			this.name = name;
			this.layer = layer;
			this.layerName = layerName;
			this.sourceId = sourceId;
			this.destId = destId;
		}

		public override string ToString () {
			return string.Format ("[Transition '{1}' id={0} ({4} -> {5}), layer({2})={3}]", id, name, layer, layerName, sourceId, destId);
		}
	}
}

