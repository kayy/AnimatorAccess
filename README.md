AnimatorWrapper
===============

Code generator utility for Unity game engine. Creates a class for convenient access Animator states and parameters.

Installation:
- Copy the Editor directory to some location under your Assets directory

Quick start code generation:
- In the hierachy view select a game object that contains an Animator component having a valid controller
- Go to the new menu item Tools/Generate Animator Wrapper
- Select a file name and the output directory where to place the C# file

Usage example:
- Generated class is 'UfoAnimatorWrapper.cs'
- Game object is 'Ufo'
- Another component 'UfoController' should handle animation stuff in its Update method
- Animator states are 'Idle' and 'Fly'
- Animator parameters are 'HoverRandom' (trigger) and 'Speed' (float)

To use it define a member in UfoController:
	UfoAnimatorWrapper animatorWrapper;	
and initialise it in Awake () using the constructor taking an Animator instance:
	animatorWrapper = new MyAnimatorWrapper (GetComponent<Animator> ());
		
Now you have convenient access to all parameters and animator states:
	void Update () {
		animatorStateInfo = animator.GetCurrentAnimatorStateInfo (0);
		int currentAnimationHash = animatorStateInfo.nameHash;
		if (animatorWrapper.IsIdle (currentAnimationHash)) {
			animatorWrapper.HoverRandom = true;
			animatorWrapper.Speed = 5f;
		}

Advanced Topics
===============
If you modify, add or remove states or parameters in the animator view you should regenerate your wrapper class.
To do so just repeat the steps decribed above. When a wrapper class is recreated, the generator performs some 
basic code analysis to help you migrating your code. If for example properties are no longer valid, a warning 
shows up so you can decide to cancel the generation. Especially in the case of renaming states or parameters it 
might be more efficient to do some refactoring in the IDE before regenerating the code. Thus you don't have to
fix a bunch of compiler warnings afterwards.

Layer names are prepended by default for all layers above the first one i.e. first layer members don't have the 
layer prefix but all higher levels do so. If you had only one layer and add another one let's call it 'Layer2', 
all your existing state checking methods like 'IsIdle' from layer 1 stay with the same name. Starting with 
Layer2 the prefix will be set e.g. 'IsLayer2Shooting'. 
