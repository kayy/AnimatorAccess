// GENERATED CODE ==> EDITS WILL BE LOST AFTER NEXT GENERATION!
// Version for Mac / UNIX 

using UnityEngine;
using System.Collections;


namespace AnimatorAccess {
    [Scio.CodeGeneration.GeneratedClassAttribute ("06/21/2014 13:47:52")]
	/// <summary>
	/// Convenience class to access Animator states and parameters.
	/// Edits will be lost when this class is regenerated. 
	/// Hint: Editing might be useful after renaming animator items in complex projects:
	///  - Right click on an obsolete member and select Refactor/Rename. 
	///  - Change it to the new name. 
	///  - Delete this member to avoid comile error CS0102 ... already contains a definition ...''. 
	/// </summary>
	public class ExamplePlayerAnimatorAccess : BaseAnimatorAccess
    {
        public Animator animator;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Walking
		/// </summary>
		public int stateIdWalking;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Idle
		/// </summary>
		public int stateIdIdle;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Yawning
		/// </summary>
		public int stateIdYawning;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Jumping
		/// </summary>
		public int stateIdJumping;
		
        /// <summary>
		/// Hash of Animator state Rot.Rotate-Left
		/// </summary>
		public int stateIdRot_Rotate_Left;
		
        /// <summary>
		/// Hash of Animator state Rot.Rotate-Right
		/// </summary>
		public int stateIdRot_Rotate_Right;
		
        /// <summary>
		/// Hash of Animator state Rot.Centered
		/// </summary>
		public int stateIdRot_Centered;
		
        /// <summary>
		/// Hash of parameter Speed
		/// </summary>
		public int paramIdSpeed;
		
        /// <summary>
		/// Hash of parameter JumpTrigger
		/// </summary>
		public int paramIdJumpTrigger;
		
        /// <summary>
		/// Hash of parameter YawnTrigger
		/// </summary>
		public int paramIdYawnTrigger;
		
        /// <summary>
		/// Hash of parameter Rotate
		/// </summary>
		public int paramIdRotate;
		
		
		
		
		public void Awake () { 
			animator = GetComponent<Animator> ();
			Initialise (animator);
			stateIdWalking = Animator.StringToHash ("Base Layer.Walking");
			stateDictionary.Add (-2010423537, new StateInfo (-2010423537, 0, "Base Layer", "Base Layer.Walking"));
			stateIdIdle = Animator.StringToHash ("Base Layer.Idle");
			stateDictionary.Add (1432961145, new StateInfo (1432961145, 0, "Base Layer", "Base Layer.Idle"));
			stateIdYawning = Animator.StringToHash ("Base Layer.Yawning");
			stateDictionary.Add (-117804301, new StateInfo (-117804301, 0, "Base Layer", "Base Layer.Yawning"));
			stateIdJumping = Animator.StringToHash ("Base Layer.Jumping");
			stateDictionary.Add (-1407378526, new StateInfo (-1407378526, 0, "Base Layer", "Base Layer.Jumping"));
			stateIdRot_Rotate_Left = Animator.StringToHash ("Rot.Rotate-Left");
			stateDictionary.Add (-1817809755, new StateInfo (-1817809755, 1, "Rot", "Rot.Rotate-Left"));
			stateIdRot_Rotate_Right = Animator.StringToHash ("Rot.Rotate-Right");
			stateDictionary.Add (1375079058, new StateInfo (1375079058, 1, "Rot", "Rot.Rotate-Right"));
			stateIdRot_Centered = Animator.StringToHash ("Rot.Centered");
			stateDictionary.Add (-1799351532, new StateInfo (-1799351532, 1, "Rot", "Rot.Centered"));
			paramIdSpeed = Animator.StringToHash ("Speed");
			paramIdJumpTrigger = Animator.StringToHash ("JumpTrigger");
			paramIdYawnTrigger = Animator.StringToHash ("YawnTrigger");
			paramIdRotate = Animator.StringToHash ("Rotate");
			transitionInfos.Add (708569559, new TransitionInfo (708569559, 0, "Base Layer", -2010423537, 1432961145));
			transitionInfos.Add (856518066, new TransitionInfo (856518066, 0, "Base Layer", 1432961145, -2010423537));
			transitionInfos.Add (1138507854, new TransitionInfo (1138507854, 0, "Base Layer", 1432961145, -117804301));
			transitionInfos.Add (389753119, new TransitionInfo (389753119, 0, "Base Layer", 1432961145, -1407378526));
			transitionInfos.Add (781957174, new TransitionInfo (781957174, 0, "Base Layer", -117804301, 1432961145));
			transitionInfos.Add (1298684863, new TransitionInfo (1298684863, 0, "Base Layer", -1407378526, 1432961145));
			transitionInfos.Add (796133751, new TransitionInfo (796133751, 1, "Rot", -1817809755, 1375079058));
			transitionInfos.Add (-1706334441, new TransitionInfo (-1706334441, 1, "Rot", -1817809755, -1799351532));
			transitionInfos.Add (-1985854260, new TransitionInfo (-1985854260, 1, "Rot", 1375079058, -1817809755));
			transitionInfos.Add (-1394703878, new TransitionInfo (-1394703878, 1, "Rot", 1375079058, -1799351532));
			transitionInfos.Add (699184725, new TransitionInfo (699184725, 1, "Rot", -1799351532, 1375079058));
			transitionInfos.Add (-825030163, new TransitionInfo (-825030163, 1, "Rot", -1799351532, -1817809755));
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Walking").
		/// </summary>
		public bool IsWalking (int nameHash) { 
			 return nameHash == stateIdWalking;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Idle").
		/// </summary>
		public bool IsIdle (int nameHash) { 
			 return nameHash == stateIdIdle;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Yawning").
		/// </summary>
		public bool IsYawning (int nameHash) { 
			 return nameHash == stateIdYawning;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Base Layer.Jumping").
		/// </summary>
		public bool IsJumping (int nameHash) { 
			 return nameHash == stateIdJumping;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Rot.Rotate-Left").
		/// </summary>
		public bool IsRot_Rotate_Left (int nameHash) { 
			 return nameHash == stateIdRot_Rotate_Left;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Rot.Rotate-Right").
		/// </summary>
		public bool IsRot_Rotate_Right (int nameHash) { 
			 return nameHash == stateIdRot_Rotate_Right;
		}
		
		/// <summary>
		/// true if nameHash equals Animator.StringToHash ("Rot.Centered").
		/// </summary>
		public bool IsRot_Centered (int nameHash) { 
			 return nameHash == stateIdRot_Centered;
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
		/// Activate trigger of parameter YawnTrigger.
		/// </summary>
		public void SetYawnTrigger () { 
			animator.SetTrigger (paramIdYawnTrigger);
		}
		
		/// <summary>
		/// Set integer value of parameter Rotate.
		/// <param name="newValue">New value for integer parameter Rotate.</param>
		/// </summary>
		public void SetRotate (int newValue) { 
			animator.SetInteger (paramIdRotate, newValue);
		}
		
		/// <summary>
		/// Access to integer parameter Rotate, default is: 1.
		/// </summary>
		public int GetRotate () { 
			return animator.GetInteger (paramIdRotate);
		}
		
		private void FixedUpdate () { 
			CheckForAnimatorStateChanges (animator);
		}
		
        
    }
}


