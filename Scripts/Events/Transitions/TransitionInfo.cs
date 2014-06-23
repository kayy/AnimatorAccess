// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections;

namespace AnimatorAccess
{
	/// <summary>
	/// Value object to provide all Animator transition information from editor at runtime. Therefore information is 
	/// generated.
	/// </summary>
	public class TransitionInfo
	{
		/// <summary>
		/// Transition name hash ID from 'Soure -> Destination'.
		/// </summary>
		public readonly int Id;
		/// <summary>
		/// The name as 'Soure -> Destination'.
		/// </summary>
		public readonly string Name;
		/// <summary>
		/// The layer index.
		/// </summary>
		public readonly int Layer;
		/// <summary>
		/// The name of the layer.
		/// </summary>
		public readonly string LayerName;
		/// <summary>
		/// The source state identifier.
		/// </summary>
		public readonly int SourceId;
		/// <summary>
		/// The destination state identifier.
		/// </summary>
		public readonly int DestId;
		
		public TransitionInfo (int id, string name, int layer, string layerName, int sourceId, int destId)
		{
			this.Id = id;
			this.Name = name;
			this.Layer = layer;
			this.LayerName = layerName;
			this.SourceId = sourceId;
			this.DestId = destId;
		}

		public override string ToString () {
			return string.Format ("[Transition '{1}' id={0} ({4} -> {5}), layer({2})={3}]", Id, Name, Layer, LayerName, SourceId, DestId);
		}
	}
}

