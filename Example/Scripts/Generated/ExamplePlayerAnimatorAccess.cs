// GENERATED CODE ==> EDITS WILL BE LOST AFTER NEXT GENERATION!
// Version for Mac / UNIX 

using UnityEngine;


namespace AnimatorAccess {
    [Scio.CodeGeneration.GeneratedClassAttribute ("07/11/2014 15:00:38")]
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
        /// <summary>
		/// Hash of Animator state Base Layer.Walking
		/// </summary>
		public readonly int stateIdWalking = -2010423537;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Idle
		/// </summary>
		public readonly int stateIdIdle = 1432961145;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Yawning
		/// </summary>
		public readonly int stateIdYawning = -117804301;
		
        /// <summary>
		/// Hash of Animator state Base Layer.Jumping
		/// </summary>
		public readonly int stateIdJumping = -1407378526;
		
        /// <summary>
		/// Hash of Animator state Rot.Rotate-Left
		/// </summary>
		public readonly int stateIdRot_Rotate_Left = -1817809755;
		
        /// <summary>
		/// Hash of Animator state Rot.Rotate-Right
		/// </summary>
		public readonly int stateIdRot_Rotate_Right = 1375079058;
		
        /// <summary>
		/// Hash of Animator state Rot.Centered
		/// </summary>
		public readonly int stateIdRot_Centered = -1799351532;
		
        /// <summary>
		/// Hash of parameter Speed
		/// </summary>
		public readonly int paramIdSpeed = -823668238;
		
        /// <summary>
		/// Hash of parameter JumpTrigger
		/// </summary>
		public readonly int paramIdJumpTrigger = 113680519;
		
        /// <summary>
		/// Hash of parameter YawnTrigger
		/// </summary>
		public readonly int paramIdYawnTrigger = 1330169897;
		
        /// <summary>
		/// Hash of parameter Rotate
		/// </summary>
		public readonly int paramIdRotate = 807753530;
		
		
		
		public override int AllTransitionsHash { 
			get{ return 859155473; }
		}
		
		
		public void Awake () { 
			animator = GetComponent<Animator> ();
		}
		
		public override void InitialiseEventManager () { 
			StateInfos.Add (-2010423537, new StateInfo (-2010423537, 0, "Base Layer", "Base Layer.Walking", "", 1f, false, false, "Walk"));
			StateInfos.Add (1432961145, new StateInfo (1432961145, 0, "Base Layer", "Base Layer.Idle", "", 1f, false, false, "Idle"));
			StateInfos.Add (-117804301, new StateInfo (-117804301, 0, "Base Layer", "Base Layer.Yawning", "", 1f, false, false, "Yawn"));
			StateInfos.Add (-1407378526, new StateInfo (-1407378526, 0, "Base Layer", "Base Layer.Jumping", "", 1f, false, false, "Jump"));
			StateInfos.Add (-1817809755, new StateInfo (-1817809755, 1, "Rot", "Rot.Rotate-Left", "", 1f, false, false, "Rotate-Left"));
			StateInfos.Add (1375079058, new StateInfo (1375079058, 1, "Rot", "Rot.Rotate-Right", "", 1f, false, false, "Rotate-Right"));
			StateInfos.Add (-1799351532, new StateInfo (-1799351532, 1, "Rot", "Rot.Centered", "", 1f, false, false, "Centered"));
			TransitionInfos.Add (708569559, new TransitionInfo (708569559, "Base Layer.Walking -> Base Layer.Idle", 0, "Base Layer", -2010423537, 1432961145, true, 0.2068965f, false, 0f, false));
			TransitionInfos.Add (-2057610033, new TransitionInfo (-2057610033, "Base Layer.Walking -> Base Layer.Jumping", 0, "Base Layer", -2010423537, -1407378526, true, 0.2068965f, false, 0f, false));
			TransitionInfos.Add (856518066, new TransitionInfo (856518066, "Base Layer.Idle -> Base Layer.Walking", 0, "Base Layer", 1432961145, -2010423537, true, 0.09230769f, false, 0f, false));
			TransitionInfos.Add (1138507854, new TransitionInfo (1138507854, "Base Layer.Idle -> Base Layer.Yawning", 0, "Base Layer", 1432961145, -117804301, true, 0.09230769f, false, 0f, false));
			TransitionInfos.Add (389753119, new TransitionInfo (389753119, "Base Layer.Idle -> Base Layer.Jumping", 0, "Base Layer", 1432961145, -1407378526, true, 0.09230769f, false, 0f, false));
			TransitionInfos.Add (781957174, new TransitionInfo (781957174, "Base Layer.Yawning -> Base Layer.Idle", 0, "Base Layer", -117804301, 1432961145, true, 0.1090909f, false, 0f, false));
			TransitionInfos.Add (1298684863, new TransitionInfo (1298684863, "Base Layer.Jumping -> Base Layer.Idle", 0, "Base Layer", -1407378526, 1432961145, true, 0.3157895f, false, 0f, false));
			TransitionInfos.Add (-1019719959, new TransitionInfo (-1019719959, "Base Layer.Jumping -> Base Layer.Walking", 0, "Base Layer", -1407378526, -2010423537, true, 0.3157895f, false, 0f, false));
			TransitionInfos.Add (796133751, new TransitionInfo (796133751, "Rot.Rotate-Left -> Rot.Rotate-Right", 1, "Rot", -1817809755, 1375079058, true, 0.75f, false, 0f, false));
			TransitionInfos.Add (-1706334441, new TransitionInfo (-1706334441, "Rot.Rotate-Left -> Rot.Centered", 1, "Rot", -1817809755, -1799351532, false, 0.4559997f, false, 0f, false));
			TransitionInfos.Add (-1985854260, new TransitionInfo (-1985854260, "Rot.Rotate-Right -> Rot.Rotate-Left", 1, "Rot", 1375079058, -1817809755, true, 0.75f, false, 0f, false));
			TransitionInfos.Add (-1394703878, new TransitionInfo (-1394703878, "Rot.Rotate-Right -> Rot.Centered", 1, "Rot", 1375079058, -1799351532, false, 0.2279998f, false, 0f, false));
			TransitionInfos.Add (699184725, new TransitionInfo (699184725, "Rot.Centered -> Rot.Rotate-Right", 1, "Rot", -1799351532, 1375079058, true, 0.7415018f, false, 0.001918154f, false));
			TransitionInfos.Add (-825030163, new TransitionInfo (-825030163, "Rot.Centered -> Rot.Rotate-Left", 1, "Rot", -1799351532, -1817809755, true, 0.25f, false, 0f, false));
		}
		
		/// <summary>
		/// true if the current Animator state of layer 0 is  "Base Layer.Walking".
		/// </summary>
		public bool IsWalking () { 
			return stateIdWalking == animator.GetCurrentAnimatorStateInfo (0).nameHash;
		}
		
		/// <summary>
		/// true if the given (state) nameHash equals Animator.StringToHash ("Base Layer.Walking").
		/// </summary>
		public bool IsWalking (int nameHash) { 
			return nameHash == stateIdWalking;
		}
		
		/// <summary>
		/// true if the current Animator state of layer 0 is  "Base Layer.Idle".
		/// </summary>
		public bool IsIdle () { 
			return stateIdIdle == animator.GetCurrentAnimatorStateInfo (0).nameHash;
		}
		
		/// <summary>
		/// true if the given (state) nameHash equals Animator.StringToHash ("Base Layer.Idle").
		/// </summary>
		public bool IsIdle (int nameHash) { 
			return nameHash == stateIdIdle;
		}
		
		/// <summary>
		/// true if the current Animator state of layer 0 is  "Base Layer.Yawning".
		/// </summary>
		public bool IsYawning () { 
			return stateIdYawning == animator.GetCurrentAnimatorStateInfo (0).nameHash;
		}
		
		/// <summary>
		/// true if the given (state) nameHash equals Animator.StringToHash ("Base Layer.Yawning").
		/// </summary>
		public bool IsYawning (int nameHash) { 
			return nameHash == stateIdYawning;
		}
		
		/// <summary>
		/// true if the current Animator state of layer 0 is  "Base Layer.Jumping".
		/// </summary>
		public bool IsJumping () { 
			return stateIdJumping == animator.GetCurrentAnimatorStateInfo (0).nameHash;
		}
		
		/// <summary>
		/// true if the given (state) nameHash equals Animator.StringToHash ("Base Layer.Jumping").
		/// </summary>
		public bool IsJumping (int nameHash) { 
			return nameHash == stateIdJumping;
		}
		
		/// <summary>
		/// true if the current Animator state of layer 1 is  "Rot.Rotate-Left".
		/// </summary>
		public bool IsRot_Rotate_Left () { 
			return stateIdRot_Rotate_Left == animator.GetCurrentAnimatorStateInfo (1).nameHash;
		}
		
		/// <summary>
		/// true if the given (state) nameHash equals Animator.StringToHash ("Rot.Rotate-Left").
		/// </summary>
		public bool IsRot_Rotate_Left (int nameHash) { 
			return nameHash == stateIdRot_Rotate_Left;
		}
		
		/// <summary>
		/// true if the current Animator state of layer 1 is  "Rot.Rotate-Right".
		/// </summary>
		public bool IsRot_Rotate_Right () { 
			return stateIdRot_Rotate_Right == animator.GetCurrentAnimatorStateInfo (1).nameHash;
		}
		
		/// <summary>
		/// true if the given (state) nameHash equals Animator.StringToHash ("Rot.Rotate-Right").
		/// </summary>
		public bool IsRot_Rotate_Right (int nameHash) { 
			return nameHash == stateIdRot_Rotate_Right;
		}
		
		/// <summary>
		/// true if the current Animator state of layer 1 is  "Rot.Centered".
		/// </summary>
		public bool IsRot_Centered () { 
			return stateIdRot_Centered == animator.GetCurrentAnimatorStateInfo (1).nameHash;
		}
		
		/// <summary>
		/// true if the given (state) nameHash equals Animator.StringToHash ("Rot.Centered").
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
			CheckForAnimatorStateChanges ();
		}
		
        
    }
}


