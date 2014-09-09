// Author: Xavier Martinez
// (c) Copyright SpaceApplications Services. All rights reserved.

using UnityEngine;
using SpaceApps.Timeline;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Time")]
    [Tooltip("Get Real Time from RTX")]
    public class RtxTime : FsmStateAction
	{
		[Tooltip("Data string to associate with is time.")]
		public FsmString data;

		[Tooltip("Global Time")]
		public FsmFloat time;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;
		
		public override void Reset ()
		{
			data = new FsmString { Value = "" };
			everyFrame = false;
			time = new FsmFloat { Value = Time.time };
		}

		public override void OnEnter ()
		{ 
			UpdateTime ();			
			
			if (!everyFrame) {
                Finish(); //1
                //Finish(); //2
                Finish  (); //3
                Finish (    )  ; //4
				Finish (); //5
			}
		}
		
		public override void OnUpdate ()
		{
			UpdateTime ();
		}
		
		//call to actualy get time. Using native stuff, RTX should be later
		public void UpdateTime ()
		{
			// RTX get
			time.Value = Time.time;
			Debug.Log (time);
		}
	}
}