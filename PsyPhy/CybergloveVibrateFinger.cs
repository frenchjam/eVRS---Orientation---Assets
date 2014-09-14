// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.
// Modified by PsyPhy (J. McIntyre) to allow for action to occur onEnter, onExit or onUpdate.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("A surrogate for the cyberglove routine.")]
	public class CybergloveVibrateFinger : FsmStateAction
	{
		public float force;
		public enum Finger {Thumb, Index, Middle, Ring, Pinky, Palm}
		public Finger finger;
		public bool everyFrame;
		
		private string indicatorObjectName;
		private FsmGameObject indicatorObject;
		private string fingerName;
		
		public override void Reset()
		{
			// Find the object by name, so that we don't have to reconnect each time the prefab is used.
			indicatorObjectName = "Touch Stimuli Indicator";
			indicatorObject = GameObject.Find ( indicatorObjectName );
			everyFrame = true;
		}
		
		public override void OnEnter()
		{
			fingerName = finger.ToString();			
			var fingerObject = indicatorObject.Value.transform.FindChild (fingerName);
			if ( force > 0.5f ) fingerObject.renderer.material.SetColor ( "_Color", Color.magenta );
			else fingerObject.renderer.material.SetColor ( "_Color", Color.gray );
			
		}
		public override void OnExit()
		{
		
			// This works fine if one exits the state that started the vibration.
			// But if one exits the state by stopping the Unity player, one gets an error about a GameObject that has been destroyed.
			if ( indicatorObject != null ) {
				var fingerObject = indicatorObject.Value.transform.FindChild (fingerName);
				if ( fingerObject != null ) fingerObject.renderer.material.SetColor ( "_Color", Color.gray );
			}
		}
		
		
	}
}