//  IMPORTANT: GENERATED CODE  ==>  DO NOT EDIT!
//  The MIT License (MIT)
//  
//  Copyright (c) 2014 kayy
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.

using UnityEngine;
using System.Collections;


namespace AnimatorAccess {
    [Scio.CodeGeneration.GeneratedClassAttribute ("06/04/2014 17:13:14")]
	/// <summary>
	/// Convenience class to access Animator states and parameters.
	/// DON'T EDIT! Your changes will be lost when this class is regenerated.
	/// </summary>
	public class ExamplePlayerAnimatorAccess : BaseAnimatorAccess
    {
        private Animator animator;
		
        public Hashtable stateDictionary = new Hashtable ();
		
        /// <summary>
		/// Hash of Animator state Base Layer.Walk
		/// </summary>
		public int stateIdWalk;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Idle
		/// </summary>
		public int stateIdIdle;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Yawn
		/// </summary>
		public int stateIdYawn;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Jump
		/// </summary>
		public int stateIdJump;
		
        /// <summary>
		/// Hash of parameter Speed
		/// </summary>
		public int paramIdSpeed;
		
        /// <summary>
		/// Hash of parameter JumpTrigger
		/// </summary>
		public int paramIdJumpTrigger;
		
        /// <summary>
		/// Hash of parameter yawnTrigger
		/// </summary>
		public int paramIdYawnTrigger;
		

		
		
		public void Awake () { 
			animator = GetComponent<Animator> ();
			stateIdWalk = Animator.StringToHash ("Base Layer.Walk");
			stateDictionary.Add (stateIdWalk, "Base Layer.Walk");
			stateIdIdle = Animator.StringToHash ("Base Layer.Idle");
			stateDictionary.Add (stateIdIdle, "Base Layer.Idle");
			stateIdYawn = Animator.StringToHash ("Base Layer.Yawn");
			stateDictionary.Add (stateIdYawn, "Base Layer.Yawn");
			stateIdJump = Animator.StringToHash ("Base Layer.Jump");
			stateDictionary.Add (stateIdJump, "Base Layer.Jump");
			paramIdSpeed = Animator.StringToHash ("Speed");
			paramIdJumpTrigger = Animator.StringToHash ("JumpTrigger");
			paramIdYawnTrigger = Animator.StringToHash ("yawnTrigger");
		}
		
		public string StateIdToName (int id) { 
			if (stateDictionary.ContainsKey (id)) {
				return (string)stateDictionary[id];
			}
			return "";
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Walk").
		/// </summary>
		public bool IsWalk (int nameHash) { 
			 return nameHash == stateIdWalk;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Idle").
		/// </summary>
		public bool IsIdle (int nameHash) { 
			 return nameHash == stateIdIdle;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Yawn").
		/// </summary>
		public bool IsYawn (int nameHash) { 
			 return nameHash == stateIdYawn;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Jump").
		/// </summary>
		public bool IsJump (int nameHash) { 
			 return nameHash == stateIdJump;
		}
		
		/// <summary>
		/// Set float parameter of Speed using damp and delta time .
		/// <param name="newValue">New value for float parameter Speed.</param>
		/// <param name="dampTime">The time allowed to parameter Speed to reach the value.</param>
		/// <param name="deltaTime">The current frame deltaTime.</param>
		/// </summary>
		public void SetSpeed (float newValue, float dampTime, float deltaTime) { 
			animator.SetFloat (paramIdSpeed, newValue, dampTime, deltaTime);
		}
		
		/// <summary>
		/// Set float value of parameter Speed.
		/// <param name="newValue">New value for float parameter Speed.</param>
		/// </summary>
		public void SetSpeed (float newValue) { 
			animator.SetFloat (paramIdSpeed, newValue);
		}
		
		/// <summary>
		/// Access to float parameter Speed, default is: 0.
		/// </summary>
		public float GetSpeed () { 
			return animator.GetFloat (paramIdSpeed);
		}
		
		/// <summary>
		/// Activate trigger of parameter JumpTrigger.
		/// </summary>
		public void SetJumpTrigger () { 
			animator.SetTrigger (paramIdJumpTrigger);
		}
		
		/// <summary>
		/// Activate trigger of parameter yawnTrigger.
		/// </summary>
		public void SetYawnTrigger () { 
			animator.SetTrigger (paramIdYawnTrigger);
		}
		
        
    }
}


