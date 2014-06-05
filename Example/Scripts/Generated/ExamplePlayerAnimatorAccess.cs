//  GENERATED CODE ==> EDITS WILL BE LOST AFTER NEXT GENERATION!

using UnityEngine;
using System.Collections;


namespace AnimatorAccess {
    [Scio.CodeGeneration.GeneratedClassAttribute ("06/05/2014 19:30:53")]
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
        private Animator animator;
		
        public Hashtable stateDictionary = new Hashtable ();
		
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
			stateIdWalking = Animator.StringToHash ("Base Layer.Walking");
			stateDictionary.Add (stateIdWalking, "Base Layer.Walking");
			stateIdIdle = Animator.StringToHash ("Base Layer.Idle");
			stateDictionary.Add (stateIdIdle, "Base Layer.Idle");
			stateIdYawning = Animator.StringToHash ("Base Layer.Yawning");
			stateDictionary.Add (stateIdYawning, "Base Layer.Yawning");
			stateIdJumping = Animator.StringToHash ("Base Layer.Jumping");
			stateDictionary.Add (stateIdJumping, "Base Layer.Jumping");
			stateIdRot_Rotate_Left = Animator.StringToHash ("Rot.Rotate-Left");
			stateDictionary.Add (stateIdRot_Rotate_Left, "Rot.Rotate-Left");
			stateIdRot_Rotate_Right = Animator.StringToHash ("Rot.Rotate-Right");
			stateDictionary.Add (stateIdRot_Rotate_Right, "Rot.Rotate-Right");
			stateIdRot_Centered = Animator.StringToHash ("Rot.Centered");
			stateDictionary.Add (stateIdRot_Centered, "Rot.Centered");
			paramIdSpeed = Animator.StringToHash ("Speed");
			paramIdJumpTrigger = Animator.StringToHash ("JumpTrigger");
			paramIdYawnTrigger = Animator.StringToHash ("YawnTrigger");
			paramIdRotate = Animator.StringToHash ("Rotate");
		}
		
		public string StateIdToName (int id) { 
			if (stateDictionary.ContainsKey (id)) {
				return (string)stateDictionary[id];
			}
			return "";
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
		
        
    }
}


