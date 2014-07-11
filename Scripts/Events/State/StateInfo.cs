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
		/// <summary>
		/// Tag of the state.
		/// </summary>
		public readonly string Tag;
		/// <summary>
		/// Speed of the motion as defined in inspector.
		/// </summary>
		public readonly float Speed;
		/// <summary>
		/// Information about the underlying motion.
		/// </summary>
		public readonly StateMotionInfo Motion;
		/// <summary>
		/// The foot IK as defined in inspector.
		/// </summary>
		public readonly bool FootIK;
		/// <summary>
		/// The mirror as defined in inspector.
		/// </summary>
		public readonly bool Mirror;

		public StateInfo (int id, int layer, string layerName, string name, string tag, float speed, bool footIK, 
				bool mirror, string motionName, float duration = 0f) {
			this.Id = id;
			this.Layer = layer;
			this.LayerName = layerName;
			this.Name = name;
			this.Tag = tag;
			this.Speed = speed;
			this.FootIK = footIK;
			this.Mirror = mirror;
			this.Motion = new StateMotionInfo (motionName, duration);
		}
		
		public override string ToString () {
			return string.Format ("[State '{3}' id={0}, layer({1})='{2}']", Id, Layer, LayerName, Name);
		}
	}

	/// <summary>
	/// Some information of the motion gathered from UnityEngine.Motion.
	/// </summary>
	public class StateMotionInfo
	{
		/// <summary>
		/// The name of the motion behind the state.
		/// </summary>
		public readonly string Name;
		/// <summary>
		/// The average duration of this motion.
		/// </summary>
		public readonly float Duration;

		public StateMotionInfo (string name, float duration) {
			this.Name = name;
			this.Duration = duration;
		}
	}
}

