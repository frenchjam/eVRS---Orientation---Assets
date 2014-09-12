// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.
// Modified by PsyPhy (J. McIntyre) to allow for action to occur onEnter, onExit or onUpdate.

using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event after an optional delay. NOTE: To send events between FSMs they must be marked as Global in the Events Browser.")]
	public class PsyPhySendEvent : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;
		
		[RequiredField]
		[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		public FsmEvent sendEvent;

		[Tooltip("Do on entry or on exit. Doesn't make sense to do it every frame.")]
		public enum When {OnEnter, OnExit}
		public When when;

		private DelayedEvent delayedEvent;

		public override void Reset()
		{
			eventTarget = null;
			sendEvent = null;
			when = When.OnEnter;
		}

		public override void OnEnter()
		{
			if (when == When.OnEnter) {
				Fsm.Event(eventTarget, sendEvent);
				Finish();
			}
		}

		public override void OnExit()
		{
			if (when == When.OnExit) {
				Fsm.Event(eventTarget, sendEvent);
			}
		}
	}
}