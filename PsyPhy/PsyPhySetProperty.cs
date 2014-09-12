// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.
// Modified by PsyPhy (J. McIntyre) to allow for action to occur onEnter, onExit or onUpdate.

#if !UNITY_FLASH

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UnityObject)]
	[Tooltip("Sets the value of any public property or field on the targeted Unity Object. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
	public class PsyPhySetProperty : FsmStateAction
	{
		public FsmProperty targetProperty;
		public enum When {OnEnter, OnExit, OnUpdate};
		public When when;

		public override void Reset()
		{
			targetProperty = new FsmProperty {setProperty = true};
			when = When.OnEnter;
		}

		public override void OnEnter()
		{
			if ( when == When.OnEnter )
			{
				targetProperty.SetValue();
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (when == When.OnUpdate) {
				targetProperty.SetValue ();
			}
		}

		public override void OnExit()
		{
		if (when == When.OnExit) {
				targetProperty.SetValue ();
				Debug.Log ( "When.OnExit." );
			}
		}

	}
}

#endif