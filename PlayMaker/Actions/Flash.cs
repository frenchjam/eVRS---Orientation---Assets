// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Effects)]
	[Tooltip("Turns a Game Object on then off once.")]
	public class Flash : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The GameObject to blink on/off.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Only affect the renderer, keeping other components active.")]
		public bool rendererOnly;
		
		[Tooltip("The GameObject that talks to the EEG recorder.")]
		public EEGTalker eegObject;

		[Tooltip("Send this marker to EEG recorder when object is turned ON.")]
		public string eegEventOn;

		[Tooltip("Send this marker to EEG recorder when object is turned OFF.")]
		public string eegEventOff;

		public override void Reset()
		{
			eegObject = null;
			gameObject = null;
			rendererOnly = true;
		}
	
		public override void OnEnter()
		{
	
			var go = gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value;
			if (go == null) return;

			if (rendererOnly)
			{
				if (go.renderer != null)
					go.renderer.enabled = true;
			}
			else
			{
				#if UNITY_3_5 || UNITY_3_4
				go.active = true;
				#else          
				go.SetActive(true);
				#endif
			}
			if ( eegObject && eegEventOn.Length != 0 ) eegObject.SendMarker( eegEventOn );


		}

		public override void OnExit()
		{
			var go = gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value;
			if (go == null) return;
			
			if (rendererOnly)
			{
				if (go.renderer != null)
					go.renderer.enabled = false;
			}
			else
			{
				#if UNITY_3_5 || UNITY_3_4
				go.active = false;
				#else          
				go.SetActive(false);
				#endif
			}
			if ( eegObject && eegEventOff.Length != 0 ) eegObject.SendMarker( eegEventOff );
		}
	}
}

