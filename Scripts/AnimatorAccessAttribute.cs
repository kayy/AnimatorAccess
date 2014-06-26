// // Created by Kay
// // Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
//
using UnityEngine;
using System.Collections;

namespace AnimatorAccess
{
	/// <summary>
	/// NOT YET USED! 
	/// </summary>
	public class AnimatorAccessAttribute : Scio.CodeGeneration.GeneratedClassAttribute
	{
		public readonly string allTransitionsHash;

		public AnimatorAccessAttribute (string creationDate, string allTransitionsHash) : base (creationDate) {
			this.allTransitionsHash = allTransitionsHash;
		}
	}
}

