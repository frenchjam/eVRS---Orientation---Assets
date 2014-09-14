// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.
// Modified by PsyPhy (J. McIntyre) to allow for action to occur onEnter, onExit or onUpdate.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("A surrogate for the cyberglove routine.")]
	public class PsyPhyGloveVibrateFinger : FsmStateAction
	{
		public float force;
		public enum Finger {Thumb, Index, Middle, Ring, Pinky, Palm}
		public Finger finger;
		private string fingerName;
		private FsmGameObject indicatorObject;
		private string handObjectName;
		
		public override void Reset()
		{
			handObjectName = "Touch Stimuli Indicator";
			indicatorObject = GameObject.Find ( handObjectName );
		}
		
		public override void OnEnter()
		{
			if ( finger == Finger.Thumb ) fingerName = "Thumb";
			if ( finger == Finger.Index ) fingerName = "Index";
			if ( finger == Finger.Middle ) fingerName = "Middle";
			if ( finger == Finger.Ring ) fingerName = "Ring";
			if ( finger == Finger.Pinky ) fingerName = "Pinky";
			if ( finger == Finger.Palm ) fingerName = "Palm";
			
			var fingerObject = indicatorObject.Value.transform.FindChild (fingerName);
			if ( force > 0.5f ) fingerObject.renderer.material.SetColor ( "_Color", Color.magenta );
			else fingerObject.renderer.material.SetColor ( "_Color", Color.gray );
			
		}
		
	}
}