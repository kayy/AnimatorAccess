// The MIT License (MIT)
// 
// Copyright (c) 2014 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using UnityEngine;

namespace AnimatorAccess {
	/// <summary>
	/// Base class for all generated AnimatorAccess classes.
	/// </summary>
	public class BaseAnimatorAccess : MonoBehaviour 
	{
		/// <summary>
		/// Callback method for animator state changes.
		/// <param name="layer">Layer in which the state has changed.</param>
		/// <param name="newState">New state just entered.</param>
		/// <param name="previousState">Previous state.</param>
		/// </summary>
		public delegate void OnStateChangeHandler (int layer, int newState, int previousState);

		/// <summary>
		/// Occurs once for every change of an animator state. If there are more than one changes at a time in different
		/// layers, the listeners are called once for every single change.
		/// </summary>
		public event OnStateChangeHandler OnStateChange;

		int [] _internalPreviousLayerStates;
		int _internalLayerCount = -1;

		/// <summary>
		/// Checks for animator state changes if there are listeners registered in OnStateChange.
		/// </summary>
		/// <param name="animator">Animator instance for reading states of all layers.</param>
		public void CheckForAnimatorStateChanges (Animator animator) {
			if (OnStateChange != null) {
				if (_internalLayerCount < 0) {
					_internalLayerCount = animator.layerCount;
					_internalPreviousLayerStates = new int[animator.layerCount];
				}
				for (int layer = 0; layer < _internalLayerCount; layer++) {
					int current = animator.GetCurrentAnimatorStateInfo (layer).nameHash;
					if (current != _internalPreviousLayerStates [layer]) {
						OnStateChange (layer, current, _internalPreviousLayerStates [layer]);
						_internalPreviousLayerStates [layer] = current;
					}
					
				}
			}
		}
	}
}
