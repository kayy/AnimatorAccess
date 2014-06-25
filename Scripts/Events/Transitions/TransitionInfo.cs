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
		/// <summary>
		/// Atomic flag as defined in inspector.
		/// </summary>
		public bool Atomic;
		/// <summary>
		/// Duration of the transition.
		/// </summary>
		public float Duration;
		/// <summary>
		/// Mute flag as defined in inspector.
		/// </summary>
		public bool Mute;
		/// <summary>
		/// Offset of this transition.
		/// </summary>
		public float Offset;
		/// <summary>
		/// Solo flag as defined in inspector.
		/// </summary>
		public bool Solo;

		public TransitionInfo (int id, string name, int layer, string layerName, int sourceId, int destId) {
			this.Id = id;
			this.Name = name;
			this.Layer = layer;
			this.LayerName = layerName;
			this.SourceId = sourceId;
			this.DestId = destId;
		}

		public TransitionInfo (int id, string name, int layer, string layerName, int sourceId, int destId, 
				bool atomic, float duration, bool mute, float offset, bool solo) {
			this.Id = id;
			this.Name = name;
			this.Layer = layer;
			this.LayerName = layerName;
			this.SourceId = sourceId;
			this.DestId = destId;
			this.Atomic = atomic;
			this.Duration = duration;
			this.Mute = mute;
			this.Offset = offset;
			this.Solo = solo;		
		}

		public override string ToString () {
			return string.Format ("[Transition '{1}' id={0} ({4} -> {5}), layer({2})={3}]", Id, Name, Layer, LayerName, SourceId, DestId);
		}
	}
}

