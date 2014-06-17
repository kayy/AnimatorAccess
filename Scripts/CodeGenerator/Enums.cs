// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections;

namespace Scio.AnimatorAccessGenerator
{
	/// <summary>
	/// Defines where checking for state changes or transitions should take place.
	/// </summary>
	public enum StateEventHandlingMethod
	{
		None = 0,
		FixedUpdate = 1,
		Update = 2,
	}
}
