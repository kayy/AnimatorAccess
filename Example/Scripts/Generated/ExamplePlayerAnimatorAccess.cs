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


namespace AnimatorAccess {
    [Scio.CodeGeneration.GeneratedClassAttribute ("06/03/2014 21:05:22")]
	/// <summary>
	/// Convenience class to access Animator states and parameters.
	/// DON'T EDIT! Your changes will be lost when this class is regenerated.
	/// </summary>
	public class ExamplePlayerAnimatorAccess : BaseAnimatorAccess
    {
        private Animator animator;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Walk
		/// </summary>
		public int walk;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Idle
		/// </summary>
		public int idle;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Yawn
		/// </summary>
		public int yawn;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Jump
		/// </summary>
		public int jump;
		
        /// <summary>
		/// Hash of parameter Speed
		/// </summary>
		public int speed;
		
        /// <summary>
		/// Hash of parameter JumpTrigger
		/// </summary>
		public int jumpTrigger;
		
        /// <summary>
		/// Hash of parameter yawnTrigger
		/// </summary>
		public int yawnTrigger;
		
		
		
		/// <summary>
		/// Access to parameter Speed, default: 0
		/// </summary>
		public float Speed { 
			get{ return animator.GetFloat (speed); }
			set{ animator.SetFloat (speed, value); }
		}
		
		/// <summary>
		/// Access to parameter JumpTrigger, default: (not available)
		/// </summary>
		public bool JumpTrigger { 
			set{ animator.SetTrigger (jumpTrigger); }
		}
		
		/// <summary>
		/// Access to parameter yawnTrigger, default: (not available)
		/// </summary>
		public bool YawnTrigger { 
			set{ animator.SetTrigger (yawnTrigger); }
		}
		
		
		public void Awake () { 
			animator = GetComponent<Animator> ();
			walk = Animator.StringToHash ("Base Layer.Walk");
			idle = Animator.StringToHash ("Base Layer.Idle");
			yawn = Animator.StringToHash ("Base Layer.Yawn");
			jump = Animator.StringToHash ("Base Layer.Jump");
			speed = Animator.StringToHash ("Speed");
			jumpTrigger = Animator.StringToHash ("JumpTrigger");
			yawnTrigger = Animator.StringToHash ("yawnTrigger");
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Walk").
		/// </summary>
		public bool IsWalk (int nameHash) { 
			 return nameHash == walk;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Idle").
		/// </summary>
		public bool IsIdle (int nameHash) { 
			 return nameHash == idle;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Yawn").
		/// </summary>
		public bool IsYawn (int nameHash) { 
			 return nameHash == yawn;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Jump").
		/// </summary>
		public bool IsJump (int nameHash) { 
			 return nameHash == jump;
		}
		
		public void SetSpeed (float newValue, float dampTime, float deltaTime) { 
			animator.SetFloat (speed, newValue, dampTime, deltaTime);
		}
		
        
    }
}


