// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections;

namespace AnimatorAccess
{
	/// <summary>
	/// Value object to provide all Animator state information from editor at runtime. Therefore information is 
	/// generated.
	/// </summary>
	public class StateInfo
	{
		/// <summary>
		/// Result of Animator.StringToHash (Name).
		/// </summary>
		public readonly int Id;
		/// <summary>
		/// The name of this Animator state.
		/// </summary>
		public readonly string Name;
		/// <summary>
		/// Layer index.
		/// </summary>
		public readonly int Layer;
		/// <summary>
		/// The name of the layer.
		/// </summary>
		public readonly string LayerName;

		public StateInfo (int id, int layer, string layerName, string stateName) {
			this.Id = id;
			this.Layer = layer;
			this.LayerName = layerName;
			this.Name = stateName;
		}

		public override string ToString () {
			return string.Format ("[State '{3}' id={0}, layer({1})='{2}']", Id, Layer, LayerName, Name);
		}
	}
}

