// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sets the Rotation of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class VRPNSurrogate  : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The object that simulates the VRPN input.")]
		public FsmGameObject vrpnObject;

		public bool DrivePosition;
		public bool DriveRotation;

		public override void Reset()
		{
			gameObject = null;
			vrpnObject = null;
		}

		public override void OnEnter()
		{
			DoSetRotation();
		}

		public override void OnUpdate()
		{
			DoSetRotation();
		}

		void DoSetRotation()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}

			var vm = vrpnObject.Value;
			if (vrpnObject == null) return;

			if ( DrivePosition ) go.transform.position = vm.transform.position;
			if ( DriveRotation ) go.transform.rotation = vm.transform.rotation;

		}
	}
}