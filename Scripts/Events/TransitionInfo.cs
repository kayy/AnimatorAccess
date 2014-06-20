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
		public int layer;
		public string layerName;
		public int sourceId;
		public int destId;
		
		public TransitionInfo (int id, int layer, string layerName, int sourceId, int destId)
		{
			this.id = id;
			this.layer = layer;
			this.layerName = layerName;
			this.sourceId = sourceId;
			this.destId = destId;
		}
	}
}

